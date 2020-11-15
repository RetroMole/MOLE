using System;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using MOLE_Back.Libs;
using MOLE_Back;
namespace win
{
    public partial class Form1 : Form
    {
        ROMHandler RH;
        public Form1()
        {
            InitializeComponent();
        }

        private void openROMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.DefaultExt = ".smc";
            openFileDialog1.DereferenceLinks = true;
            openFileDialog1.InitialDirectory = Path.GetDirectoryName(Program.args[0]);
            openFileDialog1.Multiselect = false;
            openFileDialog1.Title = "Open ROM";
            openFileDialog1.Filter = "Common ROM Files (*.smc; *.sfc) | *.smc; *.sfc | All files (*.*) | *.*";
            openFileDialog1.ShowHelp = true;
            openFileDialog1.FileName = "SMW_U.smc";
            openFileDialog1.AddExtension = true;
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.ShowDialog();
            RH = new ROMHandler(openFileDialog1.FileName);
            treeView1.CollapseAll();
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(RH.ROMName);

            ThreadStart threadStart = new ThreadStart(LoadTreeView);
            Thread thread = new Thread(threadStart);
            thread.Start();
        }

        private void saveROMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RH.Save();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.DefaultExt = ".smc";
            saveFileDialog1.DereferenceLinks = true;
            saveFileDialog1.InitialDirectory = Path.GetDirectoryName(Program.args[0]);
            saveFileDialog1.Title = "Open ROM";
            saveFileDialog1.Filter = "Common ROM Files (*.smc *.sfc) | *.smc, *.sfc | All files(*.*) | *.*";
            saveFileDialog1.ShowHelp = true;
            saveFileDialog1.ShowDialog();
            RH.ROMPath = saveFileDialog1.FileName;
            RH.Save();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (RH != null)
            {
                int addr;
                string val = "";
                string pre = "";
                if (treeView1.SelectedNode != null && (!treeView1.SelectedNode.Text.Contains(".smc") && !treeView1.SelectedNode.Text.Contains(".sfc")))
                {
                    addr = Asar.SnesToPc(Convert.ToInt32(treeView1.SelectedNode.Text.Replace("$", String.Empty)));
                    val = MOLE_Back.Utils.Hex.ByteArrToHexStr(new byte[1] { RH.ROM[addr] });
                    pre = "#$";
                }
                else { val = "unknown"; }

                label1.Text = "Value: "+pre+val;
                textBox1.Text = val;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int addr = 0;
            if (treeView1.SelectedNode != null && (!treeView1.SelectedNode.Text.Contains(".smc") && !treeView1.SelectedNode.Text.Contains(".sfc")))
            {
                addr = Asar.SnesToPc(Convert.ToInt32(treeView1.SelectedNode.Text.Replace("$", String.Empty)));
                Regex regex = new Regex(@"^[\dA-F]{2}$");
                Match match = regex.Match(textBox1.Text);
                if (match.Success)
                {
                    RH.HexWrite(textBox1.Text, (uint)addr, "Main");
                    treeView1_AfterSelect(new object(), new TreeViewEventArgs(treeView1.SelectedNode));
                }
            }
        }

        private void LoadTreeView()
        {
            Asar.Init();
            for (int i = 0; i < RH.ROM.Length; i++)
            {
                try
                {
                    var a = treeView1.BeginInvoke(new Deleg(AddNode), i);
                    Thread.Sleep(10);
                    treeView1.EndInvoke(a);
                }
                catch { Thread.CurrentThread.Abort(); }
            }
            Asar.Close();
            foreach (TreeNode node in treeView1.Nodes[0].Nodes)
            {
                if (node.Text == "-1") node.Remove();
            }
        }
        private delegate void Deleg(int i);
        private void AddNode(int i)
        {
            treeView1.Nodes[0].Nodes.Add("$"+Asar.PcToSnes(i).ToString());
        }

        private void testBackendToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MOLE.Main(Program.args);
        }
    }
}
