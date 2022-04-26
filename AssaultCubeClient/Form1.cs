using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AssaultCubeClient.Model;
using ezOverLay;

namespace AssaultCubeClient
{
    public partial class Form1 : Form
    {
        ez ez = new ez();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            //ez.SetInvi(this);
           
        }
    }
}
