using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

namespace Perfect_Dark_Automation {
    public partial class Form1 : Form {

        PerfectDark perfectdark;

        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            Log.SetLogWindow(logWindow);
            perfectdark = new PerfectDark();
            Filters.active = label16;
        }

        private void button1_Click(object sender, EventArgs e) {
            Filters.Add(new Filter(textBox3.Text, textBox2.Text, textBox1.Text, checkBox1.Checked));
            textBox3.Text = "";
            textBox2.Text = "";
            textBox1.Text = "";
        }

        private void timer1_Tick(object sender, EventArgs e) {
            Memory.Update(true);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            timer1.Interval = (Int32.Parse(comboBox1.Text) * 1000);
            Log.WriteLine("Changed refresh interval to " + timer1.Interval + "ms");
        }


        private void button3_Click(object sender, EventArgs e) {
            if (button3.Text == "Go") {
                button3.Text = "Stop";
                timer1.Start();
                Memory.Update(false);
            }
            else {
                button3.Text = "Go";
                timer1.Stop();
            }
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e) {
            timer1.Interval = (Int32.Parse(comboBox1.Text) * 1000);
            Log.WriteLine("Refresh update interval to " + timer1.Interval + "ms");
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e) {
            Log.maxLength = Int32.Parse(comboBox3.Text);
            Log.WriteLine("Maximum log size is now " + Log.maxLength);
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e) {
            if (checkBox3.Checked)
                Log.writeToFile = true;
            else
                Log.writeToFile = false;
        }

        private void button4_Click(object sender, EventArgs e) {
            Log.WriteLine("Current Filters:~");
            foreach (Filter filter in Filters.filters) {
                Log.WriteLine("fileName:" + filter.fileName + " uploader:" + filter.uploader + " hash:" + filter.hash + " persistant?" + filter.persistant);
            }
        }
    }
}
