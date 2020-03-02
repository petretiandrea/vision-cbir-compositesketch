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
using Vision.Model.Extractor;

namespace Vision.Model
{
    interface CBR
    {
        bool Train();
        string[] Search(string queryImagePath, int maxImageToRetrive = 10);
    }


    class CBRTest : CBR
    {
        static string FACE_MODEL = @"C:\Users\Petreti Andrea\Desktop\progetto-visione\haarcascade_frontalface_default.xml";
        static string LANDMARK_MODEL = @"C:\Users\Petreti Andrea\Desktop\progetto-visione\lbfmodel.yaml";

        private FaceComponentsDetector landmark = new FaceLandmarkDetectorLBF(new CascadeFaceDetector(FACE_MODEL), LANDMARK_MODEL);
        private FaceComponentsExtractor faceNorm = new DefaultFaceComponentsExtractor();

        private FusionStrategy<string> fusionStrategy;
        private string dbImagesFolder;
        private List<Dictionary<string, DenseHistogram[]>> database;

        public CBRTest(string dbImagesFolder, FusionStrategy<string> fusionStrategy)
        {
            this.database = new List<Dictionary<string, DenseHistogram[]>>();
            this.dbImagesFolder = dbImagesFolder;
            this.fusionStrategy = fusionStrategy;
        }

        public bool Train()
        {
            /*database = Directory.GetFiles(dbImagesFolder, "*.jpg").ToDictionary(item => item, item =>
            {
                var img = Preprocessing.PreprocessImage(new Image<Bgr, byte>(item));
                var imgComponents = faceNorm.NormalizeComponents(landmark.DetectFaceComponents(img)).First();
                return ComputeFaceComponentsHists(img, imgComponents);
            });*/
            return true;
        }

        public string[] Search(string queryImagePath, int maxImageToRetrive = 10)
        {
            Console.WriteLine(database[0].Count);
            var img = new Image<Bgr, float>(queryImagePath);
             
            Emgu.CV.UI.ImageViewer.Show(img);
            return new string[] { "" };
        }

        public Dictionary<string, DenseHistogram[]> ComputeFaceComponentsHists(Image<Gray, byte> img, FaceComponents components)
        {
            var lbp = new LBP<byte>();
            var dict = new Dictionary<string, DenseHistogram[]>();

            var eyes = img.GetSubRect(components.Eyes);
            //dict.Add("EYES", BlockLBPH(lbp, eyes, 5));

            /*var mouth = img.GetSubRect(components.Mouth);
            dict.Add("MOUTH", BlockLBPH(mouth, 5));

            var nose = img.GetSubRect(components.Nose);
            dict.Add("NOSE", BlockLBPH(nose, 5));

            var eyeBrows = img.GetSubRect(components.EyeBrows);
            eyeBrows.Resize(300, 45, Inter.Area);
            dict.Add("EYEBROWS", BlockLBPH(eyeBrows, 5));*/

            // todo: add hair component
            return dict;
        }
    }
}
