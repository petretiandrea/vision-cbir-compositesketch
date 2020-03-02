using System;
using Emgu.CV;
using System.Drawing;
using System.Threading;
using Emgu.CV.UI;

namespace Vision.Detector
{
    public class Eyes
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

    public interface EyesDetector
    {
        Eyes DetectEyes(IImage image);
    }

    static class Extension
    {
        public static Point GetCenter(this Rectangle rect)
        {
            return new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        }
    }
}
