using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DatawashLibrary;

namespace Datawash
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            using (var people = new FileStream(textBox1.Text, FileMode.Open))
            {
                using (var criterias = new FileStream(textBox2.Text, FileMode.Open))
                {

                    var washer = new Washer(people, criterias);
                    using (var file = File.OpenWrite(textBox3.Text))
                    {
                        washer.CleanTo(file);
                    }
                    button1.Enabled = true;
                }
            }
        }
    }
}
