using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vision.CBR;

namespace Vision.performance
{
    public class TestAccuracy
    {
        private const string FOLDER_PHOTOS = @" ";
        private const string FOLDER_SKETCHS = @" ";

        private static int[] RANKS = new int[] { 5, 10, 20, 50 };

        public void Run()
        {
            var cbr = new PhotoSketchCBR();
            
            
        }

    }
}
