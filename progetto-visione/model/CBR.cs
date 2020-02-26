using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
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
        private FusionStrategy<string> fusionStrategy;
        private List<Dictionary<string, float[]>> database;

        public CBRTest(int numberOfFeatures, string dbImagesFolder, FusionStrategy<string> fusionStrategy)
        {
            database = new List<Dictionary<string, float[]>>();
            database.Add(Directory.GetFiles(dbImagesFolder, "*.jpg").ToDictionary(item => item, item => new float[] { 0f }));
            this.fusionStrategy = fusionStrategy;
        }

        public bool Train()
        {
            return true;
        }

        public string[] Search(string queryImagePath, int maxImageToRetrive = 10)
        {
            Console.WriteLine(database[0].Count);
            var img = new Image<Bgr, float>(queryImagePath);
             
            Emgu.CV.UI.ImageViewer.Show(img);
            return new string[] { "" };
        }
    }
}
