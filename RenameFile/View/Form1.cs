using RenameFile.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RenamFile.View
{
    public partial class Form1 : Form
    {
        private Form1ViewModel viewModel;
        public Form1()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            textBox1.ReadOnly = true;

            viewModel = new Form1ViewModel(this);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.textBox1.DataBindings.Add("Text", viewModel, "DirPath");
            this.textBox2.DataBindings.Add("Text", viewModel, "startNO");
            this.richTextBox1.DataBindings.Add("Text", viewModel, "Msg");
            this.dataGridView1.DataBindings.Add("DataSource", viewModel, "DataList");

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog diag = new FolderBrowserDialog();
            if (diag.ShowDialog() == DialogResult.OK)
            {

                viewModel.DirPath = diag.SelectedPath;


            }
        }
        private void button2_Click(object sender, EventArgs e)
        {

            viewModel.StartNO = textBox2.Text;
            if (string.IsNullOrEmpty(viewModel.StartNO))
            {
                viewModel.UpdateMsg("输入起始编号！");
                return;
            }
            try
            {
                Convert.ToInt32(viewModel.StartNO);
            }
            catch (Exception)
            {

                viewModel.UpdateMsg("起始编号只能是数字");
                return;
            }

            (new Thread(() =>
            {

                viewModel.GenerateNewName();
                this.Invoke((Action)(() =>
                {
                    this.dataGridView1.Refresh();
                }));

            })).Start();


        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog diag = new OpenFileDialog();
            diag.Multiselect = true;
            if (diag.ShowDialog() == DialogResult.OK)
            {
                viewModel.Files = diag.FileNames.ToList();

            }
        }
        private void button4_Click(object sender, EventArgs e)
        {


            (new Thread(() =>
            {
                int errRowNum = -1;
                if ((errRowNum = viewModel.ValidateGridData()) != -1)
                {
                    viewModel.UpdateMsg(string.Format("位置{0}行，新名称为空。", errRowNum + 1));

                    this.dataGridView1.ClearSelection();
                    this.dataGridView1.Rows[errRowNum].Selected = true;
                }
                else
                {
                    viewModel.CommitChanged();
                }



            })).Start();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            this.richTextBox1.SelectionStart = this.richTextBox1.Text.Length;
            this.richTextBox1.ScrollToCaret();
        }



        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
             e.RowBounds.Location.Y,
             dataGridView1.RowHeadersWidth - 4,
             e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                dataGridView1.RowHeadersDefaultCellStyle.Font,
                rectangle,
                dataGridView1.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        private void dataGridView1_DragDrop(object sender, DragEventArgs e)
        {
            //获得路径
            string[] pathList = ((string[])e.Data.GetData(DataFormats.FileDrop));

            foreach( string path in pathList)
            {
                if (Directory.Exists(path))
                {
                    continue;
                }
                else
                {
                    if (File.Exists(path))
                    {
                         viewModel.Files.Add(path);
                    }
                    else
                    {
                        Console.WriteLine("无效路径");
                    }

                }

            }
          
        }

        private void dataGridView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // 表明是所有类型的数据，比如文件路径
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }

        }

        private void 清空ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.viewModel.Clear();
        }
    }
}
