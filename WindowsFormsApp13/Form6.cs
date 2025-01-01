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
    public partial class Form6 : Form
    {
        public Form6()
        {
            InitializeComponent();
        }

        NpgsqlConnection baglan = new NpgsqlConnection("server = localHost; port = 5432; Database = Proje; user Id = postgres; password = Ssema.7.");

        private void SilMusteri(int musteriID)
        {
            try
            {
                baglan.Open();
                string sorgu = "DELETE FROM musteri WHERE musteriID = @musteriID";

                using (NpgsqlCommand cmd = new NpgsqlCommand(sorgu, baglan))
                {
                    cmd.Parameters.AddWithValue("@musteriID", musteriID);
                    int etkilenensatir = cmd.ExecuteNonQuery();

                    if (etkilenensatir > 0)
                    {
                        MessageBox.Show("Müşteri başarıyla silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Müşteri bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
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

        private void GuncelleMusteri(int musteriID, string yeniTelefon, string yeniMail)
        {
            try
            {
                baglan.Open();
                string sorgu = "UPDATE musteri SET telefon = @yeniTelefon, mail = @yeniMail WHERE musteriID = @musteriID";

                using (NpgsqlCommand cmd = new NpgsqlCommand(sorgu, baglan))
                {
                    cmd.Parameters.AddWithValue("@musteriID", musteriID);
                    cmd.Parameters.AddWithValue("@yeniTelefon", yeniTelefon);
                    cmd.Parameters.AddWithValue("@yeniMail", yeniMail);

                    int etkilenensatir = cmd.ExecuteNonQuery();

                    if (etkilenensatir > 0)
                    {
                        MessageBox.Show("Müşteri bilgileri başarıyla güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Müşteri bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
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

        private void button1_Click(object sender, EventArgs e)
        {
            baglan.Open();
            NpgsqlCommand komut = new NpgsqlCommand("insert into musteri (musteriAd, musteriSoyad, telefon, mail) values (@p1,@p2,@p3,@p4)", baglan);
            komut.Parameters.AddWithValue("@p1", textad.Text);
            komut.Parameters.AddWithValue("@p2", textsoyad.Text);
            komut.Parameters.AddWithValue("@P3", texttel.Text);
            komut.Parameters.AddWithValue("@P4",textmail.Text);
            komut.ExecuteNonQuery();
            baglan.Close();
            MessageBox.Show("Kişi Sisteme Kaydedildi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void Form6_Load(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textSilMusteriID.Text, out int musteriID))
            {
                SilMusteri(musteriID);
            }
            else
            {
                MessageBox.Show("Lütfen geçerli bir müşteri ID'si girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textGuncelleMusteriID.Text, out int musteriID))
            {
                string yeniTelefon = textYeniTelefon.Text;
                string yeniMail = textYeniMail.Text;

                GuncelleMusteri(musteriID, yeniTelefon, yeniMail);
            }
            else
            {
                MessageBox.Show("Lütfen geçerli bir müşteri ID'si girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
