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

namespace Vision
{
    
    // Cozzaglia di codice per testare i vari algoritmi TODO: refactor
    class Test1
    {
        static string PHOTO_PATH = @"C:\Users\Petreti Andrea\Desktop\progetto-visione\dataset\cuhk\CUHK_training_photo\photo\";
        static string SKETCH_PATH = @"C:\Users\Petreti Andrea\Desktop\progetto-visione\dataset\cuhk\CUHK_training_sketch\sketch\";

        static string FACE_MODEL = @"C:\Users\Petreti Andrea\Desktop\progetto-visione\haarcascade_frontalface_default.xml";
        static string LANDMARK_MODEL = @"C:\Users\Petreti Andrea\Desktop\progetto-visione\lbfmodel.yaml";

        public void Run()
        {
            //var image = GetPhotoFromSet("f-006-01.jpg");
            var landmark = new FaceLandmarkDetectorLBF(new CascadeFaceDetector(FACE_MODEL), LANDMARK_MODEL);
            var faceNorm = new DefaultFaceComponentsExtractor();

            var photos = Directory.GetFiles(PHOTO_PATH);
            var sketches = Directory.GetFiles(SKETCH_PATH);
            var eyesMean = new Mat(45, 150, Emgu.CV.CvEnum.DepthType.Cv64F, 1);
            eyesMean.SetTo(new MCvScalar(0));

            var sketch = new Image<Bgr, byte>(sketches[1]);
            var normSketch = Preprocessing.PreprocessImage(sketch);
            var sketchComponents = faceNorm.ExtractComponentsFromImage(landmark.DetectFaceComponents(normSketch)).First();
            var sketchHists = ComputeFaceComponentsHists(normSketch, sketchComponents);

            //ImageViewer.Show(normSketch);
            foreach (var img in photos.Take(5).ToList())
            {
                var image = new Image<Bgr, byte>(img);
                var normImage = Preprocessing.PreprocessImage(image);
                ImageViewer.Show(normImage);
                var facesRects = landmark.DetectFaceComponents(normImage);
                facesRects = faceNorm.ExtractComponentsFromImage(facesRects);

                var face = facesRects[0];

                var dict = ComputeFaceComponentsHists(normImage, face);
                CalculateSimilarity(dict, sketchHists);

                /*foreach (var face in facesRects)
                {
                    //DrawFaceComponentsRect(normImage, face);
                    //new Thread(() => ImageViewer.Show(normImage)).Start();
                    
                    //Console.WriteLine("ch: " + eyes.Convert<Gray, double>().Mat.NumberOfChannels);
                    //CvInvoke.Add(eyesMean, eyes.Convert<Gray, double>(), eyesMean);
                }*/
            }

            //eyesMean.ConvertTo(eyesMean, Emgu.CV.CvEnum.DepthType.Cv8U, 1.0 / 5);
            //ImageViewer.Show(eyesMean, "Mean");
        }

        private Image<Bgr, byte> GetPhotoFromSet(string imgName)
        {
            return new Image<Bgr, byte>(PHOTO_PATH + imgName);
        }

        private Image<Bgr, byte> GetSketchFromSet(string imgName)
        {
            return new Image<Bgr, byte>(SKETCH_PATH + imgName);
        }

        private void DrawFaceComponentsRect(Image<Gray, byte> img, FaceComponents face)
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

        public Dictionary<string, DenseHistogram[]> ComputeFaceComponentsHists(Image<Gray, byte> img, FaceComponents components)
        {
            var lbp = new LBP<byte>();
            var dict = new Dictionary<string, DenseHistogram[]>();

            var eyes = img.GetSubRect(components.Eyes);
            dict.Add("EYES", BlockLBPH(lbp, eyes, 5));

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

        public DenseHistogram[] BlockLBPH(LBP<byte> algo, Image<Gray, byte> img, int numberOfPatch)
        {
            // 1. calculate LBP on image
            // 2. compute the patches for image
            // 3. for each patch compute the histogram
            var lbp = algo.ComputeLBP(img, 1);
            var patches = ComputePatches(lbp, numberOfPatch);
            var hists = patches.AsParallel().Select(patch => { // parallelize
                //lbp.Draw(patch, new Gray(0), 1);
                //lbp.GetSubRect(patch);
                var lbpPatch = lbp.GetSubRect(patch);
                return algo.ComputeLBPH(lbpPatch);
            }).ToArray();

            //new Thread(() => ImageViewer.Show(lbp)).Start();

            return hists;
        }

        private Rectangle[] ComputePatches(Image<Gray, byte> img, int k)
        {
            // TODO: compute overlapped patch
            var patches = new Rectangle[k * k];
            var patchSize = new Size(img.Width / k, img.Height / k);
            var overlap = new Size(img.Width % k, img.Height % k);
            for (int i = 0; i < patches.Length; i++)
            {
                var row = i / k;
                var col = i % k;
                var r = new Rectangle(
                    col * patchSize.Width,
                    row * patchSize.Height,
                    patchSize.Width + overlap.Width,
                    patchSize.Height + overlap.Height);
                patches[i] = r;
                //Console.WriteLine(r);
            }
            return patches;
        }

        

        // TODO: allow to serialize and deserialize DenseHistogram as feature vector
        private DenseHistogram CreateHistogramFromData<TDepth>(TDepth[] data)
            where TDepth : new()
        {
            var parsedHist = new DenseHistogram(256, new RangeF(0, 256)); 
            var m = new Matrix<TDepth>(data);
            parsedHist.Calculate(new Matrix<TDepth>[] { m }, false, null);
            return parsedHist;
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
