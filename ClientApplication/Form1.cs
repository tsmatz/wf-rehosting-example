using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceModel;

namespace ClientApplication
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ChannelFactory<CustomWorkflowLibrary.IWorkflow1> factory = new ChannelFactory<CustomWorkflowLibrary.IWorkflow1>("WSHttpContextBinding_IWorkflow1");
            CustomWorkflowLibrary.IWorkflow1 channel = factory.CreateChannel();
            int res = channel.CalcData(int.Parse(textBox1.Text));
            factory.Close();

            MessageBox.Show(res.ToString());
        }
    }
}
