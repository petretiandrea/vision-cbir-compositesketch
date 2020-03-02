using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vision.Model;

namespace Vision.Detector
{
    public interface FaceComponentsDetector
    {
        FaceComponents[] DetectFaceComponents(IImage image);
    }
}
