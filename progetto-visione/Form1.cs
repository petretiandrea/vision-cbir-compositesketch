using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;

namespace Vision
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            RandomFullList();
        }
        static string PATH1 = @"C:\Users\Petreti Andrea\Desktop\progetto-visione\dataset\cuhk\CUHK_training_photo\photo\";

        private void RandomFullList()
        {
            
            var paths1 = Directory.GetFiles(PATH1);

            for(int i = 0; i < paths1.Length; i++)
            {
                imageGallery.AddImage(paths1[i]);
            }
        }
    }
}
