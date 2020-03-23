using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vision.CBR;
using Vision.Model;
using Vision.Normalization;
using Vision.UI;

namespace Vision
{
    static class MainCBIR
    {
        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var featureBlockParams = Params.GetComponentBlockParams();
            var boundingBoxParams = Params.GetComponentBoundingBoxParams();
            var referenceShape = Params.GetReferenceShape();
            var componentExtractor = ComponentAlignerFactory.FromReferenceShape(boundingBoxParams, referenceShape);

            var cbr = new PhotoSketchCBIR(PhotoSketchFeatureExtractorFactory.Default(componentExtractor, featureBlockParams));
            var view = new FormDashboardCBIR();
            var controller = new PresenterCBIR(view, cbr);
            
            Application.Run(view);
        }
    }
}
