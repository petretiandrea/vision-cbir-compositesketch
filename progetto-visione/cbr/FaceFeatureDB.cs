using LumenWorks.Framework.IO.Csv;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Vision.Model
{
    public class FaceFeaturesDB
    {
        public static FaceFeaturesDB CreateFromDump(string csvFilename)
        {
            return FaceFeaturesDBSerializer.ReadCSV(csvFilename);
        }

        private IDictionary<PhotoMetadata, FaceFeatures> memoryDb = new ConcurrentDictionary<PhotoMetadata, FaceFeatures>();

        public Tuple<PhotoMetadata, FaceFeatures>[] Entries => memoryDb.Select(e => Tuple.Create(e.Key, e.Value)).ToArray();
        public Tuple<PhotoMetadata, double[]>[] FeaturesEyes(Gender gender) => MapFilter(gender, tuple => tuple.Eyes.Features);
        public Tuple<PhotoMetadata, double[]>[] FeaturesEyebrows(Gender gender) => MapFilter(gender, tuple => tuple.Eyebrows.Features);
        public Tuple<PhotoMetadata, double[]>[] FeaturesNose(Gender gender) => MapFilter(gender, tuple => tuple.Nose.Features);
        public Tuple<PhotoMetadata, double[]>[] FeaturesMouth(Gender gender) => MapFilter(gender, tuple => tuple.Mouth.Features);
        public Tuple<PhotoMetadata, double[]>[] FeaturesHair(Gender gender) => MapFilter(gender, tuple => tuple.Hair.Features);
        public Tuple<PhotoMetadata, double[]>[] FeaturesShape(Gender gender) => MapFilter(gender, tuple => tuple.Shape);

        public void AddPhotoFeatures(PhotoMetadata photo, FaceFeatures features)
        {
            memoryDb[photo] = features;
        }

        public bool Dump(string csvFilename)
        {
            try {
                FaceFeaturesDBSerializer.SaveCSV(csvFilename, this);
                return true;
            } catch (Exception) {
                return false;
            }
        }

        private Tuple<PhotoMetadata, double[]>[] MapFilter(Gender gender, Func<FaceFeatures, double[]> map)
        {
            var genderFiltered = gender == Gender.UNKNOWN ? memoryDb : memoryDb.Where(tuple => tuple.Key.Gender == Gender.UNKNOWN || tuple.Key.Gender == gender);
            return genderFiltered.Select(tuple => Tuple.Create(tuple.Key, map(tuple.Value))).ToArray();
        }
        

        private class FaceFeaturesDBSerializer
        {
            private static List<string> columnNames = new List<string> {
                "Id", "Path", "Gender", "FeaturesHair", "FeaturesEyebrows", "FeaturesEyes", "FeaturesNose", "FeaturesMouth", "FeaturesShape"
            };

            public static void SaveCSV(string csvFile, FaceFeaturesDB face, string delim = ",")
            {
                using (var writer = new StreamWriter(csvFile))
                {
                    writer.WriteLine(string.Join(delim, columnNames));
                    foreach (var entry in face.Entries)
                    {
                        var metadata = string.Join(delim, PhotoMetadataCsv.Serializer(entry.Item1, Path.GetFullPath(csvFile)));
                        writer.Write(string.Join(delim,
                            metadata,
                            Escape(string.Join(";", entry.Item2.Hair.Features)),
                            Escape(string.Join(";", entry.Item2.Eyebrows.Features)),
                            Escape(string.Join(";", entry.Item2.Eyes.Features)),
                            Escape(string.Join(";", entry.Item2.Nose.Features)),
                            Escape(string.Join(";", entry.Item2.Mouth.Features)),
                            Escape(string.Join(";", entry.Item2.Shape))
                        ));
                        writer.WriteLine();
                    }
                    writer.Flush();
                    writer.Close();
                }
            }

            public static FaceFeaturesDB ReadCSV(string filename, string delim = ",")
            {
                var db = new FaceFeaturesDB();
                using (var csvreader = new CsvReader(new StreamReader(filename), true))
                {
                    var headers = csvreader.GetFieldHeaders();
                    while (csvreader.ReadNextRecord())
                    {
                        var metdata = PhotoMetadataCsv.Deserializer(filename, csvreader);
                        var features = new FaceFeatures(
                            new ComponentFeature(ParseEscapedVector(csvreader[3]), 0),
                            new ComponentFeature(ParseEscapedVector(csvreader[4]), 0),
                            new ComponentFeature(ParseEscapedVector(csvreader[5]), 0),
                            new ComponentFeature(ParseEscapedVector(csvreader[6]), 0),
                            new ComponentFeature(ParseEscapedVector(csvreader[7]), 0),
                            ParseEscapedVector(csvreader[8])
                        );
                        db.AddPhotoFeatures(metdata, features);
                    }
                }
                return db;
            }

            private static double[] ParseEscapedVector(string vector)
            {
                return vector.Trim()
                    .Replace("\"", "")
                    .Split(';')
                    .Select(v => double.Parse(v))
                    .ToArray();
            }

            private static string Escape(string value)
            {
                return $"\"{value}\"";
            }
        }
    }
}
