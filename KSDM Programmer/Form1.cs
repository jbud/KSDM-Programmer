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
            nameArray = System.IO.Ports.SerialPort.GetPortNames();      // get a list of available ports
            int i = 0;
            foreach (string p in nameArray)
            {
                exe test = new exe(p, "", true);                        // Test each port searching for m328p microprocessor
                if (test.success)
                    break;
                else
                    i++;
            }
            comboBox1.DataSource = nameArray;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.SelectedIndex = i;                            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            textBox1.Text = openFileDialog1.FileName;
            button2.Enabled = true;
            button2.Text = "Flash";
            richTextBox1.Text = "Click \"Flash\" to continue...";
            richTextBox1.ForeColor = Color.GreenYellow;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Text = "Flashing!";
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
                    progressBar1.ForeColor = Color.Red;                 // An exception was thrown, does AVRDUDE exist???
                    richTextBox1.ForeColor = Color.Red;
                    progressBar1.Style = ProgressBarStyle.Continuous;
                    progressBar1.Value = 100;
                }
                else if (ex.done)
                {
                    progressBar1.Style = ProgressBarStyle.Continuous;
                    progressBar1.Value = 100;
                    button2.Text = "Finished!";
                    textBox1.Text = "";
                    openFileDialog1.FileName = "";
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