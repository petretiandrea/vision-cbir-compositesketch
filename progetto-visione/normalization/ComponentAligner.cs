using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vision.Detector;
using Vision.Model;

namespace Vision.Normalization
{
    public struct BoundingBoxParam
    {
        public double AspectRatio { get; set; }
        public int Padding { get; set; }
        public int CropWidth { get; set; }
    }

    // Factory for ComponentAligner, allow to create it with the default component detector
    public static class ComponentAlignerFactory
    {
        private static string FACE_MODEL = @"resources\haarcascade_frontalface_default.xml";
        private static string LANDMARK_MODEL = @"resources\lbfmodel.yaml";
        
        public static ComponentAligner FromReferenceImage(FaceComponentsDetector detector, Dictionary<FaceComponent, BoundingBoxParam> componentParams, Image<Gray, byte> referenceImage)
        {
            var referenceShape = detector.Fit(referenceImage);
            return new ComponentAligner(detector, componentParams, referenceShape);
        }

        public static ComponentAligner FromReferenceImage(Dictionary<FaceComponent, BoundingBoxParam> componentParams, Image<Gray, byte> referenceImage)
        {
            var detector = new FaceLandmarkDetectorLBF(new CascadeFaceDetector(FACE_MODEL), LANDMARK_MODEL);
            return ComponentAlignerFactory.FromReferenceImage(detector, componentParams, referenceImage);
        }

        public static ComponentAligner FromReferenceShape(FaceComponentsDetector detector, Dictionary<FaceComponent, BoundingBoxParam> componentParams, PointF[][] referenceShape)
        {
            return new ComponentAligner(detector, componentParams, referenceShape);
        }

        public static ComponentAligner FromReferenceShape(Dictionary<FaceComponent, BoundingBoxParam> componentParams, PointF[][] referenceShape)
        {
            var detector = new FaceLandmarkDetectorLBF(new CascadeFaceDetector(FACE_MODEL), LANDMARK_MODEL);
            return ComponentAlignerFactory.FromReferenceShape(detector, componentParams, referenceShape);
        }
    }

    public class ComponentAligner : FaceComponentsExtractor
    {
        public FaceComponentsDetector Detector { get; private set; }
        private Dictionary<FaceComponent, BoundingBoxParam> componentParams;

        public PointF[][] ReferenceShape { get; set; }

        public ComponentAligner(FaceComponentsDetector detector, Dictionary<FaceComponent, BoundingBoxParam> componentParams, PointF[][] referenceShape)
        {
            this.Detector = detector;
            this.componentParams = componentParams;
            this.ReferenceShape = referenceShape;
        }

        public FaceComponentContainer ExtractComponentsFromImage(Image<Gray, byte> image)
        {
            var landmarks = Detector.Fit(image);
            if (landmarks.Length < 6) throw new ArgumentException("Detector cannot detect landmarks from input image");

            if (ReferenceShape == null) throw new InvalidOperationException("No Reference shape is set. Must be set before extract components");

            image._EqualizeHist();

            var hair = GetHairComponent(image, landmarks[5], landmarks[4], componentParams[FaceComponent.HAIR]);

            var eyebrows = AlignComponent(image, landmarks[0], ReferenceShape[0], componentParams[FaceComponent.EYEBROWS]);
            var eyes = AlignComponent(image, landmarks[1], ReferenceShape[1], componentParams[FaceComponent.EYES]);
            var nose = AlignComponent(image, landmarks[2], ReferenceShape[2], componentParams[FaceComponent.NOSE]);
            var mouth = AlignComponent(image, landmarks[3], ReferenceShape[3], componentParams[FaceComponent.MOUTH]);
            var shape = landmarks[4].SelectMany(p => new double[] { p.X, p.Y }).ToArray();

            
            return new FaceComponentContainer(hair, eyebrows, eyes, nose, mouth, shape);
        }
        

        /// <summary>
        /// Align and extract a component from face.
        /// </summary>
        /// <param name="image">Image that contains face.</param>
        /// <param name="component">Landmark points of component. Ex. Mouth, Eyes, Eyebrows, ...</param>
        /// <param name="referenceComponent">Reference landmarks points, which is used for alignement.</param>
        /// <param name="param">Paramters for extract the subimage that contains the aligned component.</param>
        /// <returns>Aligned component.</returns>
        public Image<Gray, byte> AlignComponent(Image<Gray, byte> image, PointF[] component, PointF[] referenceComponent, BoundingBoxParam param)
        {
            var transformation = Procrustes.OrdinaryAnalysis(component, referenceComponent);
            var alignedImage = Warp(image, transformation);
            var alignedComponentPoints = CvInvoke.PerspectiveTransform(component, transformation);
            var cropped = CropToBoundingBox(alignedImage, alignedComponentPoints, param);
            return cropped;
        }

        // Hair component is a particular component not identified by landmarks
        // This methods extract hair component starting from the bounding box of face and
        // from the bounding box of shape identified by landmarks.
        public Image<Gray, byte> GetHairComponent(Image<Gray, byte> image, PointF[] face, PointF[] shape, BoundingBoxParam hairParam)
        {
            var shapeBox = PointCollection.BoundingRectangle(shape);
            var faceBox = Rectangle.Ceiling(new RectangleF(face[0].X, face[0].Y, face[1].X - face[0].X, face[1].Y - face[0].Y));

            // calculate height from shape box and top border of image
            var height = image.Size.Height - (image.Size.Height - shapeBox.Top);
            // calculate the normalized height using the desidered aspect ratio
            var normHeight = (int) Math.Floor(faceBox.Width * hairParam.AspectRatio);
            // calculate if the normalized height go out of image border
            var extra = height - normHeight;

            // create the right bounding box, adding on bottom of bounding box the extra height
            var hairBox = (extra < 0) ?
                new Rectangle(faceBox.X, 0, faceBox.Width, height - extra) :
                new Rectangle(faceBox.X, extra, faceBox.Width, normHeight);
            
            var cropHeight = (int)Math.Floor(hairParam.CropWidth * hairParam.AspectRatio);
            return image.GetSubRect(hairBox).Resize(hairParam.CropWidth, cropHeight, Inter.Area);
        }

        // Extract an sub image using creating a bounding box from the landmarks points.
        // Using the BoundingBoxParam, the sub image is resized in order to have same aspect ratio across different subject.
        public Image<Gray, byte> CropToBoundingBox(Image<Gray, byte> image, PointF[] componentPoints, BoundingBoxParam param)
        {
            var bounding = GetBoundingBox(componentPoints, param.AspectRatio, param.Padding);
            var cropHeight = (int) Math.Ceiling(param.CropWidth * param.AspectRatio);
            return image.GetSubRect(bounding).Resize(param.CropWidth, cropHeight, Inter.Cubic);
        }

        // Create rectangle bounding box with specific aspect ratio starting from a set of landarmarks points.
        // Border padding is used for expand the bounding box mantaining same aspect ratio.
        public Rectangle GetBoundingBox(PointF[] componentPoints, double ratio, int borderPadding)
        {
            var rect = PointCollection.BoundingRectangle(componentPoints);
            var height = (int) Math.Ceiling(rect.Width * ratio);
            var diff = Math.Ceiling((height - rect.Height) / 2d);
            rect.Inflate(0, (int) diff);
            rect.Inflate(borderPadding, borderPadding);
            return rect;
        }


        public Image<Gray, byte> Warp(Image<Gray, byte> img, Matrix<float> transformation)
        {
            var copy = new Image<Gray, byte>(img.Size);
            CvInvoke.WarpPerspective(img, copy, transformation, copy.Size,
                Inter.Area, Emgu.CV.CvEnum.Warp.Default, BorderType.Constant, new MCvScalar(0));
            return copy;
        }
    }
}
