using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emgu.CV.Face;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing;
using Emgu.CV.UI;
using Vision.Model;
using System.Collections;
using Emgu.CV.CvEnum;

namespace Vision.Detector
{
    public class FaceLandmarkDetectorLBF : FaceComponentsDetector
    {
        private static Range EYES_POINT_RANGE = new Range(36, 47);
        private static Range EYEBROWS_POINT_RANGE = new Range(17, 26);
        private static Range NOSE_POINT_RANGE = new Range(28, 35);
        private static Range MOUTH_POINT_RANGE = new Range(48, 67);

        private static Size DEFAULT_PADDING = new Size(10, 0);

        private FaceDetector faceDetector;
        private FacemarkLBF facemark;
        

        public FaceLandmarkDetectorLBF(FaceDetector faceDetector, string landmarkModel)
        {
            this.faceDetector = faceDetector;
            this.facemark = new FacemarkLBF(new FacemarkLBFParams());
            this.facemark.LoadModel(landmarkModel);
        }

        public Tuple<PointF[], Dictionary<FaceComponent, Rectangle>> Fit(IImage image)
        {
            var faces = new VectorOfRect(faceDetector.DetectBoxFaces(image));
            var landmarks = new VectorOfVectorOfPointF();

            if (facemark.Fit(image, faces, landmarks))
            {
                var faceShape = landmarks.ToArrayOfArray().First();
                var components = ExtractFacialComponentRects(faceShape);
                components.Add(FaceComponent.HAIR, ExtractHairRectangle(image, components[FaceComponent.EYEBROWS]));
                return Tuple.Create(faceShape, components);
            }
            return null;
        }

        private Dictionary<FaceComponent, Rectangle> ExtractFacialComponentRects(PointF[] landamarksPoints)
        {
            return new Dictionary<FaceComponent, Rectangle>()
            {
                { FaceComponent.EYES, ExtractRectFromLandmarks(landamarksPoints, EYES_POINT_RANGE, DEFAULT_PADDING) },
                { FaceComponent.EYEBROWS, ExtractRectFromLandmarks(landamarksPoints, EYEBROWS_POINT_RANGE) },
                { FaceComponent.NOSE, ExtractRectFromLandmarks(landamarksPoints, NOSE_POINT_RANGE, DEFAULT_PADDING) },
                { FaceComponent.MOUTH, ExtractRectFromLandmarks(landamarksPoints, MOUTH_POINT_RANGE, DEFAULT_PADDING) }
            };
        }
        
        private Rectangle ExtractRectFromLandmarks(PointF[] landamarksPoints, Range range, Size padding = default(Size))
        {
            var points = landamarksPoints.Skip(range.Start)
                .Take(range.End - range.Start + 1)
                .ToArray();
            var rect = PointCollection.BoundingRectangle(points);
            rect.Inflate(padding.Width, padding.Height);
            return rect;
        }

        private Rectangle ExtractHairRectangle(IImage image, Rectangle eyebrowsRect)
        {
            var hairHeight = image.Size.Height - (image.Size.Height - eyebrowsRect.Top);
            return new Rectangle(0, 0, image.Size.Width, hairHeight);
        }
    }
}
