using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vision.Model;
using Vision.Normalization;
using Vision.UI;

namespace Vision
{
    static class MainFeatureExtraction
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
            var featureExtractor = PhotoSketchFeatureExtractorFactory.Default(componentExtractor, featureBlockParams);

            var form = new FormGalleryExtraction(featureExtractor);
            
            Application.Run(form);
        }
    }
}
