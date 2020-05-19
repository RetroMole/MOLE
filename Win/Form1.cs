using System;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using AsarCLR;
using LA_Back;
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
            openFileDialog1.InitialDirectory = Path.GetDirectoryName(@"C:\Users\Leuu\source\repos\Lunar Alchemy");
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
            treeView1.Nodes.Add(openFileDialog1.FileName);

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
            saveFileDialog1.InitialDirectory = Path.GetDirectoryName(@"C:\Users\Leuu\source\repos\Lunar Alchemy");
            saveFileDialog1.Title = "Open ROM";
            saveFileDialog1.Filter = "Common ROM Files (*.smc *.sfc) | *.smc, *.sfc | All files(*.*) | *.*";
            saveFileDialog1.ShowHelp = true;
            saveFileDialog1.ShowDialog();
            RH.ROMName = saveFileDialog1.FileName;
            RH.Save();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (RH != null)
            {
                int addr = Convert.ToInt32(treeView1.SelectedNode.Text);
                string val = RH.ROM[addr].ToString();
                label1.Text = (treeView1.SelectedNode != null) ? val : "unknown";
            }
        }

        private void LoadTreeView()
        {
            Asar.Init();
            for (int i = 0; i < 500; i++)
            {
                var a = treeView1.BeginInvoke(new Del(AddNode), i);
                Thread.Sleep(10);
                treeView1.EndInvoke(a);
            }
            Asar.Close();
            foreach (TreeNode node in treeView1.Nodes[0].Nodes)
            {
                if (node.Text == "-1") node.Remove();
            }
        }
        private delegate void Del(int i);
        private void AddNode(int i)
        {
            treeView1.Nodes[0].Nodes.Add(Asar.PcToSnes(i).ToString());
        }

        private void testBackendToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Lunar_Alchemy.Main(Program.args);
        }
    }
}
