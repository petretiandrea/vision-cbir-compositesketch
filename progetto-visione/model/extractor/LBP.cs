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

        double[] HistogramFromLBP(Image<Gray, byte> lbpImage);
    }

    public interface MultiscaleLBP
    {
        Image<Gray, byte>[] Apply<TColor, TDepth>(Image<TColor, TDepth> image)
            where TColor : struct, IColor
            where TDepth : new();

        double[] HistogramFromMLBP(params Image<Gray, byte>[] lbpImages);
    }
}
