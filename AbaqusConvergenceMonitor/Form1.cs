using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AbaqusConvergenceMonitor
{
    public partial class Form1 : Form
    {
        private long fpos;
        private DateTime last_write_time;
        private System.IO.FileStream fs;
        private List<string> arr;
        private IterationInfo info;
        private string path;


        public Form1()
        {
            InitializeComponent();
            fpos = 0;
            info = new IterationInfo();
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            var y = (string[])e.Data.GetData(DataFormats.FileDrop);

            path = y.First();
            var dir = System.IO.Path.GetDirectoryName(path);
            this.fileSystemWatcher1.Path = dir;
            this.fileSystemWatcher1.Filter = System.IO.Path.GetFileName(path);
            info.Reset();
            fpos = 0;
//            fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite  );
            ReadMsg();
            this.fileSystemWatcher1.EnableRaisingEvents = true;
        }

        private void ReadMsg()
        {
            fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
            using (var sr = new System.IO.StreamReader(fs))
            {
                fs.Position = fpos;
                string line;
                arr = new List<string>();
                while ((line = sr.ReadLine()) != null)
                {
                    arr.Add(line.TrimEnd());
                }
                fpos = fs.Position;
            }
            last_write_time = System.IO.File.GetLastWriteTime(path);

            StringBuilder sb = new StringBuilder();

            sb.Append(textBox1.Text);

            foreach (var t in arr)
            {
                if (info.Add(t))
                {
                    sb.AppendLine(info.ToString());
                }

            }
            textBox1.Text = sb.ToString();
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.ScrollToCaret();
        }


        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {

                e.Effect = DragDropEffects.Copy;
            }
        }

        private void fileSystemWatcher1_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            if (e.ChangeType == System.IO.WatcherChangeTypes.Changed)
            {
                if(System.IO.File.GetLastWriteTime(e.FullPath) > last_write_time){
                    ReadMsg();
                }
            }
        }


    }
}
