using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using LA_Back;
using AsarCLR;

namespace win
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        Graphics g;
        readonly GFXHandler gh = new GFXHandler(@"C:\Users\Leuu\source\repos\Lunar Alchemy\TEST\CleanROMs\SMW_U.smc");

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            // Setup
            float pxsize = 14.56f;
            g = e.Graphics;
            g.ScaleTransform(pxsize, pxsize);
            g.FillRectangle(Brushes.Gray, 0, 0, Size.Width, Size.Height);


            byte[]  spr = gh.tempSPR();
            Color[] pal = BuildPal();

            DrawSpr(spr, pal, 16);

        }

        /// <summary>
        /// Draw a sprite
        /// </summary>
        /// <param name="spr">sprite data</param>
        /// <param name="pal">palette data</param>
        /// <param name="w">width of spr</param>
        public void DrawSpr(byte[] spr, Color[] pal, int w)
        {
            for (int i = 0; i < spr.Length; i++)
            {
                Color col = pal[spr[i]];
                SolidBrush brush = new SolidBrush(col);
                g.FillRectangle(brush, i - ((i / w) * w), i / w, 1, 1);
            }
        }

        /// <summary>
        /// Builds Mario's palette
        /// </summary>
        /// <returns></returns>
        public Color[] BuildPal()
        {
            byte[] bPal = new byte[32];
            ushort[] sPal = new ushort[16];
            for (int i = 0; i < 8; i++)
            {
                bPal[i+4] = gh.ROMHandler.ROM[0x3280 + i];
            }
            for (int i = 0; i < 20; i++)
            {
                bPal[i+12] = gh.ROMHandler.ROM[0x32C8 + i];
            }
            for (int i = 0; i < 16; i++)
            {
                try { sPal[i] = BitConverter.ToUInt16(bPal, i * 2); } catch { }
            }
            
            Color[] pal = new Color[sPal.Length];
            
            for (int i = 2; i < sPal.Length; i++)
            {
                uint[] col = gh.Pal2Col(sPal[i]);
                pal[i] = Color.FromArgb((int)col[0], (int)col[1], (int)col[2]);
            }
            pal[0] = Color.Transparent;
            var w = gh.Pal2Col(0x7FDD);
            pal[1] = Color.FromArgb((int)w[0], (int)w[1], (int)w[2]);

            return pal;
        }
    }
}
