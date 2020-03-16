using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Vision.Detector;

namespace Vision.Model
{
    public struct BoundingBoxParam
    {
        public double AspectRatio { get; set; }
        public int TargetWidth { get; set; }
    }

    public class PhotoSketchFaceComponentsExtractor : FaceComponentsExtractor
    {
        public FaceComponentsDetector Detector { get; private set; }
        private Dictionary<FaceComponent, BoundingBoxParam> boundingBoxParams;

        public PhotoSketchFaceComponentsExtractor(FaceComponentsDetector detector, BoundingBoxParam hair, BoundingBoxParam eyebrows, 
            BoundingBoxParam eyes, BoundingBoxParam nose, BoundingBoxParam mouth)
        {
            Detector = detector;
            boundingBoxParams = new Dictionary<FaceComponent, BoundingBoxParam>();
            boundingBoxParams[FaceComponent.HAIR] = hair;
            boundingBoxParams[FaceComponent.EYEBROWS] = eyebrows;
            boundingBoxParams[FaceComponent.EYES] = eyes;
            boundingBoxParams[FaceComponent.NOSE] = nose;
            boundingBoxParams[FaceComponent.MOUTH] = mouth;
        }

        public FaceComponentContainer<Image<TColor, TDepth>, float[]> ExtractComponentsFromImage<TColor, TDepth>(Image<TColor, TDepth> image)
            where TColor : struct, IColor
            where TDepth : new()
        {
            if (image == null) return FaceComponentContainer.Empty<Image<TColor, TDepth>, float[]>();
            var faceComponentBoxes = Detector.Fit(image);
            var subImages = boundingBoxParams
                .Select(kv => new { kv.Key, Value = ExtractSubImage(image, faceComponentBoxes.GetComponent(kv.Key), kv) })
                .ToDictionary(tuple => tuple.Key, tuple => tuple.Value);

            var shape = faceComponentBoxes.Shape.SelectMany(p => new float[] { p.X, p.Y }).ToArray();
            return FaceComponentContainer.FromDictionary(subImages, shape);
        }

        private Image<TColor, TDepth> ExtractSubImage<TColor, TDepth>(Image<TColor, TDepth> image, Rectangle boundingBox, KeyValuePair<FaceComponent, BoundingBoxParam> normParams)
            where TColor : struct, IColor
            where TDepth : new()
        {
            var normBoundingBox = normParams.Key == FaceComponent.HAIR ?
                NormalizeHairAspectRatio(image, boundingBox) : 
                NormalizeAspectRatio(boundingBox, normParams.Value.AspectRatio);

            /*var i = image as Image<Gray, byte>;
            i.Draw(normBoundingBox, new Gray(0), 1);
            ImageViewer.Show(i);*/
            try
            {
                var resizedImage = ResizeFromParams(image.GetSubRect(normBoundingBox), normParams.Value);
                //resizedImage.Save(Path.Combine("faces_pieces", $"{image.GetHashCode()}-{normParams.Key}.jpg"));
                //Console.WriteLine("Box: {0}, TR: {1}, R: {2}", normBoundingBox, resizedImage.Size, resizedImage.Height / (float)resizedImage.Width);
                return resizedImage;
            } catch(Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }

        private Rectangle NormalizeHairAspectRatio<TColor, TDepth>(Image<TColor, TDepth> image, Rectangle hairRect)
            where TColor : struct, IColor
            where TDepth : new()
        {
            var hairNormParams = boundingBoxParams[FaceComponent.HAIR];
            var targetHeight = (int) Math.Floor(hairRect.Width * hairNormParams.AspectRatio);
            var padding = hairRect.Height - targetHeight;
            if(padding < 0)
            {
                var a = new Rectangle(hairRect.Left, hairRect.Top, hairRect.Width, targetHeight + (-padding));
                /*var i = image as Image<Gray, byte>;
                i.Draw(a, new Gray(255), 1);
                ImageViewer.Show(i);*/
                return a;
            }
            return new Rectangle(hairRect.X, hairRect.Top + padding, hairRect.Width, targetHeight);
            /*
            var excessHeight = (int) Math.Floor((image.Width * hairNormParams.AspectRatio) - hairRect.Height);
            hairRect.Height += excessHeight;*
            
            /*var color = image.Convert<Bgr, byte>();   
            color.Draw(hairRect, new Bgr(0, 255, 0));
            ImageViewer.Show(color);*/

            //return hairRect;
        }

        private Image<TColor, TDepth> ResizeFromParams<TColor, TDepth>(Image<TColor, TDepth> image, BoundingBoxParam param)
            where TColor : struct, IColor
            where TDepth : new()
        {
            var targetHeigth = (int) Math.Floor(param.TargetWidth * param.AspectRatio);
            return image.Resize(param.TargetWidth, targetHeigth, Emgu.CV.CvEnum.Inter.Cubic);
        }

        private Rectangle NormalizeAspectRatio(Rectangle rect, double aspectRatio = 1)
        {
            var targetHeight = Math.Ceiling(rect.Width * aspectRatio);
            var padding = Math.Ceiling((targetHeight - rect.Height) / 2d);
            var target = new Rectangle(rect.Location, rect.Size);
            target.Inflate(0, (int) padding);
            return target;
        }
    }
}
