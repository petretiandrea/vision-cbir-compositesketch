using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision.Detector
{
    public interface FaceDetector
    {
        Rectangle[] DetectBoxFaces(IImage image);
    }
}
