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
using Guna.UI2.WinForms;

namespace WindowsFormsApp13
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();

            guna2DataGridView1.CellFormatting += guna2DataGridView1_CellFormatting;
        }

        NpgsqlConnection baglan = new NpgsqlConnection("server = localHost; port = 5432; Database = Proje; user Id = postgres; password = Ssema.7.");

        void hizmetliste()
        {
            baglan.Open();
            NpgsqlDataAdapter da = new NpgsqlDataAdapter("Select * From hizmet", baglan);
            DataTable dt = new DataTable();
            da.Fill(dt);
            Cmbhizmet.ValueMember = "alanID";
            Cmbhizmet.DisplayMember = "alanAdi";
            Cmbhizmet.DataSource = dt;
            baglan.Close();
        }

        void musteriliste()
        {
            baglan.Open();
            NpgsqlDataAdapter da3 = new NpgsqlDataAdapter("Select musteriid From Musteri", baglan);
            DataTable dt3 = new DataTable();
            da3.Fill(dt3);
            cmbmusteri.ValueMember = "musteriid";
            cmbmusteri.DisplayMember = "musteriid";
            cmbmusteri.DataSource = dt3;
            baglan.Close();
        }

        void randevuliste(DateTime tarih)
        {
            string sorgu = "SELECT * FROM bilgi()";
            NpgsqlDataAdapter da4 = new NpgsqlDataAdapter(sorgu, baglan);
            DataTable ds4 = new DataTable();
            da4.Fill(ds4);
            guna2DataGridView1.DataSource = ds4;
        }

        private void IptalRandevu(int randevuID)
        {
            using (var conn = new NpgsqlConnection("server = localHost; port = 5432; Database = Proje; user Id = postgres; password = Ssema.7."))
            {
                conn.Open();
                string sorgu = "UPDATE Randevu SET durum = 'iptal', saat = NULL WHERE randevuID = @RandevuID";

                using (var cmd = new NpgsqlCommand(sorgu, conn))
                {
                    cmd.Parameters.AddWithValue("@RandevuID", randevuID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void GecmisRandevulariGuncelle()
        {
            try
            {
                baglan.Open();
                string sorgu = "UPDATE Randevu SET durum = 'Geçti' " +
                               "WHERE tarih < CURRENT_DATE OR (tarih = CURRENT_DATE AND saat < CURRENT_TIME);";

                NpgsqlCommand cmd = new NpgsqlCommand(sorgu, baglan);
                int etkilenenSatir = cmd.ExecuteNonQuery();

                if (etkilenenSatir > 0)
                {
                    Console.WriteLine($"{etkilenenSatir} randevunun durumu 'Geçti' olarak güncellendi.");
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

        private void Cmbuzman_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void cmbmusteri_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form5_Load_1(object sender, EventArgs e)
        {
            hizmetliste();
            musteriliste();
            GecmisRandevulariGuncelle();
            randevuliste(DateTime.Now);
            guna2DateTimePicker1.MinDate = DateTime.Today;
            
            cmbsaat.DataSource = null;
            cmbsaat.Items.Clear();

            cmbsaat.Items.Add("Saat Seçiniz");
            cmbsaat.SelectedIndex = 0;
        }

        private void Cmbhizmet_SelectedIndexChanged(object sender, EventArgs e)
        {
            NpgsqlDataAdapter da2 = new NpgsqlDataAdapter("Select uzmanID, (uzmanAd || ' ' || uzmanSoyad) AS isim from uzman where alanID = " + Cmbhizmet.SelectedValue, baglan);
            DataTable dt2 = new DataTable();
            da2.Fill(dt2);
            Cmbuzman.ValueMember = "uzmanID";
            Cmbuzman.DisplayMember = "isim";
            Cmbuzman.DataSource = dt2;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            baglan.Open();
            DateTime selectedDate = guna2DateTimePicker1.Value; 
            TimeSpan selectedTime = TimeSpan.Parse(cmbsaat.SelectedItem.ToString()); 
            NpgsqlCommand komut = new NpgsqlCommand("insert into Randevu (alanID, uzmanID, musteriID, tarih, saat) values (@p1, @p2, @p3, @p4, @p5)", baglan);
            komut.Parameters.AddWithValue("@p1", (Cmbhizmet.SelectedValue));
            komut.Parameters.AddWithValue("@p2", (Cmbuzman.SelectedValue));
            komut.Parameters.AddWithValue("@p3", (cmbmusteri.SelectedValue));
            komut.Parameters.AddWithValue("@P4", selectedDate.Date);
            komut.Parameters.AddWithValue("@P5", selectedTime);
            komut.ExecuteNonQuery();
            baglan.Close();
            MessageBox.Show("Randevu Olusturuldu", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private List<TimeSpan> GetAvailableHours(DateTime selectedDate, int uzmanID)
        {
            List<TimeSpan> availableHours = new List<TimeSpan>();

            string connectionString = "server = localHost; port = 5432; Database = Proje; user Id = postgres; password = Ssema.7.";
            string query = @"
                     SELECT (ts.saat::time) AS saat
                     FROM generate_series(
                     '2024-01-01 09:00:00'::timestamp, 
                     '2024-01-01 18:00:00'::timestamp, 
                     '60 minutes'::interval) AS ts(saat)
                     LEFT JOIN Randevu r 
                     ON r.Saat = ts.saat::time 
                     AND r.Tarih = @Tarih 
                     AND r.uzmanID = @uzmanID
                     WHERE r.Saat IS NULL;";
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Tarih", selectedDate.Date);
                        cmd.Parameters.AddWithValue("@uzmanID", uzmanID);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TimeSpan availableTime = reader.GetTimeSpan(0); 
                                availableHours.Add(availableTime);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            return availableHours;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DateTime selectedDate = guna2DateTimePicker1.Value.Date;
            int uzmanID = Convert.ToInt32(Cmbuzman.SelectedValue);

            List<TimeSpan> availableHours = GetAvailableHours(selectedDate, uzmanID);

            cmbsaat.DataSource = availableHours;
            cmbsaat.DisplayMember = "ToString";

            MessageBox.Show("Boş saatler listelendi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void guna2DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (guna2DataGridView1.Columns[e.ColumnIndex].Name == "tarih" && e.Value != null)
            {
                DateTime randevuTarihi;
                string durum = guna2DataGridView1.Rows[e.RowIndex].Cells["durum"].Value?.ToString(); 
                var saatHücre = guna2DataGridView1.Rows[e.RowIndex].Cells["saat"].Value; 
                TimeSpan randevuSaati;

                if (durum == "iptal")
                {
                    e.CellStyle.BackColor = Color.Red;  
                    e.CellStyle.ForeColor = Color.White;  
                }

                if (DateTime.TryParse(e.Value.ToString(), out randevuTarihi) &&
                    TimeSpan.TryParse(saatHücre?.ToString(), out randevuSaati))
                {

                    if (randevuTarihi < DateTime.Now.Date ||
                            (randevuTarihi == DateTime.Now.Date && randevuSaati < DateTime.Now.TimeOfDay))
                    {
                        e.CellStyle.BackColor = Color.Gray;
                        e.CellStyle.ForeColor = Color.White;
                    }
                    else
                    {
                        e.CellStyle.BackColor = Color.Green;  
                        e.CellStyle.ForeColor = Color.White;
                    }
                }
            }
        }

        private void guna2Button1_Click_1(object sender, EventArgs e)
        {
            if (guna2DataGridView1.SelectedRows.Count > 0)
            {
                int randevuID = Convert.ToInt32(guna2DataGridView1.SelectedRows[0].Cells["randevuID"].Value);
                IptalRandevu(randevuID);
                randevuliste(DateTime.Now);
            }
        }
    }
}
