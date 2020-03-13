using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vision.CBR;
using Vision.Model;
using Vision.UI;

namespace Vision
{
    static class Program
    {
        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var cbr = new PhotoSketchCBR(PhotoSketchAlgorithm.Default);
            var view = new DashboardCBR();
            var controller = new PhotoSketchCBRPresenter(view, cbr);
            
            Application.Run(view);

            //FaceFeaturesDB db = new FaceFeaturesDB();
            //FaceFeaturesDB.SaveCSV(db, "ciao.csv");
        }
    }
}
