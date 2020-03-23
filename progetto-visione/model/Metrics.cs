using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vision.Model;

namespace Vision.Model
{
    public delegate double FeatureCompareMetric(double[] features1, double[] features2);
    
    public static class Metrics
    {
        public static FeatureCompareMetric Intersection = (double[] features1, double[] features2) =>
        {
            var normFactor = Math.Min(features1.Sum(), features2.Sum());
            var intersection = Enumerable.Range(0, features1.Length)
                    .Select(i => Math.Min(features1[i], features2[i]))
                    .Sum();

            return intersection / normFactor;
        };

        public static FeatureCompareMetric CosineDistance = (double[] features1, double[] features2) =>
        {
            var f1f2 = features1.Zip(features2, (f1, f2) => f1 * f2).Sum();
            var f1f1 = features1.Select(f1 => f1 * f1).Sum();
            var f2f2 = features2.Select(f2 => f2 * f2).Sum();
            var s = f1f2 / (Math.Sqrt(f1f1) * Math.Sqrt(f2f2));
            return 1 - s;
        };
    }

    public static class ScoreNormalization
    {
        public static Func<double[], Func<double, double>> Tanh => (scores) =>
        {
            using (var matrix = new Matrix<double>(scores))
            {
                var stddev = new MCvScalar();
                var mean = new MCvScalar();
                CvInvoke.MeanStdDev(matrix, ref mean, ref stddev);
                return score => 0.5 * (Math.Tanh(0.01 * ((score - mean.V0) / stddev.V0)) + 1);
            }
        };
    }
}
