using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Vision.Model
{
    using FeatureEntry = Tuple<Gender, float[], float[], float[], float[], float[], float[]>;

    public static class FaceFeatureDB
    {
        public static void CreateFromCSV(string csvFile)
        {

        }

        public static void SaveOnCSV(FaceFeaturesDB db, string csvFile)
        {

        }
    }
    public class FaceFeaturesDB
    {
        private Dictionary<string, FeatureEntry> memoryDb = new Dictionary<string, FeatureEntry>();

        public void AddPhoto(string photoPath, Gender gender, float[] eyes, float[] eyebrows, float[] nose, float[] mouth, float[] hair, float[] shape)
        {
            memoryDb[photoPath] = Tuple.Create(gender, eyes, eyebrows, nose, mouth, hair, shape);
        }

        public Tuple<string, float[]>[] FeaturesEyes(Gender gender) => MapFilter(gender, tuple => tuple.Item2);
        public Tuple<string, float[]>[] FeaturesEyebrows(Gender gender) => MapFilter(gender, tuple => tuple.Item3);
        public Tuple<string, float[]>[] FeaturesNose(Gender gender) => MapFilter(gender, tuple => tuple.Item4);
        public Tuple<string, float[]>[] FeaturesMouth(Gender gender) => MapFilter(gender, tuple => tuple.Item5);
        public Tuple<string, float[]>[] FeaturesHair(Gender gender) => MapFilter(gender, tuple => tuple.Item6);
        public Tuple<string, float[]>[] FeatureShape(Gender gender) => MapFilter(gender, tuple => tuple.Item7);

        private Tuple<string, T>[] MapFilter<T>(Gender gender, Func<FeatureEntry, T> map)
        {
            var genderFiltered = gender == Gender.UNKOWN ? memoryDb : memoryDb.Where(tuple => tuple.Value.Item1 == Gender.UNKOWN || tuple.Value.Item1 == gender);
            
            return genderFiltered.Select(tuple => Tuple.Create(tuple.Key, map(tuple.Value))).ToArray();
        }
    }
}
