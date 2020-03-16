using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vision.Model;

namespace Vision.performance
{
    public static class TestUtils
    {
        public static PhotoSketchAlgorithm GetAlgorithm()
        {
            var options = PhotoSketchAlgorithmOptions.Default;
            options.Scales = new float[] { 1, 3, 5, 7 };
            return new PhotoSketchAlgorithm(options);
        }

        public static double TestSpeed(Action action)
        {
            var start = DateTime.Now;
            action();
            return (DateTime.Now - start).TotalMilliseconds;
        }
    }
}
