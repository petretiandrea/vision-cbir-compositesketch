using System;
using System.Linq;
using Emgu.CV;
using System.Drawing;

namespace Vision.Detector
{
    public class HaarEyesDetector : EyesDetector
    {
        private CascadeClassifier classifier;
        public HaarEyesDetector(string haarEyesModel)
        {
            classifier = new CascadeClassifier(haarEyesModel);
        }

        public Eyes DetectEyes(IImage image)
        {
            var facesRect = classifier.DetectMultiScale(image, 1.1, 8);
            Console.WriteLine("Eyes Detected: " + facesRect.Length);
            if (facesRect.Length < 2) return null;

            Rectangle eye1;
            Rectangle eye2;
            if (facesRect.Length > 2)
            {
                // find the two bigger box
                var eyes = facesRect.Select((box, index) => new { box, index })
                    .OrderByDescending(item => item.box.Width * item.box.Height)
                    .Take(2)
                    .ToArray();
                eye1 = eyes[0].box;
                eye2 = eyes[1].box;
            }
            else
            {
                eye1 = facesRect[0];
                eye2 = facesRect[1];
            }

            return eye1.X < eye2.X ? Eyes.Create(eye1, eye2) : Eyes.Create(eye2, eye1);
        }
    }
}
