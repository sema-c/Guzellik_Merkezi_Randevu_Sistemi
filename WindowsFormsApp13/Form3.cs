using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp13
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            guna2ShadowForm1.SetShadowForm(this);
            container(new Form4());
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            container(new Form4());
        }

        private void container(object _form)
        {
            if (guna2Panelcont.Controls.Count > 0) guna2Panelcont.Controls.Clear();

            Form fm = _form as Form;
            fm.TopLevel = false;
            fm.FormBorderStyle = FormBorderStyle.None;
            fm.Dock = DockStyle.Fill;
            guna2Panelcont.Controls.Add(fm);
            guna2Panelcont.Tag = fm;
            fm.Show();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            container(new Form6());
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            container(new Form5());
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            container(new Form7());
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            container(new Form8());
        }
    }
}
