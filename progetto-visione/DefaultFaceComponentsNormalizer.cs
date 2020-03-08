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

        public Dictionary<FaceComponent, Image<TColor, TDepth>> ExtractComponentsFromImage<TColor, TDepth>(Image<TColor, TDepth> image)
            where TColor : struct, IColor
            where TDepth : new()
        {
            var faceComponentBoxes = Detector.DetectFaceComponents(image);
            var subImages = faceComponentBoxes
                //.AsParallel()
                .Select(keyvalue => new { keyvalue.Key, Value = ExtractSubImage(image, keyvalue) })
                //.AsSequential()
                .ToDictionary(tuple => tuple.Key, tuple => tuple.Value);

            if(!subImages.ContainsKey(FaceComponent.HAIR))
            {
                subImages.Add(FaceComponent.HAIR, ExtractHair(image, faceComponentBoxes[FaceComponent.EYEBROWS]));
            }

            return subImages;
        }

        private Image<TColor, TDepth> ExtractSubImage<TColor, TDepth>(Image<TColor, TDepth> image, KeyValuePair<FaceComponent, Rectangle> boundingBox)
            where TColor : struct, IColor
            where TDepth : new()
        {
            var normParams = NORMALIZE_PARAMS[boundingBox.Key];
            var normBoundingBox = NormalizeAspectRatio(boundingBox.Value, normParams.AspectRatio);
            var resizedImage = ResizeFromParams(image.GetSubRect(normBoundingBox), normParams);

            Console.WriteLine("Box: {0}, TR: {1}, R: {2}", normBoundingBox, resizedImage.Size, resizedImage.Height / (float)resizedImage.Width);
            return resizedImage;
        }

        private Image<TColor, TDepth> ExtractHair<TColor, TDepth>(Image<TColor, TDepth> image, Rectangle eyebrows)
            where TColor : struct, IColor
            where TDepth : new()
        {
            var hairNormParams = NORMALIZE_PARAMS[FaceComponent.HAIR];
            var hairHeight = image.Height - (image.Height - eyebrows.Top);
            var boundingBox = new Rectangle(0, 0, image.Width, hairHeight);
            var marginBottom = (int) Math.Floor(Math.Abs((image.Width * hairNormParams.AspectRatio) - boundingBox.Height));
            boundingBox.Height -= marginBottom;

            var resizedImage = ResizeFromParams(image.GetSubRect(boundingBox), hairNormParams);

            Console.WriteLine("a: {0}, {1}", boundingBox, resizedImage.Height / (float)resizedImage.Width);
            return resizedImage;
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
            var targetHeight = Math.Floor(rect.Width * aspectRatio);
            var padding = Math.Floor((targetHeight - rect.Height) / 2d);
            var target = new Rectangle(rect.Location, rect.Size);
            target.Inflate(0, (int) padding);
            return target;
        }
    }
}
