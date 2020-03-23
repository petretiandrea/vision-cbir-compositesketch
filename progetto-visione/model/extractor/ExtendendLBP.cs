using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Vision.Model.Extractor
{
    public enum Method
    {
        DEFAULT,
        UNIFORM
    }

    public class ExtendedLBP : LBP
    {
        public const int DEFAULT_NEIGHBORS_FACTOR = 8;
        public float Radius { get; private set; }
        public int Neighbors { get; private set; }
        public Method Method { get; private set; }
        private int[] uniformLookup;

        public static ExtendedLBP Create(float radius) => new ExtendedLBP(radius, DEFAULT_NEIGHBORS_FACTOR * (int)radius);
        public static ExtendedLBP Create(float radius, int neighbors) => new ExtendedLBP(radius, neighbors);
        public static ExtendedLBP CreateUniform(float radius, int neighbors) => new ExtendedLBP(radius, neighbors, Method.UNIFORM);

        protected ExtendedLBP(float radius, int neighbors, Method method = default(Method))
        {
            this.Radius = radius;
            this.Neighbors = neighbors <= 32 ? neighbors : throw new ArgumentException("Max supported neighbors is 32");
            this.Method = method;

            if(this.Method == Method.UNIFORM)
            {
                ComputeLookupTable();
            }
        }

        private void ComputeLookupTable()
        {
            // create lookup table of 2^P elements
            //uniformLookup = new int[(int)Math.Pow(2, Neighbors)];
            uniformLookup = new int[256] {
                0,1,2,3,4,58,5,6,7,58,58,58,8,58,9,10,11,58,58,58,58,58,58,58,12,58,58,58,13,58,
                14,15,16,58,58,58,58,58,58,58,58,58,58,58,58,58,58,58,17,58,58,58,58,58,58,58,18,
                58,58,58,19,58,20,21,22,58,58,58,58,58,58,58,58,58,58,58,58,58,58,58,58,58,58,58,
                58,58,58,58,58,58,58,58,58,58,58,58,23,58,58,58,58,58,58,58,58,58,58,58,58,58,
                58,58,24,58,58,58,58,58,58,58,25,58,58,58,26,58,27,28,29,30,58,31,58,58,58,32,58,
                58,58,58,58,58,58,33,58,58,58,58,58,58,58,58,58,58,58,58,58,58,58,34,58,58,58,58,
                58,58,58,58,58,58,58,58,58,58,58,58,58,58,58,58,58,58,58,58,58,58,58,58,58,58,
                58,35,36,37,58,38,58,58,58,39,58,58,58,58,58,58,58,40,58,58,58,58,58,58,58,58,58,
                58,58,58,58,58,58,41,42,43,58,44,58,58,58,45,58,58,58,58,58,58,58,46,47,48,58,49,
                58,58,58,50,51,52,58,53,54,55,56,57
            };
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
                            var binaryValue = SignFunction(interPixel - imgDouble[row, col].Intensity);
                            pattern |= binaryValue << n;
                        }
                        lbp[row, col] = Method == Method.UNIFORM ? uniformLookup[pattern] : pattern;
                    }
                }
                return lbp.Mat.ToImage<Gray, byte>();
            }
        }

        public double[] HistogramFromLBP(Image<Gray, byte> lbpImage)
        {
            var bins = (Method == Method.UNIFORM) ? 
                Neighbors * (Neighbors - 1) + 3 : // P (P - 1) + 3 pattern
                (int)Math.Pow(2, Neighbors);
                
            using (var hist = new DenseHistogram(bins, new RangeF(0, bins))) // real range is to bins - 1, but its exclusive
            {
                hist.Calculate(new Image<Gray, byte>[] { lbpImage }, false, null);
                return hist.GetBinValues().Select(v => (double)v).ToArray();
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
            var topLeft = mat. ValueOrDefault(minRow, minCol, 0);
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
    }
}
