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
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApp13
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
            ToplamMusteriSayisiGoster();
            ToplamKazancGoster();
            GrafikYukle();
        }

        NpgsqlConnection baglan = new NpgsqlConnection("server = localHost; port = 5432; Database = Proje; user Id = postgres; password = Ssema.7.");

        private void Form4_Load(object sender, EventArgs e)
        {

        }

        private void ToplamMusteriSayisiGoster()
        {
            try
            {
                baglan.Open();
                string sql = "SELECT * FROM toplam_randevu_sayisi();";

                using (NpgsqlCommand komut = new NpgsqlCommand(sql, baglan))
                {
                    using (NpgsqlDataReader reader = komut.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            long buAyToplam = reader.IsDBNull(0) ? 0 : reader.GetInt64(0);
                            long gecenAyToplam = reader.IsDBNull(1) ? 0 : reader.GetInt64(1);

                            label16.Text = " " + buAyToplam;
                            label18.Text = " " + gecenAyToplam;
                        }
                        else
                        {
                            label16.Text = " 0";
                            label18.Text = " 0";
                        }
                    }
                }
                baglan.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void ToplamKazancGoster()
        {
            try
            {
                baglan.Open();
                string sql = "SELECT * FROM toplam_kazanc_hesapla();";

                using (var komut = new NpgsqlCommand(sql, baglan))
                {
                    using (var reader = komut.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            decimal buAykiToplam = reader.GetDecimal(0);
                            decimal buYilkiToplam = reader.GetDecimal(1);

                            labelBuAyKazanc.Text = " " + buAykiToplam.ToString("C");
                            labelBuYilKazanc.Text = " " + buYilkiToplam.ToString("C");
                        }
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

        private void GrafikYukle()
        {
            try
            {
                baglan.Open();
                string sql = @"
                    SELECT 
                    TO_CHAR(tarih, 'Month') AS ay,
                    COUNT(CASE WHEN durum = 'aktif' or durum='Geçti' THEN 1 END) AS aktif_randevu,
                    COUNT(CASE WHEN durum = 'iptal' THEN 1 END) AS pasif_randevu
                    FROM Randevu
                    WHERE EXTRACT(YEAR FROM tarih) = EXTRACT(YEAR FROM CURRENT_DATE)
                    GROUP BY ay, EXTRACT(MONTH FROM tarih)
                    ORDER BY EXTRACT(MONTH FROM tarih);";

                using (NpgsqlCommand komut = new NpgsqlCommand(sql, baglan))
                using (NpgsqlDataReader reader = komut.ExecuteReader())
                {
                    chart1.Series.Clear();

                    Series aktifSeri = new Series("Aktif Randevular")
                    {
                        ChartType = SeriesChartType.Column,
                        Color = Color.Green,
                        IsValueShownAsLabel = true
                    };

                    Series pasifSeri = new Series("iptal Randevular")
                    {
                        ChartType = SeriesChartType.Column,
                        Color = Color.Red,
                        IsValueShownAsLabel = true
                    };

                    while (reader.Read())
                    {
                        string ay = reader.GetString(0).Trim();
                        int aktif = reader.GetInt32(1);
                        int iptal = reader.GetInt32(2);

                        aktifSeri.Points.AddXY(ay, aktif);
                        pasifSeri.Points.AddXY(ay, iptal);
                    }

                    chart1.Series.Add(aktifSeri);
                    chart1.Series.Add(pasifSeri);

                    chart1.ChartAreas[0].AxisX.Title = "Aylar";
                    chart1.ChartAreas[0].AxisY.Title = "Randevu Sayısı";
                    chart1.Titles.Clear();
                    chart1.Titles.Add("Aylık Aktif ve Pasif Randevular");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            baglan.Close();
        }
        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }
    }
}