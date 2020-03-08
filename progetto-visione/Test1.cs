using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV.Face;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing;
using Emgu.CV.UI;
using Vision.Model;
using System.IO;
using System.Threading;
using Emgu.CV.CvEnum;
using Vision.Model.Extractor;
using Vision.Detector;
using Vision.Preprocess;
using Vision.CBR;

namespace Vision
{
    
    // Cozzaglia di codice per testare i vari algoritmi TODO: refactor
    class Test1
    {
        static string PHOTO_PATH = @"C:\Users\Petreti Andrea\Desktop\progetto-visione\dataset\cuhk\CUHK_training_photo\photo\";
        static string SKETCH_PATH = @"C:\Users\Petreti Andrea\Desktop\progetto-visione\dataset\cuhk\CUHK_training_sketch\sketch\";

        static string FACE_MODEL = @"resources\haarcascade_frontalface_default.xml";
        static string LANDMARK_MODEL = @"resources\lbfmodel.yaml";

        public void Run()
        {
            //var image = GetPhotoFromSet("f-006-01.jpg");
            /*var landmark = new FaceLandmarkDetectorLBF(new CascadeFaceDetector(FACE_MODEL), LANDMARK_MODEL);
            var faceNorm = new DefaultFaceComponentsExtractor();


            var photos = Directory.GetFiles(PHOTO_PATH);
            var sketches = Directory.GetFiles(SKETCH_PATH);
            var eyesMean = new Mat(45, 150, Emgu.CV.CvEnum.DepthType.Cv64F, 1);
            eyesMean.SetTo(new MCvScalar(0));

            var sketch = new Image<Bgr, byte>(sketches[1]);
            var normSketch = Preprocessing.PreprocessImage(sketch);*/

            /*var classificationRank = new int[] { 5, 10, 30, 50 };

            var photoTraining = Directory.GetFiles(PHOTO_PATH).Take(15).ToArray();
            var sketchTraining = Directory.GetFiles(SKETCH_PATH).Take(5).ToArray();

            var a = new PhotoSketchCBR();
            Console.WriteLine("Training....");
            a.Train(photoTraining);
            Console.WriteLine("Train done.");

            var contained = new float[4];
            for (int i = 0; i < sketchTraining.Length; i++)
            {
                var rank = a.Search(sketchTraining[i]);
                for (int j = 0; j < contained.Length; j++)
                {
                    contained[j] += rank.Take(classificationRank[j]).Contains(photoTraining[i]) ? 1 : 0;
                }
            }

            for(int j = 0; j < classificationRank.Length; j++)
            {
                Console.WriteLine("Rank-{0} accuracy: {1}%, Sketch: {2} over {3} photos", 
                    classificationRank[j], 
                    (contained[j] / (float)sketchTraining.Length) * 100f,
                    sketchTraining.Length,
                    photoTraining.Length);
            }*/
        }

        private Image<Bgr, byte> GetPhotoFromSet(string imgName)
        {
            return new Image<Bgr, byte>(PHOTO_PATH + imgName);
        }

        private Image<Bgr, byte> GetSketchFromSet(string imgName)
        {
            return new Image<Bgr, byte>(SKETCH_PATH + imgName);
        }

        private void DrawFaceComponentsRect(Image<Gray, byte> img, FaceComponentBoxes face)
        {
            img.Draw(face.Eyes, new Gray(0));
            img.Draw(face.EyeBrows, new Gray(0));
            img.Draw(face.Nose, new Gray(0));
            img.Draw(face.Mouth, new Gray(0));
        }

        private void CalculateSimilarity(Dictionary<string, DenseHistogram[]> photo, Dictionary<string, DenseHistogram[]> sketch)
        {
            foreach(var entry in photo)
            {
                var similarity = 0d;
                for(var i = 0; i < entry.Value.Length; i++)
                {
                    var photoHist = entry.Value[i];
                    var sketchHist = sketch[entry.Key][i];
                    var normFactor = Math.Min(photoHist.GetBinValues().Sum(), sketchHist.GetBinValues().Sum());
                    similarity += CvInvoke.CompareHist(photoHist, sketchHist, HistogramCompMethod.Intersect) / normFactor;
                }
                Console.WriteLine("Sim of {0}: {1}", entry.Key, similarity);
            }
        }

        public struct FaceFeature
        {
            public float[] EyesFeature { get; set; }
            public float[] EyeBrowsFeature { get; set; }
            public float[] MouthFeature { get; set; }
            public float[] NoseFeature { get; set; }
            public float[] HairFeature { get; set; }
        }


        // var lbpEyes = new HistogramBlockLBP(5, MultiscaleLBP.Create(1, 3, 5, 7));

        public Dictionary<FaceComponent, float[]> ComputeFaceComponentsHists(Dictionary<FaceComponent, Image<Gray, byte>> componentsImage, Dictionary<FaceComponent, FeatureExtractor> featureExtractors)
        {

            /* dict.Add(FaceComponent.EYES, lbpEyes.ExtractDescriptor(eyes));


             dict.Add(FaceComponent.MOUTH, lbpEyes.ExtractDescriptor(mouth));
             */
            /*var nose = img.GetSubRect(components.Nose);
            dict.Add("NOSE", BlockLBPH(nose, 5));

            var eyeBrows = img.GetSubRect(components.EyeBrows);
            eyeBrows.Resize(300, 45, Inter.Area);
            dict.Add("EYEBROWS", BlockLBPH(eyeBrows, 5));*/

            // todo: add hair component
            //return dict;
            return null;
        } 

        private void PrintLBP(int pattern)
        {
            Console.WriteLine("Pattern: {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}",
                Convert.ToInt16((pattern & (1 << 0)) == 1),
                Convert.ToInt16((pattern & (1 << 1)) == 2),
                Convert.ToInt16((pattern & (1 << 2)) == 4),
                Convert.ToInt16((pattern & (1 << 3)) == 8),
                Convert.ToInt16((pattern & (1 << 4)) == 16),
                Convert.ToInt16((pattern & (1 << 5)) == 32),
                Convert.ToInt16((pattern & (1 << 6)) == 64),
                Convert.ToInt16((pattern & (1 << 7)) == 128));
        }

        private void PrintHistogram(DenseHistogram hist)
        {
            Console.Write("Histogram values: ");
            foreach (var val in hist.GetBinValues())
            {
                Console.Write("{0}, ", val);
            }
            Console.WriteLine();
        }

        private void PrintMatrix<T>(Image<Gray, T> mat) where T : new()
        {
            for(int i = 0; i < mat.Rows; i++)
            {
                for(int j = 0; j < mat.Cols; j++)
                {
                    Console.Write("{0}, ", mat[i, j].Intensity);
                }
                Console.WriteLine();
            }
        }
    }
    
}
