using System;
using System.Linq;
using Emgu.CV;
using System.Drawing;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System.Collections.Generic;

namespace Vision.Detector
{
    public class HaarEyesDetector : EyesDetector
    {
        private CascadeClassifier classifier;

        public HaarEyesDetector(string haarEyesModel)
        {
            classifier = new CascadeClassifier(haarEyesModel);
        }
        private readonly object syncLock = new object();
        public Eyes DetectEyes(IImage image)
        {
            lock(syncLock)
            {
                var facesRect = classifier.DetectMultiScale(image, 1.1, 3);
                //Console.WriteLine("Eyes Detected: " + facesRect.Length);
                if (facesRect.Length < 2) throw new ArgumentException("The image does not contains a face or the detector cannot");

                return FindBestCandidateEyes(facesRect);
            }
        }

        private Eyes FindBestCandidateEyes(Rectangle[] rects)
        {
            var candidate = new List<Eyes>();
            for (int i = 0; i < rects.Length; i++)
            {
                var rect1 = rects[i];
                for (int j = i + 1; j < rects.Length; j++)
                {
                    var rect2 = rects[j];

                    if (Math.Abs(rect1.Y - rect2.Y) < (0.66d * rect1.Height) && // 2/3 of height
                        Math.Abs(rect1.X - rect2.X) > rect1.Width + (0.33 * rect1.Width)) // 1/3 of width
                    {
                        candidate.Add(CreateEyesFromRects(rect1, rect2));
                    }
                }
            }
            if(candidate.Count == 0) throw new ArgumentException("The image does not contains a face or the detector cannot");
            var maxYdelta = candidate.Select(e => Math.Abs(e.Left.Y - e.Right.Y)).Max();
            return candidate.OrderByDescending(e => maxYdelta - Math.Abs(e.Left.Y - e.Right.Y) + Math.Abs(e.Left.X - e.Right.X)).First();
        }

        private Eyes CreateEyesFromRects(Rectangle r1, Rectangle r2)
        {
            return r1.X < r2.X ? Eyes.Create(r1, r2) : Eyes.Create(r2, r1);
        } 
    }
}
