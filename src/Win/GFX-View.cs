﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using MOLE_Back;
using MOLE_Back.Libs;

namespace win
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        Graphics g;
        readonly GFXHandler gh = new GFXHandler(Program.args[0]);

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            // Setup
            float pxsize = 14.56f;
            g = e.Graphics;
            g.ScaleTransform(pxsize, pxsize);
            g.FillRectangle(Brushes.Gray, 0, 0, Size.Width, Size.Height);


            Color[] pal = BuildPal();
            DrawSpr(gh.tempSPR(), pal, 0, 0, 8, g);
        }

        /// <summary>
        /// Draw a sprite
        /// </summary>
        /// <param name="spr">Sprite Data</param>
        /// <param name="pal">Palette Data</param>
        /// <param name="x">X position to draw Sprite at</param>
        /// <param name="y">Y position to draw Sprite at</param>
        /// <param name="w">Width of Sprite</param>
        /// <param name="g">Graphics object of canvas to draw on</param>
        public void DrawSpr(byte[] spr, Color[] pal, int x, int y, int w, Graphics g)
        {
            for (int i = 0; i < spr.Length; i++)
            {
                SolidBrush brush = new SolidBrush(pal[spr[i]]);
                g.FillRectangle(brush, x + i - ((i / w) * w), y + i / w, 1, 1);
            }
        }

        /// <summary>
        /// Builds Mario's palette
        /// </summary>
        /// <returns></returns>
        public Color[] BuildPal()
        {
            byte[] bPal = new byte[72];
            ushort[] sPal = new ushort[36];
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

            /*for (int i = 0; i < 72; i++)
            {
                bPal[i] = gh.ROMHandler.ROM[0x3280 + i];
            }
            for (int i = 0; i < 36; i++)
            {
                try { sPal[i] = BitConverter.ToUInt16(bPal, i * 2); } catch { }
            }
            Color[] pal = new Color[sPal.Length];
            for (int i = 0; i < sPal.Length; i++)
            {
                uint[] col = gh.Pal2Col(sPal[i]);
                pal[i] = Color.FromArgb((int)col[0], (int)col[1], (int)col[2]);
            }
            pal[0] = Color.Transparent;*/
            return pal;
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            float pxsize = 14.56f;
            g = e.Graphics;
            g.ScaleTransform(pxsize, pxsize);
            g.FillRectangle(Brushes.Gray, 0, 0, Size.Width, Size.Height);

            Color[] pal = BuildPal();
            DrawSpr(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19,20,21,22,23,24,25,26,27,28,29,30,31 }, pal, 0, 0, 16, g);
        }
    }
}
