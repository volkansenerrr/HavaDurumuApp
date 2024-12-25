using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;

namespace HavaDurumuApp
{
    public partial class frm_HavaDurumu : Form
    {
        public frm_HavaDurumu()
        {
            InitializeComponent();
        }

        // İngilizce rüzgar türlerini Türkçe karşılıklarına eşleyen bir sözlük
        Dictionary<string, string> ruzgarSozluk = new Dictionary<string, string>()
        {
           { "Calm", "Sakin" },
           { "Light air", "Hafif esinti" },
           { "Light breeze", "Hafif rüzgar" },
           { "Gentle breeze", "Hafif meltem" },
           { "Moderate breeze", "Orta rüzgar" },
           { "Fresh breeze", "Canlı rüzgar" },
           { "Strong breeze", "Kuvvetli rüzgar" },
           { "Gale", "Fırtına" },
           { "Strong gale", "Kuvvetli fırtına" },
           { "Storm", "Fırtına" },
           { "Violent storm", "Şiddetli fırtına" },
           { "Hurricane", "Kasırga" }
        };

        private string RuzgarTipiCevir(string ingilizceRuzgar)
        {
            // Eğer sözlükte karşılığı varsa çevir, yoksa olduğu gibi döndür
            return ruzgarSozluk.ContainsKey(ingilizceRuzgar) ? ruzgarSozluk[ingilizceRuzgar] : ingilizceRuzgar;
        }


        private void btn_Goster_Click(object sender, EventArgs e)
        {
            string sehir = tb_Sehir.Text.Trim(); // Kullanıcıdan şehir ismini al

            if (string.IsNullOrEmpty(sehir))
            {
                MessageBox.Show("Lütfen bir şehir girin.");
                return;
            }

            // API URL'si dinamik hale getiriliyor. Kullanıcının girdiği şehir ile sorgulama yapıyoruz.
            string path = $"https://api.openweathermap.org/data/2.5/weather?q={sehir}&appid=43c8bdcd0b9b77a91fe11ec9f1408225&mode=xml&units=metric&lang=tr";

            try
            {
                // API'den XML verisini alıyoruz
                XDocument havaDurumu = XDocument.Load(path);
                XElement current = havaDurumu.Root;

                // Şehir adı ile veriyi karşılaştırıyoruz
                var cityElement = current.Element("city");
                if (cityElement != null && sehir.Equals(cityElement.Attribute("name").Value, StringComparison.OrdinalIgnoreCase))
                {
                    double dblSicaklik = Convert.ToDouble(current.Element("temperature").Attribute("value").Value.Replace(".", ","));
                    lbl_Sicaklik.Text = "Sıcaklık = " + Convert.ToInt32(dblSicaklik).ToString() + "℃";

                    double hissedilen = Convert.ToDouble(current.Element("feels_like").Attribute("value").Value.Replace(".", ","));
                    lbl_HissedilenSicaklik.Text = "Hissedilen = " + Convert.ToInt32(hissedilen).ToString() + "℃";

                    lbl_HavaDurumu.Text = current.Element("weather").Attribute("value").Value;

                    lbl_MevcutNem.Text = current.Element("humidity").Attribute("value").Value + "%";

                    string ingilizceRuzgarTipi = current.Element("wind").Element("speed").Attribute("name").Value;
                    lbl_MevcutRuzgar.Text = RuzgarTipiCevir(ingilizceRuzgarTipi);

                    lbl_MevcutRuzgarHizi.Text = current.Element("wind").Element("speed").Attribute("value").Value + " m/s";

                    lbl_MevcutGorus.Text = current.Element("visibility").Attribute("value").Value + " m ";

                    lbl_MevcutUlke.Text = cityElement.Element("country").Value;

                    lbl_MevcutSehir.Text = cityElement.Attribute("name").Value;

                    // Hava durumu ikonu (resim)
                    string iconCode = current.Element("weather").Attribute("icon").Value;  // 'icon' kodu
                    string iconUrl = $"http://openweathermap.org/img/wn/{iconCode}.png";   // Icon URL'si

                    // Resmi yüklemek için PictureBox'a aktardım
                    pb_MevcutHavaIcon.ImageLocation = iconUrl;

                    // Resmi yükleme işlemini kontrol et
                    try
                    {
                        pb_MevcutHavaIcon.Load();
                        if (pb_MevcutHavaIcon.Image == null)
                        {
                            MessageBox.Show("Resim yüklenemedi.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Resim yüklenirken bir hata oluştu: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("Veri alınamadı, şehir adı hatalı olabilir.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
        }
    }
}
