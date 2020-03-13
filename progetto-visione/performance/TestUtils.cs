using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vision.Model;

namespace Vision.performance
{
    public static class TestUtils
    {
        public static PhotoSketchAlgorithm GetAlgorithm()
        {
            var options = PhotoSketchAlgorithmOptions.Default;
            options.Scales = new int[] { 1 };
            options.EyeBrowsPatches = 10;
            options.HairPatches = 10;
            options.MouthPatches = 10;
            options.NosePatches = 10;
            return new PhotoSketchAlgorithm(options);
        }

        public static double TestSpeed(Action action)
        {
            var start = DateTime.Now;
            action();
            return (DateTime.Now - start).TotalMilliseconds;
        }

        public static class TestSketchMetadataCsv
        {
            public class TestSketchMetadata : PhotoMetadata
            {
                public string Ground { get; set; }

                public TestSketchMetadata(int id, string path, Gender gender, string ground) : base(id, path, gender)
                {
                    Ground = ground ?? throw new ArgumentNullException(nameof(ground));
                }
            }

            public static List<TestSketchMetadata> FromCSV(string csvFile, bool useCsvPath = true)
            {
                using (var reader = new StreamReader(csvFile))
                using (var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.ToLower();
                    var photos = csv.GetRecords<TestSketchMetadata>().ToList();
                    if (useCsvPath)
                    {
                        var csvRoot = Path.GetDirectoryName(csvFile);
                        return photos.Select(p => new TestSketchMetadata(p.Id, Path.Combine(csvRoot, p.Path), p.Gender, p.Ground)).ToList();
                    }
                    return photos;
                }
            }
        }
    }
}
