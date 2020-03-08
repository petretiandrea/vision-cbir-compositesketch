using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Vision.Model.Extractor
{
    public class CircularLBP : LBP
    {
        private const int DEFAULT_NEIGHBORS_FACTOR = 8;
        public float Radius { get; private set; }
        public int Neighbors { get; private set; }

        public CircularLBP(float radius, int neighbors = -1)
        {
            this.Radius = radius;
            this.Neighbors = NormalizeNeighborPoints(radius, neighbors);
        }

        public Image<Gray, byte> Apply<TColor, TDepth>(Image<TColor, TDepth> image)
            where TColor : struct, IColor
            where TDepth : new()
        {
            using (var imgDouble = image.Convert<Gray, double>()) // convert to image calculable
            using (var lbp = new Matrix<int>(image.Rows, image.Cols)) // lbp final image
            {
                var locations = GetSamplingPointLocation(Radius, Neighbors);
                var rowLocation = locations.Item1;
                var colLocation = locations.Item2;

                // apply lbp filter to all image
                for (int row = 0; row < image.Rows; row++)
                {
                    for (int col = 0; col < image.Cols; col++)
                    {
                        var pattern = 0;
                        foreach (var n in Enumerable.Range(0, Neighbors))
                        {
                            // make bi-linear interpolation for neighbor point that not match to matrix grid
                            var interPixel = BilinearInterpolation(imgDouble, row + rowLocation[n], col + colLocation[n]);
                            pattern |= SignFunction(interPixel - imgDouble[row, col].Intensity) << n;
                        }
                        lbp[row, col] = pattern;
                    }
                }
                return lbp.Mat.ToImage<Gray, byte>();
            }
        }

        private double BilinearInterpolation(Image<Gray, double> mat, double rowP, double colP)
        {
            var minRow = (int)Math.Floor(rowP);
            var minCol = (int)Math.Floor(colP);
            var maxRow = (int)Math.Ceiling(rowP);
            var maxCol = (int)Math.Ceiling(colP);

            var dRow = rowP - minRow;
            var dCol = colP - minCol;

            // 4 near cell
            var topLeft = mat.ValueOrDefault(minRow, minCol, 0);
            var topRight = mat.ValueOrDefault(minRow, maxCol, 0);
            var bottomLeft = mat.ValueOrDefault(maxRow, minCol, 0);
            var bottomRight = mat.ValueOrDefault(maxRow, maxCol, 0);

            // weigths
            var w1 = (1 - dRow) * (1 - dCol);
            var w2 = (1 - dRow) * dCol;
            var w3 = dRow * (1 - dCol);
            var w4 = dRow * dCol;

            return w1 * topLeft + w2 * topRight + w3 * bottomLeft + w4 * bottomRight;
        }

        private static int SignFunction(double v)
        {
            return v >= 0 ? 1 : 0;
        }

        private Tuple<double[], double[]> GetSamplingPointLocation(float radius, int points)
        {
            var rowCirc = Enumerable.Range(0, points).Select(n => -radius * Math.Sin(2 * Math.PI * n / (float)points)).Select(row => Math.Round(row, 5)).ToArray();
            var colCirc = Enumerable.Range(0, points).Select(n => radius * Math.Cos(2 * Math.PI * n / (float)points)).Select(col => Math.Round(col, 5)).ToArray();
            return Tuple.Create(rowCirc, colCirc);
        }

        private int NormalizeNeighborPoints(float radius, int neighbors)
        {
            return (neighbors > 0 && neighbors < 33) ? neighbors : DEFAULT_NEIGHBORS_FACTOR * (int)radius;
        }
    }
}
