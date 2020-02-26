using System;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using System.Linq;

namespace Vision.Model
{
    class Eyes
    {
        public Rectangle Left { get; private set; }
        public Rectangle Right { get; private set; }
        public static Eyes Create(Rectangle left, Rectangle right) { return new Eyes(left, right); }
        private Eyes(Rectangle left, Rectangle right)
        {
            Left = left;
            Right = right;
        }
    }

    static class EyesDetector
    {
        public static Eyes GetEyes<TDepth>(CascadeClassifier eyesDector, Image<Bgr, TDepth> originalImage) where TDepth : new()
        {
            var facesRect = eyesDector.DetectMultiScale(originalImage, 1.1, 8);
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

    static class Extension
    {
        public static Point GetCenter(this Rectangle rect)
        {
            return new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        }
    }
}
