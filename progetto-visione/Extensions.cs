using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Vision.Model;

namespace Vision
{
    public static class Extensions
    {
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }

    public static class MatrixExtensions
    {
        public static void SetCellValue<TDepth>(this Matrix<TDepth> m, int row, int col, double value) where TDepth : new()
        {
            m.GetRow(row).GetCol(col).SetValue(value);
        }

        public static TDepth GetCellValue<TDepth>(this Matrix<TDepth> m, int row, int col) where TDepth : new()
        {
            return m.Data[row, col];
        }

        public static double ValueOrDefault<TDepth>(this Image<Gray, TDepth> img, int row, int col, double defaultValue = 0)
            where TDepth : new()
        {
            if (row >= 0 && row < img.Rows && col >= 0 && col < img.Cols)
            {
                return img[row, col].Intensity;
            }
            return defaultValue;
        }
    }

    public static class RankExtension {
        public static Rank<T,S> ToRank<T, S>(this IEnumerable<Tuple<T, S>> enumarable)
        {
            return Rank.Create(enumarable);
        }
    }

    
}
