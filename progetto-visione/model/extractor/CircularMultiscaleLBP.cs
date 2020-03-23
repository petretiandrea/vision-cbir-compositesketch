using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vision.Model.Extractor;

namespace Vision.Model.Extractor
{
    public class CircularMultiscaleLBP : MultiscaleLBP
    {
        public static MultiscaleLBP CreateUniform(params float[] multiscaleRadius)
        {
            var lbps = multiscaleRadius.Select(rad => ExtendedLBP.CreateUniform(rad, ExtendedLBP.DEFAULT_NEIGHBORS_FACTOR)).ToArray();
            return new CircularMultiscaleLBP(lbps);
        }

        public static MultiscaleLBP Create(params float[] multiscaleRadius)
        {
            var lbps = multiscaleRadius.Select(rad => ExtendedLBP.Create(rad, ExtendedLBP.DEFAULT_NEIGHBORS_FACTOR)).ToArray();
            return new CircularMultiscaleLBP(lbps);
        }

        private LBP[] lbps;
        
        protected CircularMultiscaleLBP(LBP[] lbps)
        {
            this.lbps = lbps;
        }
        
        public Image<Gray, byte>[] Apply<TColor, TDepth>(Image<TColor, TDepth> image) where TColor : struct, IColor where TDepth : new()
            => lbps.AsParallel().AsOrdered().Select(lbp => lbp.Apply(image)).ToArray();
       
        public double[] HistogramFromMLBP(params Image<Gray, byte>[] lbpImages)
        {
            return lbpImages.Select((img, index) => lbps[index].HistogramFromLBP(img))
               .SelectMany(hist => hist)
               .ToArray();
        }
    }
}
