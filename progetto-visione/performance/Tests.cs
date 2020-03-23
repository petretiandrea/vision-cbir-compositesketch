using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vision.CBR;
using Vision.Model;
using LumenWorks.Framework.IO.Csv;
using static Vision.performance.TestUtils;
using System.Collections.Concurrent;
using Vision.Normalization;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;

namespace Vision.performance
{
    // Contains all tested performance of CBIR
    public static class Tests
    {
        //private static string GALLERY_CSV = @"C:\Users\Petreti Andrea\Desktop\progetto-visione\dataset\photo_gallery\photo_gallery_ar_meds.csv";
        private static string GALLERY_CSV = @"C:\Users\Petreti Andrea\Desktop\progetto-visione\dataset\photo_gallery\photo_gallery_full.csv";
        private static string SKETCHS_CSV = @"C:\Users\Petreti Andrea\Desktop\progetto-visione\dataset\sketches\epirip\dataset.csv";
        private static string DUMPED_DB = @"C:\Users\Petreti Andrea\Desktop\progetto-visione\dataset\photo_gallery\GalleryFeatures-Full.csv";

        private static int[] RANKS = Enumerable.Range(1, 200).ToArray();

        static void Main()
        {
            var photos = PhotoMetadataCsv.FromCSV(GALLERY_CSV).ToList();
            var sketches = PhotoMetadataCsv.FromCSV(SKETCHS_CSV).ToList();

            var extractor = TestUtils.GetPhotoSketchFeatureExtractor(Params.GetReferenceShape());
            // extract or load from dumped db
            //var db = ExtractFeaturesDB(extractor, true);
            var db = FaceFeaturesDB.CreateFromDump(DUMPED_DB);
            var cbir = new PhotoSketchCBIR(extractor) { Database = db };

            TestSingleComponents(cbir, sketches);
            TestDifferentFusion(cbir, sketches);
            TestBestCBR(cbir, sketches);
        }

        public static FaceFeaturesDB ExtractFeaturesDB(PhotoSketchFeatureExtractor extractor, List<PhotoMetadata> photos, bool savedb = true)
        {
            var dbName = string.Format("features-{0}.csv", DateTimeOffset.Now.ToUnixTimeMilliseconds());
            var db = new FaceFeaturesDB();
            Parallel.ForEach(photos, photo =>
            {
                try {
                    var feature = extractor.ExtractFaceFeatures(photo.AbsolutePath);
                    db.AddPhotoFeatures(photo, feature);
                } catch (ArgumentException) { }
            });
            
            if(savedb)
            {
                db.Dump(dbName);
                Console.WriteLine("Features db saved on: {0}", dbName);
            }

            return db;
        }

        // execute the test that creates CMC for each component
        private static void TestSingleComponents(PhotoSketchCBIR cbir, List<PhotoMetadata> sketches)
        {
            var ranks = CMCPerComponent(cbir, sketches);
            var accuraciesNoGender = ranks[0];
            var accuraciesWithGender = ranks[1];

            // for each component
            for (int i = 0; i < 6; i++)
            {
                SerializeRecognitionCMC(string.Format("rank_performance_{0}.csv", i),
                    accuraciesNoGender[i],
                    accuraciesWithGender[i]);
            }
        }

        // execute the test that creates CMC combining the most discriminative components
        private static void TestDifferentFusion(PhotoSketchCBIR cbir, List<PhotoMetadata> sketches)
        {
            var bestFusions = new double[5][]
            {
                new double[] {0, 1, 1, 0, 0, 0 }, // eyebrows and eyes
                new double[] {0, 1, 1, 0, 1, 0 }, // eyebrows, eyes and mouth
                new double[] {0, 1, 1, 0, 1, 1 }, // eyebrows, eyes, mouth and shape
                new double[] {1, 1, 1, 0, 1, 1 }, // hair, eyebrows, eyes, mouth and shape
                new double[] {1, 1, 1, 1, 1, 1 }  // all components
            };
            
            for (int f = 0; f < bestFusions.Length; f++)
            {
                cbir.SearchFusionStrategy = new WeightedSum(bestFusions[f]);
                var cmcData = CreateRecognitionCMC(cbir, sketches, false);
                var cmcDataGender = CreateRecognitionCMC(cbir, sketches, true);

                SerializeRecognitionCMC(string.Format("rank_performance_fusion_{0}.csv", f),
                    cmcData,
                    cmcDataGender);
            }
        }
        
        // execute a test for eveluate the performance of cbr using the best features fusion
        private static void TestBestCBR(PhotoSketchCBIR cbir, List<PhotoMetadata> sketches)
        {
            cbir.SearchFusionStrategy = new WeightedSum(1, 1, 1, 0, 1, 1); // hair, eyebrows, eyes, mouth and shape

            var accuraciesNoGender = CreateRecognitionCMC(cbir, sketches, false);
            var accuraciesWithGender = CreateRecognitionCMC(cbir, sketches, true);

            SerializeRecognitionCMC(string.Format("rank_best_cbr.csv"), accuraciesNoGender, accuraciesWithGender);
        }

        private static double[] CreateRecognitionCMC(PhotoSketchCBIR cbr, List<PhotoMetadata> sketches, bool filterGender = true)
        {
            var maxRank = RANKS.Max();

            var accuracies = new double[RANKS.Length];
            sketches.ForEach(sketch =>
            {
                try
                {
                    var ranks = cbr.Search(sketch.AbsolutePath, filterGender ? sketch.Gender : Gender.UNKNOWN, maxRank);
                    for (int i = 0; i < RANKS.Length; i++)
                    {
                        accuracies[i] += ranks.Take(RANKS[i]).Any(t => t.Item1.Id == sketch.Id) ? 1 : 0;
                    }
                }
                catch (Exception) { }
            });

            return accuracies.Select(v => v / sketches.Count * 100d).ToArray();
        }

        private static List<double[][]> CMCPerComponent(PhotoSketchCBIR cbr, List<PhotoMetadata> sketches)
        {
            // matrix of accuracies for each component: hair, eyebrows, eye, nose, mouth and shape
            var accuraciesNoGender = new double[6][];
            var accuraciesWithGender = new double[6][];
            
            foreach (var sketch in sketches)
            {
                try
                {
                    var ranksNoGender = cbr.ComputeComponentRanks(sketch.AbsolutePath, Gender.UNKNOWN);
                    var ranksWithGender = cbr.ComputeComponentRanks(sketch.AbsolutePath, sketch.Gender);
                    for (int i = 0; i < RANKS.Length; i++)
                    {
                        for(int j = 0; j < 6; i++)
                        {
                            accuraciesNoGender[j][i] += ranksNoGender[j].Take(RANKS[i]).Any(t => t.Item1.Id == sketch.Id) ? 1 : 0;
                            accuraciesWithGender[j][i] += ranksWithGender[j].Take(RANKS[i]).Any(t => t.Item1.Id == sketch.Id) ? 1 : 0;
                        }
                    }
                }
                catch (ArgumentException e) { }
            }
            return new List<double[][]> {
                accuraciesNoGender.Select(values => values.Select(v => v / sketches.Count * 100d).ToArray()).ToArray(),
                accuraciesWithGender.Select(values => values.Select(v => v / sketches.Count * 100d).ToArray()).ToArray()
            };
        }

        private static void SerializeRecognitionCMC(string name, double[] accuraciesNoGender, double[] accuraciesWithGender)
        {
            using (var csv = new StreamWriter(name))
            {
                csv.WriteLine("Rank,Accuracy,AccuracyGender");
                foreach (var index in Enumerable.Range(0, RANKS.Length))
                {
                    var record = string.Format(new CultureInfo("en-US"), "{0},{1},{2}",
                        RANKS[index],
                        accuraciesNoGender[index],
                        accuraciesWithGender[index]);

                    csv.WriteLine(record);
                }
            }
        }

        // Compute the average image
        private static void GlobalAvgImage()
        {
            var photos = PhotoMetadataCsv.FromCSV(GALLERY_CSV).ToList();
            var sketches = PhotoMetadataCsv.FromCSV(SKETCHS_CSV).ToList();

            var boundingParams = Params.GetComponentBoundingBoxParams();
            var alignerExtractor = ComponentAlignerFactory.FromReferenceShape(boundingParams, Params.GetReferenceShape());

            // compute average images
            //var images = new List<Image<Gray, byte>>();
            var photosComponents = new List<FaceComponentContainer>();
            foreach (var photo in sketches)
            {
                try
                {
                    var img = Normalization.Preprocessing.PreprocessImage(photo.AbsolutePath);
                    //images.Add(img);
                    var container = alignerExtractor.ExtractComponentsFromImage(img);
                    photosComponents.Add(container);
                }
                catch (IndexOutOfRangeException e) { }
                catch (ArgumentException e) { }
            }

            var hair = photosComponents.Select(c => c.Hair).ToArray();
            var eyes = photosComponents.Select(c => c.Eyes).ToArray();
            var eyebrows = photosComponents.Select(c => c.Eyebrows).ToArray();
            var nose = photosComponents.Select(c => c.Nose).ToArray();
            var mouth = photosComponents.Select(c => c.Mouth).ToArray();
            ImageViewer.Show(ImageUtils.CreateAvgImage(160, 80, hair));
            ImageViewer.Show(ImageUtils.CreateAvgImage(144, 29, eyebrows));
            ImageViewer.Show(ImageUtils.CreateAvgImage(144, 29, eyes));
            ImageViewer.Show(ImageUtils.CreateAvgImage(96, 96, nose));
            ImageViewer.Show(ImageUtils.CreateAvgImage(160, 80, mouth));
            //ImageViewer.Show(ImageUtils.CreateAvgImage(200, 250, images));
        }
        
    }
}
