using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace WindowsFormsApp13
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        NpgsqlConnection baglan = new NpgsqlConnection("server = localHost; port = 5432; Database = Proje; user ID = postgres; password = Ssema.7.");

        private void Form1_Load(object sender, EventArgs e)
        {
            guna2ShadowForm1.SetShadowForm(this);

        }

        private void guna2PictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (guna2TextBox1.Text == "" || guna2TextBox2.Text == "")
            {
                MessageBox.Show("Lütfen boş alanları doldurun!");
            }

            else
            {
                try
                {
                    baglan.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand("Select * from kullanici where kullaniciadi = @P1 AND sifre = @P2", baglan);
                    cmd.Parameters.AddWithValue("@P1", guna2TextBox1.Text);
                    cmd.Parameters.AddWithValue("@P2", guna2TextBox2.Text);
                    NpgsqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        Form2 frm = new Form2();
                        frm.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Hatalı bilgi girişi!", "UYARI!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                finally
                {
                    baglan.Close();
                }
            }

        }
        
        private void guna2TextBox2_TextChanged(object sender, EventArgs e)
        {
                guna2TextBox2.UseSystemPasswordChar = true;

        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            if (guna2TextBox1.Text == "" || guna2TextBox2.Text == "")
            {
                MessageBox.Show("Lütfen boş alanları doldurun!");
            }
            else
            {
                try
                {
                    baglan.Open();

                    string query = @"
                    SELECT u.uzmanID 
                    FROM uzman u 
                    INNER JOIN kullanici k ON CAST(u.uzmanID AS VARCHAR) = k.kullaniciadi
                    WHERE k.kullaniciadi = @P1 AND k.sifre = @P2";

                    NpgsqlCommand cmd = new NpgsqlCommand(query, baglan);
                    cmd.Parameters.AddWithValue("@P1", guna2TextBox1.Text);
                    cmd.Parameters.AddWithValue("@P2", guna2TextBox2.Text);

                    using (NpgsqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            int loggedInUzmanID = Convert.ToInt32(dr["uzmanID"]);

                            Form9 uzmanForm = new Form9(loggedInUzmanID);
                            uzmanForm.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Hatalı kullanıcı adı veya şifre!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
                finally
                {
                    baglan.Close();
                }
            }
        }
    }
}