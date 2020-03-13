using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vision.Model;

namespace Vision.Model.Extractor
{
    public interface LBP
    {
        Image<Gray, byte> Apply<TColor, TDepth>(Image<TColor, TDepth> image)
            where TColor : struct, IColor
            where TDepth : new();
    }

    public interface MultiscaleLBP
    {
        Image<Gray, byte>[] Apply<TColor, TDepth>(Image<TColor, TDepth> image)
            where TColor : struct, IColor
            where TDepth : new();
    }

    public static class LBPUtils
    {
        public static float[] CalculateHistogramFromLBP(Image<Gray, byte> image)
        {
            using (var hist = new DenseHistogram(256, new RangeF(0, 256)))
            {
                hist.Calculate(new Image<Gray, byte>[] { image }, false, null);
                return hist.GetBinValues();
            }
        }

        public static float[] CalculateHistogramFromMLBP(Image<Gray, byte>[] images)
        {
            return images.Select(img => LBPUtils.CalculateHistogramFromLBP(img))
                .SelectMany(hist => hist)
                .ToArray();
        }

        public static double CalculareSimilarity(float[] features1, float[] features2, int numberOfPatch, FeatureCompareMetric metric)
        {
            if (features1.Length != features2.Length) throw new ArgumentException("The features vector must be to same length");

            var featuresChunckLength = features1.Length / (numberOfPatch * numberOfPatch);
            return Enumerable.Range(0, (numberOfPatch * numberOfPatch))
                .AsParallel()
                .Select(cellIndex => new
                {
                    PhotoPatchFeatures = features1.SubArray(cellIndex * featuresChunckLength, featuresChunckLength),
                    SketchPatchFeature = features2.SubArray(cellIndex * featuresChunckLength, featuresChunckLength)
                })
                .Select(chunkFeatures => metric(chunkFeatures.PhotoPatchFeatures, chunkFeatures.SketchPatchFeature))
                .Aggregate(0d, (acc, sim) => acc + sim);
        }

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }
}
