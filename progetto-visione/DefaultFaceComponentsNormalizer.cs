using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Vision.Detector;

namespace Vision.Model
{
    struct FaceComponentParams
    {
        public double AspectRatio { get; set; }
        public int UpscaleWidth { get; set; }
    }

    public class DefaultFaceComponentsExtractor : FaceComponentsExtractor
    {
        private readonly Dictionary<FaceComponent, FaceComponentParams> NORMALIZE_PARAMS = new Dictionary<FaceComponent, FaceComponentParams>()
        {
            { FaceComponent.EYES, new FaceComponentParams { AspectRatio = 0.3, UpscaleWidth = 300 }  },
            { FaceComponent.EYEBROWS, new FaceComponentParams { AspectRatio = 0.15, UpscaleWidth = 320 }  },
            { FaceComponent.MOUTH, new FaceComponentParams { AspectRatio = 0.6, UpscaleWidth = 200 }  },
            { FaceComponent.NOSE, new FaceComponentParams { AspectRatio = 1.0, UpscaleWidth = 120 } },
            { FaceComponent.HAIR, new FaceComponentParams { AspectRatio = 0.35, UpscaleWidth = 150 } }
        };

        public FaceComponentsDetector Detector { get; private set; }

        public DefaultFaceComponentsExtractor(FaceComponentsDetector detector)
        {
            Detector = detector;
        }

        public Face<Image<TColor, TDepth>> ExtractComponentsFromImage<TColor, TDepth>(Image<TColor, TDepth> image)
            where TColor : struct, IColor
            where TDepth : new()
        {
            var faceComponentBoxes = Detector.Fit(image);
            var subImages = faceComponentBoxes.Item2
                .Select(keyvalue => new { keyvalue.Key, Value = ExtractSubImage(image, keyvalue) })
                .ToDictionary(tuple => tuple.Key, tuple => tuple.Value);

            var shape = faceComponentBoxes.Item1.Aggregate(new List<float>(), (acc, p) => { acc.Add(p.X); acc.Add(p.Y); return acc; }).ToArray();
            return Face.FromDictionary(subImages, shape);
        }

        private Image<TColor, TDepth> ExtractSubImage<TColor, TDepth>(Image<TColor, TDepth> image, KeyValuePair<FaceComponent, Rectangle> boundingBox)
            where TColor : struct, IColor
            where TDepth : new()
        {
            var normParams = NORMALIZE_PARAMS[boundingBox.Key];
            var normBoundingBox = boundingBox.Key == FaceComponent.HAIR ?
                NormalizeHairAspectRatio(image, boundingBox.Value) : 
                NormalizeAspectRatio(boundingBox.Value, normParams.AspectRatio);
            try
            {
                var resizedImage = ResizeFromParams(image.GetSubRect(normBoundingBox), normParams);

                Console.WriteLine("Box: {0}, TR: {1}, R: {2}", normBoundingBox, resizedImage.Size, resizedImage.Height / (float)resizedImage.Width);
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
            var hairNormParams = NORMALIZE_PARAMS[FaceComponent.HAIR];
            var excessHeight = (int) Math.Floor((image.Width * hairNormParams.AspectRatio) - hairRect.Height);
            hairRect.Height += excessHeight;
            
            /*var color = image.Convert<Bgr, byte>();
            color.Draw(hairRect, new Bgr(0, 255, 0));
            ImageViewer.Show(color);*/

            return hairRect;
        }

        private Image<TColor, TDepth> ResizeFromParams<TColor, TDepth>(Image<TColor, TDepth> image, FaceComponentParams param)
            where TColor : struct, IColor
            where TDepth : new()
        {
            var targetHeigth = (int) Math.Floor(param.UpscaleWidth * param.AspectRatio);
            return image.Resize(param.UpscaleWidth, targetHeigth, Emgu.CV.CvEnum.Inter.Cubic);
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
