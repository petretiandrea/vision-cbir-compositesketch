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
        public static MultiscaleLBP Create(params float[] multiscaleRadius) => new CircularMultiscaleLBP(multiscaleRadius.Select(r => Tuple.Create(r, CircularLBP.DEFAULT_NEIGHBORS_FACTOR)).ToArray());
        public static MultiscaleLBP Create(params Tuple<float, int>[] radiusNeighborPairs) => new CircularMultiscaleLBP(radiusNeighborPairs);

        private LBP[] lbps;

        protected CircularMultiscaleLBP(Tuple<float, int>[] radiusNeighborPairs)
        {
            this.lbps = radiusNeighborPairs.Select(radiusNeigh => CircularLBP.Create(radiusNeigh.Item1, radiusNeigh.Item2)).ToArray();
        }
        
        public Image<Gray, byte>[] Apply<TColor, TDepth>(Image<TColor, TDepth> image) where TColor : struct, IColor where TDepth : new()
            => lbps.AsParallel().AsOrdered().Select(lbp => lbp.Apply(image)).ToArray();
    }
}
