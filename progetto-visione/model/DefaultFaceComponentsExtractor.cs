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

        public FaceComponentContainer<Image<TColor, TDepth>, float[]> ExtractComponentsFromImage<TColor, TDepth>(Image<TColor, TDepth> image)
            where TColor : struct, IColor
            where TDepth : new()
        {
            if (image == null) return FaceComponentContainer.Empty<Image<TColor, TDepth>, float[]>();
            var faceComponentBoxes = Detector.Fit(image);
            var subImages = NORMALIZE_PARAMS
                .Select(kv => new { kv.Key, Value = ExtractSubImage(image, faceComponentBoxes.GetComponent(kv.Key), kv) })
                .ToDictionary(tuple => tuple.Key, tuple => tuple.Value);

            var shape = faceComponentBoxes.Shape.SelectMany(p => new float[] { p.X, p.Y }).ToArray();
            return FaceComponentContainer.FromDictionary(subImages, shape);
        }

        private Image<TColor, TDepth> ExtractSubImage<TColor, TDepth>(Image<TColor, TDepth> image, Rectangle boundingBox, KeyValuePair<FaceComponent, FaceComponentParams> normParams)
            where TColor : struct, IColor
            where TDepth : new()
        {
            var normBoundingBox = normParams.Key == FaceComponent.HAIR ?
                NormalizeHairAspectRatio(image, boundingBox) : 
                NormalizeAspectRatio(boundingBox, normParams.Value.AspectRatio);
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
