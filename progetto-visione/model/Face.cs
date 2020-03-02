using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;

namespace Vision.Model
{
    public class FaceComponents
    {
        public Rectangle Eyes { get; set; }
        public Rectangle EyeBrows { get; set; }
        public Rectangle Nose { get; set; }
        public Rectangle Mouth { get; set; }
        public Rectangle Hair { get; set; }
    }
    
    public interface FaceComponentsExtractor
    {
        FaceComponents[] ExtractComponentsFromImage(FaceComponents[] facesComponents);
    }
}
