using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision.Utils
{
    public delegate float FeatureCompareMetric(float[] features1, float[] features2);
    
    public static class HistogramUtils
    {
        public static FeatureCompareMetric Intersection = (float[] features1, float[] features2) =>
        {
            var normFactor = Math.Min(features1.Sum(), features2.Sum());
            var intersection = Enumerable.Range(0, features1.Length)
                    .Select(i => Math.Min(features1[i], features2[i]))
                    .Sum();

            return intersection / normFactor;
        };
    }
}
