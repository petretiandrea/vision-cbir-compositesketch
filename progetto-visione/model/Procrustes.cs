using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision.Model
{
    public static class Procrustes
    {
        public static Matrix<float> OrdinaryAnalysis(PointF[] points1, PointF[] points2)
        {
            var mean1 = new PointF(points1.Select(p => p.X).Sum() / points1.Length,
                points1.Select(p => p.Y).Sum() / points1.Length);

            var mean2 = new PointF(points2.Select(p => p.X).Sum() / points2.Length,
                points2.Select(p => p.Y).Sum() / points2.Length);

            points1 = points1.Select(p => new PointF(p.X - mean1.X, p.Y - mean1.Y)).ToArray();
            points2 = points2.Select(p => new PointF(p.X - mean2.X, p.Y - mean2.Y)).ToArray();

            var s1 = Stddev(points1.SelectMany(p => new float[] { p.X, p.Y }).ToArray());
            var s2 = Stddev(points2.SelectMany(p => new float[] { p.X, p.Y }).ToArray());

            points1 = points1.Select(p => new PointF(p.X / s1, p.Y / s1)).ToArray();
            points2 = points2.Select(p => new PointF(p.X / s2, p.Y / s2)).ToArray();

            var pointsMatrix1 = ToMatrix(points1);
            var pointsMatrix2 = ToMatrix(points2);

            var t = pointsMatrix1.Transpose();
            var product = pointsMatrix1.Transpose() * pointsMatrix2;

            using (var mean1Matrix = new Matrix<float>(new float[,] { { mean1.X, mean1.Y } }))
            using (var mean2Matrix = new Matrix<float>(new float[,] { { mean2.X, mean2.Y } }))
            using (var U = new Matrix<float>(2, 2))
            using (var S = new Matrix<float>(2, 1))
            using (var Vt = new Matrix<float>(2, 2))
            {
                CvInvoke.SVDecomp(product, S, U, Vt, Emgu.CV.CvEnum.SvdFlag.Default);

                var R = (U * Vt).Transpose();
                var scaleRotation = (s2 / s1) * R;
                var translation = mean2Matrix.Transpose() - (s2 / s1) * R * mean1Matrix.Transpose();
                var i = new Matrix<float>(new float[,] { { 0, 0, 1 } });
                var affineMatrix = scaleRotation.ConcateHorizontal(translation).ConcateVertical(i);

                return affineMatrix;
            }
        }

        private static float Stddev(IEnumerable<float> values)
        {
            if (values.Any())
            {
                // Compute the average.     
                var avg = values.Average();
                // Perform the Sum of (value-avg)_2_2.      
                var sum = values.Sum(d => Math.Pow(d - avg, 2));
                // Put it all together.      
                var variance = sum / values.Count();
                return (float)Math.Sqrt(variance);
            }

            return 0;
        }

        private static Matrix<float> ToMatrix(PointF[] points)
        {
            var m = new Matrix<float>(points.Length, 2);
            for (var i = 0; i < points.Length; i++)
            {
                m.GetRow(i).GetCol(0).SetValue(points[i].X);
                m.GetRow(i).GetCol(1).SetValue(points[i].Y);
            }
            return m;
        }
    }
}
