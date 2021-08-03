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

            //if (exe.findKSDM())
            //{
            //    potential = exe.fport;
            //    if (exe.type == "rp2040")
            //    {
            //        openFileDialog1.FilterIndex = 2;
            //    }
            //}

            //richTextBox1.Text = "Found KSDM " + exe.type + " type, at com port: " + potential + ".";
            
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
        
        private void Form1_Shown(object sender, EventArgs e)
        {
            string[] nameArray;
            nameArray = System.IO.Ports.SerialPort.GetPortNames();      // get a list of available ports
            string typeFound = "";
            bool found = false;
            string tr;

            // convoluted way to move COM1 to end of list, usually it's not what we're looking for but sometimes it can be.
            tr = nameArray[0];
            nameArray[0] = nameArray[nameArray.Length-1];
            nameArray[nameArray.Length-1] = tr;

            foreach (string b in nameArray)
            {
                string temp = exe.serialPoke(b);
                if (temp.Contains("ksdm3"))
                {
                    found = true;
                    if (temp.Contains("avr"))
                    {
                        typeFound = "KSDM3-avr";
                        potential = b;
                        break;
                    }
                    else if (temp.Contains("rp2040"))
                    {
                        typeFound = "KSDM3-rp2040";
                        potential = b;
                        break;
                    }
                }
                continue;
            }
            if (found)
                richTextBox1.Text = "Found " + typeFound + " at com port: " + potential + ".";
            else
                richTextBox1.Text = "KSDM could not be automatically found, manually select a port or contact support@stinger.store";

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
            if (comboBox1.Items.Count - 1 >= cindex)
            {
                comboBox1.SelectedIndex = cindex;
            }
            ActiveForm.Text = "KSDM Programmer";
        }
    }
}