using Npgsql;
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
    public partial class Form9 : Form
    {
        private int loggedInUzmanID;
        public Form9( int uzmanID)
        {
            InitializeComponent();
            this.loggedInUzmanID = uzmanID;
        }

        NpgsqlConnection baglan = new NpgsqlConnection("server = localHost; port = 5432; Database = Proje; user ID = postgres; password = Ssema.7.");

        private void LoadRandevular()
        {
            try
            {
                baglan.Open();

                string query = "SELECT r.randevuID, m.musteriAd, m.musteriSoyad, r.tarih, r.saat, r.durum FROM Randevu r JOIN Musteri m ON r.musteriID = m.musteriID WHERE r.uzmanID = @P1;";
                NpgsqlCommand cmd = new NpgsqlCommand(query, baglan);
                cmd.Parameters.AddWithValue("@P1", loggedInUzmanID);

                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                guna2DataGridView1.DataSource = dt;
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

        private void Form9_Load(object sender, EventArgs e)
        {
            LoadRandevular();
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtEskiSifre.Text) ||
                string.IsNullOrEmpty(txtYeniSifre.Text) ||
                string.IsNullOrEmpty(txtYeniSifreOnay.Text))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtYeniSifre.Text != txtYeniSifreOnay.Text)
            {
                MessageBox.Show("Yeni şifreler eşleşmiyor!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                baglan.Open();

                string kontrolQuery = "SELECT COUNT(*) FROM Kullanici WHERE kullaniciadi = @P1 AND sifre = @P2";
                NpgsqlCommand kontrolCmd = new NpgsqlCommand(kontrolQuery, baglan);
                kontrolCmd.Parameters.AddWithValue("@P1", loggedInUzmanID.ToString());
                kontrolCmd.Parameters.AddWithValue("@P2", txtEskiSifre.Text);

                int sonuc = Convert.ToInt32(kontrolCmd.ExecuteScalar());
                if (sonuc == 0)
                {
                    MessageBox.Show("Eski şifre yanlış!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string updateQuery = "UPDATE kullanici SET sifre = @P1 WHERE kullaniciadi = @P2";
                NpgsqlCommand updateCmd = new NpgsqlCommand(updateQuery, baglan);
                updateCmd.Parameters.AddWithValue("@P1", txtYeniSifre.Text);
                updateCmd.Parameters.AddWithValue("@P2", loggedInUzmanID.ToString());

                int rowsAffected = updateCmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Şifre başarıyla değiştirildi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtEskiSifre.Clear();
                    txtYeniSifre.Clear();
                    txtYeniSifreOnay.Clear();
                }
                else
                {
                    MessageBox.Show("Şifre değiştirilemedi!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }
}
