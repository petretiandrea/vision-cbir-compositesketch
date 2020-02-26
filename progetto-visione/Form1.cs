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
            tableLayoutPanel1.GrowStyle = TableLayoutPanelGrowStyle.AddRows;
            var imgWidth = tableLayoutPanel1.Size.Width / tableLayoutPanel1.ColumnCount;
            var imgHeight = tableLayoutPanel1.Size.Height / 2;

            var paths1 = Directory.GetFiles(PATH1);

            for(int i = 0; i < paths1.Length; i++)
            {
                var box = new ImageBox();
                box.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                box.Height = imgHeight;
                box.Width = imgWidth;
                box.SizeMode = PictureBoxSizeMode.Zoom;
                box.FunctionalMode = ImageBox.FunctionalModeOption.Minimum;
                box.Image = new Image<Bgr, float>(paths1[i]);
                tableLayoutPanel1.Controls.Add(box);
            }
        }
    }
}
