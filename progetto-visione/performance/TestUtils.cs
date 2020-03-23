using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vision.Model;
using Vision.Normalization;

namespace Vision.performance
{
    public static class TestUtils
    {
        public static PhotoSketchFeatureExtractor GetPhotoSketchFeatureExtractor(PointF[][] referenceShape)
        {
            var boundingBoxParams = Params.GetComponentBoundingBoxParams();
            var blockParams = Params.GetComponentBlockParams();
            var detector = ComponentAlignerFactory.FromReferenceShape(boundingBoxParams, referenceShape);

            return PhotoSketchFeatureExtractorFactory.Default(detector, blockParams);
        }

        public static double TestSpeed(Action action)
        {
            var start = DateTime.Now;
            action();
            return (DateTime.Now - start).TotalMilliseconds;
        }
    }
}
