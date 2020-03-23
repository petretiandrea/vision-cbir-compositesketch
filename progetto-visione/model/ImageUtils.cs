using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision
{
    public static class ImageUtils
    {
        public static Rectangle[] SplitIntoBlocks<TColor, TDepth>(Image<TColor, TDepth> img, int size, int stride)
            where TColor : struct, IColor
            where TDepth : new()
        {
            if (img.Width < size && img.Height < size) throw new ArgumentException("The height or width of image is smaller than size"); 
            var n = (img.Width - size) / stride + 1;
            var m = (img.Height - size) / stride + 1;
            var rectangles = new List<Rectangle>(n * m);
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    var r = new Rectangle(
                        j * stride,
                        i * stride,
                        size,
                        size);
                    rectangles.Add(r);
                }
            }
            return rectangles.ToArray();
        }
        
        public static int CountBlocks<TColor, TDepth>(Image<TColor, TDepth> img, int size, int stride)
            where TColor : struct, IColor
            where TDepth : new()
        {
            var n = (img.Width - size) / stride + 1;
            var m = (img.Height - size) / stride + 1;
            return n * m;
        }

        public static Image<Gray, byte> CreateAvgImage(int width, int height, IEnumerable<Image<Gray, byte>> images)
        {
            var avg = new Image<Gray, double>(width, height);
            var count = 0;
            foreach (var img in images)
            {
                avg += img.Convert<Gray, double>();
                count++;
            }
            avg /= count;
            return avg.Convert<Gray, byte>();
        }
    }
}
