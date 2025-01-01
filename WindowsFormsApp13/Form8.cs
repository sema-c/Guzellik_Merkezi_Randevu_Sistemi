using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace WindowsFormsApp13
{
    public partial class Form8 : Form
    {
        public Form8()
        {
            InitializeComponent();
        }

        NpgsqlConnection baglan = new NpgsqlConnection("server = localHost; port = 5432; Database = Proje; userId = postgres; password = Ssema.7.");

        private void Form8_Load(object sender, EventArgs e)
        {
            uzmanliste();
            hizmetliste();
        }

        void uzmanliste()
        {
            NpgsqlDataAdapter da3 = new NpgsqlDataAdapter("Select * From uzman", baglan);
            DataTable dt3 = new DataTable();
            da3.Fill(dt3);
            guna2DataGridView1.DataSource = dt3;
        }

        void hizmetliste()
        {
            NpgsqlDataAdapter da2 = new NpgsqlDataAdapter("Select * From hizmet", baglan);
            DataTable dt2 = new DataTable();
            da2.Fill(dt2);
            guna2DataGridView2.DataSource = dt2;
        }

        private void btnEkle_Click_1(object sender, EventArgs e)
        {
            if (txtUzmanAd.Text == "" || txtUzmanSoyad.Text == "" || txtAlanID.Text == "")
            {
                MessageBox.Show("Lütfen tüm alanları doldurun!");
                return;
            }

            try
            {
                baglan.Open();

                string query = "CALL adduzmansifre(@uzmanAd, @uzmanSoyad, @alanID); " +
                 "SELECT sifre FROM Kullanici WHERE kullaniciAdi = (SELECT MAX(uzmanID)::TEXT FROM Uzman)";

                NpgsqlCommand cmd = new NpgsqlCommand(query, baglan);

                cmd.Parameters.AddWithValue("@uzmanAd", txtUzmanAd.Text);
                cmd.Parameters.AddWithValue("@uzmanSoyad", txtUzmanSoyad.Text);
                cmd.Parameters.AddWithValue("@alanID", int.Parse(txtAlanID.Text));

                string yeniSifre = cmd.ExecuteScalar()?.ToString();
                MessageBox.Show($"Yeni uzman başarıyla eklendi. Şifre: {yeniSifre}", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
            finally
            {
                baglan.Close();
            }
        }

        private void btnSil_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (guna2DataGridView1.SelectedRows.Count > 0)
                {
                    int uzmanID = Convert.ToInt32(guna2DataGridView1.SelectedRows[0].Cells["uzmanID"].Value);

                    baglan.Open();

                    string sorgu = "DELETE FROM uzman WHERE uzmanID = @id";
                    using (var komut = new NpgsqlCommand(sorgu, baglan))
                    {
                        komut.Parameters.AddWithValue("@id", uzmanID);
                        komut.ExecuteNonQuery();
                    }

                    MessageBox.Show("Uzman başarıyla silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    uzmanliste();
                }
                else
                {
                    MessageBox.Show("Lütfen silinecek uzmanı seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                baglan.Close();
            }
        }

        private void guna2DataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
