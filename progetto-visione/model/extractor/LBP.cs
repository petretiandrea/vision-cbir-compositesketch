using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vision.Utils;

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
                .Aggregate(new List<float>(), (acc, hist) => { acc.AddRange(hist); return acc; })
                .ToArray();
        }

        public static float CalculareSimilarity(float[] features1, float[] features2, int numberOfCell, FeatureCompareMetric metric)
        {
            if (features1.Length != features2.Length) return -1; // TODO launche exception

            var featuresChunckLength = features1.Length / (numberOfCell * numberOfCell);
            return Enumerable.Range(0, (numberOfCell * numberOfCell))
                .Select(cellIndex => new
                {
                    PhotoCellFeature = features1.Skip(cellIndex * featuresChunckLength).Take(featuresChunckLength).ToArray(),
                    SketchCellFeature = features2.Skip(cellIndex * featuresChunckLength).Take(featuresChunckLength).ToArray()
                })
                .Select(chunkFeatures => metric(chunkFeatures.PhotoCellFeature, chunkFeatures.SketchCellFeature))
                .Aggregate(0f, (acc, sim) => acc + sim);
        }
    }
}
