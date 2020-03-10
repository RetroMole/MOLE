using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LA;
using LA.Properties;

namespace LAWin
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            // Backend
            Settings.Default.Reload();


            // Create and initialize a Label.
            Label label1 = new Label
            {
                BorderStyle = System.Windows.Forms.BorderStyle.None,
                Text = "Allow Dangerous Adress Access (SNES<->PC address convertor): ",
                Location = new Point(0, 0)
            };
            label1.Size = new Size(label1.PreferredWidth, label1.PreferredHeight);

            // Create and initialize a CheckBox.   
            CheckBox checkBox1 = new CheckBox
            {
                Appearance = Appearance.Normal,
                Location = new Point(350, -2),
                Checked = Settings.Default.AllowDangerAddress
            };

            // Add items to form
            Controls.Add(checkBox1);
            Controls.Add(label1);

            // Fork Properties.
            Size = PreferredSize;
            Text = "WOOH";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(Form1_FormClosing);


            void Form1_FormClosing(object sender, FormClosingEventArgs e)
            {
                if (e.CloseReason == CloseReason.UserClosing)
                {
                    if (checkBox1.Checked != Settings.Default.AllowDangerAddress)
                    {
                        dynamic result = MessageBox.Show("Do You Want To Save Before Exiting?", Application.ProductName, MessageBoxButtons.YesNoCancel);
                        if (result == DialogResult.Yes)
                        {
                            Settings.Default.Save();
                            Application.Exit();
                        }
                        else if (result == DialogResult.No)
                        {
                            Application.Exit();
                        }
                        else if (result == DialogResult.Cancel)
                        {
                            e.Cancel = true;
                        }
                    }
                }
            }
        }
    }
}
