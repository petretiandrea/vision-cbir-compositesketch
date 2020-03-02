using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.UI;

namespace Vision.Model
{
    public class DefaultFaceComponentsExtractor : FaceComponentsExtractor
    {
        private const double EYES_RATIO = 0.3;
        private const double EYEBROWS_RATIO = 0.15;
        private const double MOUTH_RATIO = 0.6;
        private const double NOSE_RATIO = 1.0;

        public FaceComponents[] ExtractComponentsFromImage(FaceComponents[] facesComponents)
        {
            var componentsImage = facesComponents.Select(rects =>
            {
                return new FaceComponents
                {
                    Eyes = NormalizeAspectRatio(rects.Eyes, EYES_RATIO),
                    EyeBrows = NormalizeAspectRatio(rects.EyeBrows, EYEBROWS_RATIO),
                    Mouth = NormalizeAspectRatio(rects.Mouth, MOUTH_RATIO),
                    Nose = NormalizeAspectRatio(rects.Nose, NOSE_RATIO),
                    //Hair = ExtractSubImage(image, rects.Hair)
                };
            }).ToArray();
            return componentsImage;
        }

        private Rectangle NormalizeAspectRatio(Rectangle rect, double aspectRatio = 1)
        {
            var targetHeight = rect.Width * aspectRatio;
            var padding = Math.Floor((targetHeight - rect.Height) / 2f);
            var target = new Rectangle(rect.Location, rect.Size);
            target.Inflate(0, (int) padding);
            return target;
        }
    }
}
