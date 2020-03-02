using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision.Detector
{
    public class CascadeFaceDetector : FaceDetector
    {

        private CascadeClassifier classifier;
        public CascadeFaceDetector(string faceModel)
        {
            this.classifier = new CascadeClassifier(faceModel);
        }

        public Rectangle[] DetectBoxFaces(IImage image)
        {
            return classifier.DetectMultiScale(image);
        }
    }
}
