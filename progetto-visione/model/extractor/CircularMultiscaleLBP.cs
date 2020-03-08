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
        public static MultiscaleLBP Create(params int[] multiscaleRadius) => new CircularMultiscaleLBP(multiscaleRadius);

        private LBP[] lbps;

        protected CircularMultiscaleLBP(params int[] multiscaleRadius)
        {
            this.lbps = multiscaleRadius.Select(radius => new CircularLBP(radius)).ToArray();
        }
        
        public Image<Gray, byte>[] Apply<TColor, TDepth>(Image<TColor, TDepth> image) where TColor : struct, IColor where TDepth : new()
            => lbps.AsParallel().AsOrdered().Select(lbp => lbp.Apply(image)).ToArray();
    }
}
