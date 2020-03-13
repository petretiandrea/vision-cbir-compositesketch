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
        //private IDictionary<PhotoMetadata, FaceComponentContainer<float[], float[]>> memoryDb = new Dictionary<PhotoMetadata, FaceComponentContainer<float[], float[]>>();
        private IDictionary<PhotoMetadata, FaceComponentContainer<float[], float[]>> memoryDb = new ConcurrentDictionary<PhotoMetadata, FaceComponentContainer<float[], float[]>>();

        public void AddPhotoFeatures(PhotoMetadata photo, FaceComponentContainer<float[], float[]> features)
        {
            memoryDb[photo] = features;
        }

        public Tuple<PhotoMetadata, FaceComponentContainer<float[], float[]>>[] Entries => memoryDb.Select(e => Tuple.Create(e.Key, e.Value)).ToArray();
        public Tuple<PhotoMetadata, float[]>[] FeaturesEyes(Gender gender) => MapFilter(gender, tuple => tuple.Eyes);
        public Tuple<PhotoMetadata, float[]>[] FeaturesEyebrows(Gender gender) => MapFilter(gender, tuple => tuple.Eyebrows);
        public Tuple<PhotoMetadata, float[]>[] FeaturesNose(Gender gender) => MapFilter(gender, tuple => tuple.Nose);
        public Tuple<PhotoMetadata, float[]>[] FeaturesMouth(Gender gender) => MapFilter(gender, tuple => tuple.Mouth);
        public Tuple<PhotoMetadata, float[]>[] FeaturesHair(Gender gender) => MapFilter(gender, tuple => tuple.Hair);
        public Tuple<PhotoMetadata, float[]>[] FeaturesShape(Gender gender) => MapFilter(gender, tuple => tuple.Shape);

        private Tuple<PhotoMetadata, T>[] MapFilter<T>(Gender gender, Func<FaceComponentContainer<float[], float[]>, T> map)
        {
            var genderFiltered = gender == Gender.UNKNOWN ? memoryDb : memoryDb.Where(tuple => tuple.Key.Gender == Gender.UNKNOWN || tuple.Key.Gender == gender);
            return genderFiltered.Select(tuple => Tuple.Create(tuple.Key, map(tuple.Value))).ToArray();
        }
    }

    public static class FaceFeaturesDBDumper
    {
        private static List<string> columnNames = new List<string> {
            "Id", "Path", "Gender", "FeaturesHair", "FeaturesEyebrows", "FeaturesEyes", "FeaturesNose", "FeaturesMouth", "FeaturesShape"
        };

        public static void SaveCSV(string filename, FaceFeaturesDB face, string delim = ",")
        {
            using (var writer = new StreamWriter(filename))
            {
                writer.WriteLine(string.Join(delim, columnNames));
                foreach (var entry in face.Entries)
                {
                    writer.Write(string.Join(delim,
                        entry.Item1.Id, 
                        Path.GetFileName(entry.Item1.Path),
                        entry.Item1.Gender,
                        Escape(string.Join(",", entry.Item2.Hair)),
                        Escape(string.Join(",", entry.Item2.Eyebrows)),
                        Escape(string.Join(",", entry.Item2.Eyes)),
                        Escape(string.Join(",", entry.Item2.Nose)),
                        Escape(string.Join(",", entry.Item2.Mouth)),
                        Escape(string.Join(",", entry.Item2.Shape))
                    ));
                    writer.WriteLine();
                }
                writer.Flush();
                writer.Close();}
        }
        
        public static FaceFeaturesDB ReadCSV(string filename, string delim = ",")
        {
            var db = new FaceFeaturesDB();
            using (var csvreader = new CsvReader(new StreamReader(filename), true))
            {
                var headers = csvreader.GetFieldHeaders();
                while(csvreader.ReadNextRecord())
                {
                    var id = int.Parse(csvreader[0]);
                    var path = csvreader[1];
                    var gender = (Gender)Enum.Parse(typeof(Gender), csvreader[2]);
                    var features = FaceComponentContainer.Create(
                        ParseEscapedVector(csvreader[3]),
                        ParseEscapedVector(csvreader[4]),
                        ParseEscapedVector(csvreader[5]),
                        ParseEscapedVector(csvreader[6]),
                        ParseEscapedVector(csvreader[7]),
                        ParseEscapedVector(csvreader[8])
                    );
                    db.AddPhotoFeatures(new PhotoMetadata(id, path, gender), features);
                }
            }
            return db;
        }

        private static float[] ParseEscapedVector(string vector)
        {
            return vector.Trim()
                .Replace("\"", "")
                .Split(',')
                .Select(v => float.Parse(v))
                .ToArray();
        }

        private static string Escape(string value)
        {
            return $"\"{value}\"";
        }
    }
}
