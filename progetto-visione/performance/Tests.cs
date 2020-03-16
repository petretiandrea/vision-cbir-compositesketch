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
        private static string GALLERY_CSV = @"C:\Users\Petreti Andrea\Desktop\progetto-visione\dataset\photo_gallery\photo_gallery_ar_meds.csv";
        private static string SKETCHS_CSV = @"C:\Users\Petreti Andrea\Desktop\progetto-visione\dataset\sketches\epirip\dataset.csv";
        private static string TEST_DB = @"C:\Users\Petreti Andrea\Desktop\progetto-visione\dataset\photo_gallery\features-1584388093062.csv";

        private static int[] RANKS = Enumerable.Range(1, 100).ToArray();

        public static PhotoSketchAlgorithm Algorithm = TestUtils.GetAlgorithm();

        public static void ExtractFeaturesDB()
        {
            var db = new FaceFeaturesDB();
            var photos = PhotoMetadataCsv.FromCSV(GALLERY_CSV).ToList();

            Console.WriteLine("Extracting Features....");
        
           
            photos.ForEach(photo =>
            {
                try
                {
                    var feature = Algorithm.ExtractFaceFeatures(photo.AbsolutePath);
                    db.AddPhotoFeatures(photo, feature);
                }
                catch (ArgumentException) { }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });

            var dbName = string.Format("features-{0}.csv", DateTimeOffset.Now.ToUnixTimeMilliseconds());
            if(db.Dump(dbName)) {
                 Console.WriteLine("Feature Extracted! Saved on: {0}", dbName);
            }

            TestAccuracy(db);
        }

        public static void TestAccuracy(FaceFeaturesDB featuresDb = null)
        {
            var sketches = PhotoMetadataCsv.FromCSV(SKETCHS_CSV).ToList();
            var gallery = featuresDb == null ? FaceFeaturesDB.FromDump(TEST_DB) : featuresDb;
            var gallerySize = gallery.Entries.Length;
            
            var cbr = new PhotoSketchCBR(Algorithm);

            cbr.Database = gallery;
            
            var accuracies = new double[RANKS.Length];
            var maxRank = RANKS.Max();

            /*var speed = TestSpeed(() =>
            {
                sketches.Take(5).ToList().ForEach(sketch =>
                {
                    var rank = cbr.Search(sketch.AbsolutePath, sketch.Gender, maxRank);
                });
            });*/

            var any = gallery.Entries.Any(t => t.Item1.Id == sketches[0].Id);

             foreach (var sketch in sketches)
            {
                try
                {
                    var rank = cbr.Search(sketch.AbsolutePath, Gender.UNKNOWN/*sketch.Gender*/, maxRank);
                    for (int i = 0; i < RANKS.Length; i++)
                    {
                        accuracies[i] += rank.Take(RANKS[i]).Any(t => t.Item1.Id == sketch.Id) ? 1 : 0;
                    }
                } catch(ArgumentException e){ }
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
            ExtractFeaturesDB();
            //TestAccuracy();
        }

        private static void TestSingleComponents()
        {
           /* var sketches = PhotoMetadataCsv.FromCSV(SKETCHS_CSV).ToList();
            var gallery = FaceFeaturesDB.FromDump(TEST_DB);
            var gallerySize = gallery.Entries.Length;

            var cbr = new PhotoSketchCBR(Algorithm);

            cbr.Database = gallery;

            var accuracies = new double[6, RANKS.Length];
            var maxRank = RANKS.Max();

            foreach (var sketch in sketches)
            {
                try
                {
                    var rank = cbr.Search(sketch.AbsolutePath, Gender.UNKNOWN, maxRank);
                    for (int i = 0; i < RANKS.Length; i++)
                    {
                        accuracies[i] += rank.Take(RANKS[i]).Any(t => t.Item1.Id == sketch.Id) ? 1 : 0;
                    }
                }
                catch (ArgumentException e) { }
            }*/
        }
    }
}
