using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Glee.Drawing;

namespace simpleDB
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            Graph g = new Graph("graph");
            for (int i = 0; i < Form1.size; i++)
            {
                Edge edge = (Edge)g.AddEdge(Form1.points[i, 0].ToString(), Form1.points[i, 1].ToString());
                edge.Attr.Label = Form1.amounts[i].ToString() + "\n";
                edge.Attr.Label += Form1.times[i] + "\n";
                edge.Attr.Label += Form1.dateTimes[i].ToString("yyyy/MM/dd")+"\n";
                if (Form1.points[i, 0] >= 0 || Form1.points[i, 1] >= 0)
                {
                    edge.Attr.Label += Form1.description[i];
                }
            }
            gViewer.Graph = g;
        }
    }
}
