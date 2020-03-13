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

namespace Vision.performance
{
    public static class Tests
    {
        private static string GALLERY_CSV = @"C:\Users\Petreti Andrea\Desktop\progetto-visione\dataset\photo_gallery.csv";
        private static string SKETCHS_CSV = @"C:\Users\Petreti Andrea\Desktop\progetto-visione\dataset\cuhk\cuhk_sketches\dataset.csv";
        private static string TEST_DB = @"db3.csv";

        private static int[] RANKS = Enumerable.Range(1, 100).ToArray();

        public static PhotoSketchAlgorithm Algorithm = TestUtils.GetAlgorithm();

        public static void ExtractFeaturesDB()
        {
            var db = new FaceFeaturesDB();
            var photos = PhotoMetadataCsv.FromCSV(GALLERY_CSV).ToList();
            var sketches = TestSketchMetadataCsv.FromCSV(SKETCHS_CSV).ToList();

            Console.WriteLine("Extracting Features....");

            photos.ForEach(photo =>
            {
                try
                {
                    var feature = Algorithm.ExtractFaceFeatures(photo.Path);
                    db.AddPhotoFeatures(photo, feature);
                }
                catch (ArgumentException) { }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });

            var dbName = string.Format("features-{0}.csv", DateTimeOffset.Now.ToUnixTimeMilliseconds());
            FaceFeaturesDBDumper.SaveCSV(dbName, db);
            Console.WriteLine("Feature Extracted! Saved on: {0}", dbName);
        }

        public static void TestAccuracy()
        {
            var sketches = TestSketchMetadataCsv.FromCSV(SKETCHS_CSV).ToList();
            var gallery = FaceFeaturesDBDumper.ReadCSV(TEST_DB);
            var gallerySize = gallery.Entries.Length;
            
            var cbr = new PhotoSketchCBR(Algorithm);

            cbr.Database = gallery;
            
            var accuracies = new double[RANKS.Length];
            var maxRank = RANKS.Max();

            var speed = TestSpeed(() =>
            {
                sketches.Take(5).ToList().ForEach(sketch =>
                {
                    var rank = cbr.Search(sketch.Path, sketch.Gender, maxRank);
                });
            });

            foreach (var sketch in sketches)
            {
                var rank = cbr.Search(sketch.Path, sketch.Gender, maxRank);
                for(int i = 0; i < RANKS.Length; i++)
                {
                    accuracies[i] += rank.Take(RANKS[i]).Any(t => t.Item1.Path == sketch.Ground) ? 1 : 0;
                }
            }

            using(var csv = new StreamWriter("rank_performance.csv"))
            {
                csv.WriteLine("Rank,Accuracy,GallerySize");
                foreach (var index in Enumerable.Range(0, RANKS.Length))
                {
                    var accuracy = accuracies[index] / sketches.Count * 100d;
                    csv.WriteLine(string.Format(new CultureInfo("en-US"), "{0},{1},{2}", RANKS[index], accuracy, gallerySize));
                    Console.WriteLine("Accuracy for Rank-{0}: {1}%", RANKS[index], accuracy);
                }
                csv.Close();
            }
            
        }
        
        static void Main()
        {
            //ExtractFeaturesDB();
            TestAccuracy();
        }
    }
}
