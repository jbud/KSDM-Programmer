using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KSDM_Programmer
{
    public partial class Form1 : Form
    {
        private exe ex;
        public Form1()
        {
            InitializeComponent();
            string[] nameArray;
            nameArray = System.IO.Ports.SerialPort.GetPortNames();
            comboBox1.DataSource = nameArray;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            textBox1.Text = openFileDialog1.FileName;
            button2.Enabled = true;
            richTextBox1.Text = "Click \"Flash\" to continue...";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            progressBar1.Style = ProgressBarStyle.Marquee;
            ex = new exe(comboBox1.Text, textBox1.Text);
            loopy();
        }
        private void loopy()
        {
            System.Diagnostics.Debug.WriteLine("amIhere?");
            while (true)
            {
                System.Threading.Thread.Sleep(15);
                if (!ex.success)
                {
                    progressBar1.ForeColor = Color.Red;
                    progressBar1.Style = ProgressBarStyle.Continuous;
                    progressBar1.Value = 100;
                }
                else if (ex.done)
                {
                    progressBar1.Style = ProgressBarStyle.Continuous;
                    progressBar1.Value = 100;
                    richTextBox1.Text = ex.output;
                    richTextBox1.SelectionStart = richTextBox1.Text.Length;
                    richTextBox1.ScrollToCaret();
                    return;
                }
                else
                {
                    continue;
                }
            }
        }
    }
}