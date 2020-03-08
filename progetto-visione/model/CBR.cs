using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Vision.Detector;
using Vision.Model;
using Vision.Model.Extractor;

namespace Vision.CBR
{
    public interface CBR
    {
        void Train(string[] photoDb);
        Rank<string, double> Search(string queryImagePath, int maxImageToRetrive = 10);
    }
}
