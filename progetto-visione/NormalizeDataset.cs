using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.CvEnum;
using System.Drawing;
using System.Runtime.InteropServices;
using Vision.Model;
using System.IO;

namespace Vision
{
    class NormalizeDataset
    {
        static string PHOTO_PATH = @"C:\Users\Petreti Andrea\Desktop\progetto-visione\dataset\cuhk\CUHK_training_photo\photo\";
        static string SKETCH_PATH = @"C:\Users\Petreti Andrea\Desktop\progetto-visione\dataset\cuhk\CUHK_training_sketch\sketch\";
        static string PHOTO2_PATH = @"C:\Users\Petreti Andrea\Desktop\progetto-visione\dataset\cuhk\CUHK_testing_photo\photo\";
        static string SKETCH2_PATH = @"C:\Users\Petreti Andrea\Desktop\progetto-visione\dataset\cuhk\CUHK_testing_sketch\sketch\";


        public void Execute()
        {
            var classifier = new CascadeClassifier(@"C:\Users\Petreti Andrea\Desktop\progetto-visione\haarcascade_eye_tree_eyeglasses.xml");
            var norm = new FaceNormalizer(classifier);
            
            NormalizeDataSet(PHOTO_PATH, norm);
            NormalizeDataSet(SKETCH_PATH, norm);
            NormalizeDataSet(PHOTO2_PATH, norm);
            NormalizeDataSet(SKETCH2_PATH, norm);
        }

        private void NormalizeDataSet(string datasetPath, FaceNormalizer normalizer)
        {
            var targetFolder = Directory.CreateDirectory(Path.Combine(datasetPath, "normalized"));
            var files = Directory.GetFiles(datasetPath, "*.jpg");
            Console.WriteLine("Total Items: " + files.Length);
            for(int i = 0; i < files.Length; i++)
            {
                var filename = Path.GetFileName(files[i]);
                Console.WriteLine("Processing: {0}/{1} - {2}", (i + 1), files.Length, filename);
                var img = new Image<Bgr, byte>(files[i]);
                var target = normalizer.Normalize(img, 300, 350);
                if(target != null)
                {
                    target.Save(Path.Combine(targetFolder.FullName, filename));
                    target.Dispose();
                }
            }
        }

        private Image<Bgr, byte> GetPhotoFromSet(string imgName)
        {
            return new Image<Bgr, byte>(PHOTO_PATH + imgName);
        }

        private Image<Bgr, byte> GetSketchFromSet(string imgName)
        {
            return new Image<Bgr, byte>(SKETCH_PATH + imgName);
        }
    }
}
