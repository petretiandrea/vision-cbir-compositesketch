using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vision.Model;

namespace Vision.Detector
{
    public interface FaceComponentsDetector
    {
        Tuple<PointF[], Dictionary<FaceComponent, Rectangle>> Fit(IImage image);
    }
}
