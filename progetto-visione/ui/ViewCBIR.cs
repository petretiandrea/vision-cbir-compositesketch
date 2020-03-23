using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vision.Model;

namespace Vision.UI
{
    public interface CBRView
    {
        event EventHandler OnSearchClick;
        event EventHandler OnLoadDbClick;
        string SketchPath { get; }
        Gender SketchGender { get; }
        string GalleryPath { get; }

        int RankSize { get; set; }
        double WeightHair { get; set; }
        double WeightEyebrows { get; set; }
        double WeightEyes { get; set; }
        double WeightNose { get; set; }
        double WeightMouth { get; set; }
        double WeightShape { get; set; }

        void ShowPhotoResults(string[] images, string[] labels);
        bool BackgroundLoading { get; set; }
        string LoadingLabel { get; set; }
    }
}
