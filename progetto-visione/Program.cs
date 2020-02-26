using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vision.Model;

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
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
            //var cbr = new Model.CBRTest(1, @"C:\Users\Petreti Andrea\Desktop\progetto-visione\dataset\cuhk\CUHK_training_photo\photo\");
            //cbr.Search("f-005-01.jpg");
            NormalizeDataset t = new NormalizeDataset();
            t.Execute();
        }
    }
}
