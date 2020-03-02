using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Vision
{
    public static class ImageExtension
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
}
