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
        private bool isRP = false;
        string potential;
        public Form1()
        {
            InitializeComponent();
            string[] nameArray;
            if (exe.findKSDM())
            {
                potential = exe.fport;
                if (exe.type == "rp2040")
                {
                    openFileDialog1.FilterIndex = 2;
                }
            }

            nameArray = System.IO.Ports.SerialPort.GetPortNames();      // get a list of available ports
            
            comboBox1.DataSource = nameArray;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;

            int cindex = 0;

            foreach (string prt in comboBox1.Items)
            {
                if (prt == potential || cindex == comboBox1.Items.Count)
                {
                    break;
                }
                cindex++;
            }
            if (comboBox1.Items.Count > 1)
            {
                comboBox1.SelectedIndex = cindex;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            textBox1.Text = openFileDialog1.FileName;
            if (openFileDialog1.SafeFileName.Contains(".uf2"))
                isRP = true;
            button2.Enabled = true;
            button2.Text = "Flash";
            richTextBox1.Text = "Click \"Flash\" to continue...";
            progressBar1.Value = 0;
            richTextBox1.ForeColor = Color.GreenYellow;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Text = "Flashing!";
            richTextBox1.Text = "Flashing KSDM... Please do not unplug the module!";
            button2.Enabled = false;
            progressBar1.Style = ProgressBarStyle.Marquee;              // We can't actually use a progress bar in syncronous process spawning, so we will marquee it.
            ex = new exe(comboBox1.Text, textBox1.Text);                // spawn AVRDUDE
            loopy();                                                    // Begin wait loop.
        }
        private void loopy()
        {
            while (true)
            {
                if (!ex.success)                                        // TODO: Add fail detection based on AVRDUDE output.
                {
                    richTextBox1.ForeColor = Color.Red;
                    progressBar1.Style = ProgressBarStyle.Continuous;
                    progressBar1.Value = 98;
                    richTextBox1.Text = "Failed to flash KSDM3, contact support@stinger.store";

                    return;
                }
                else if (ex.done)
                {
                    progressBar1.Style = ProgressBarStyle.Continuous;
                    progressBar1.Value = 100;
                    button2.Text = "Finished!";
                    textBox1.Text = "";
                    openFileDialog1.FileName = "";
                    if (isRP)
                        richTextBox1.Text = "Finished!";
                    else
                        richTextBox1.Text = ex.output;                      // show all output from AVRDUDE 
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