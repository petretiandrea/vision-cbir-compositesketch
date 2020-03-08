using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;
using Vision.Detector;

namespace Vision.Model
{
    public enum FaceComponent
    {
        EYES,
        EYEBROWS,
        NOSE,
        MOUTH,
        HAIR
    }

    public class FaceComponentBoxes
    {
        public Rectangle Eyes { get; set; }
        public Rectangle EyeBrows { get; set; }
        public Rectangle Nose { get; set; }
        public Rectangle Mouth { get; set; }
        public Rectangle Hair { get; set; }
    }
    
    public interface FaceComponentsExtractor
    {
        FaceComponentsDetector Detector { get; }
        Dictionary<FaceComponent, Image<TColor, TDepth>> ExtractComponentsFromImage<TColor, TDepth>(Image<TColor, TDepth> image)
            where TColor : struct, IColor
            where TDepth : new();
    }
}
