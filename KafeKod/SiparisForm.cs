using KafeKod.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KafeKod
{
    public partial class SiparisForm : Form
    {

        public event EventHandler<MasaTasimaEventArgs> MasaTasiniyor;

        KafeVeri db;
        Siparis siparis;
        BindingList<SiparisDetay> blSiparisDetaylar;
        public SiparisForm(KafeVeri kafeVeri, Siparis siparis)
        {
            db = kafeVeri;
            this.siparis = siparis;
            blSiparisDetaylar =
                new BindingList<SiparisDetay>(siparis.SiparisDetaylar);
            InitializeComponent();
            MasaNolariYukle();
            MasaNoGuncelle();
            TutarGuncelle();
            cboUrun.DataSource = db.Urunler;
            cboUrun.SelectedItem = null;
            dgvSiparisDetaylari.DataSource = blSiparisDetaylar;  // data grid view e UrunAD BirimFiyat getirme
        }

        private void MasaNolariYukle()
        {
            for (int i = 0; i < db.MasaAdet; i++)
            {
                if (i != siparis.MasaNo)
                {
                    cboMasaNo.Items.Add(i);
                }
            }
        }

        private void TutarGuncelle()
        {
            lblTutar.Text = siparis.ToplamTutarTL;
        }

        private void MasaNoGuncelle()
        {
            Text = "Masa " + siparis.MasaNo;
            lblMasaNo.Text = siparis.MasaNo.ToString("00");
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            if (cboUrun.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir ürün seçiniz !!");
                return;
            }

            Urun SeciliUrun = (Urun)cboUrun.SelectedItem;
            var sipdetay = new SiparisDetay
            {
                UrunAd = SeciliUrun.UrunAd,
                BirimFiyat = SeciliUrun.BirimFiyat,
                Adet = (int)nudAdet.Value
            };
            blSiparisDetaylar.Add(sipdetay);
            cboUrun.SelectedItem = 0;
            nudAdet.Value = 1;
            TutarGuncelle();
        }

        private void btnSiparisIptal_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show(
                "Sipariş iptal edilecektir.! Onaylıyor musunuz ? ",
                "Sipariş Onayı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (dr == DialogResult.Yes)
            {
                siparis.Durum = SiparisDurum.Iptal;
                siparis.KapanisZamani = DateTime.Now;
                Close();
            }
            siparis.Durum = SiparisDurum.Iptal;

        }

        private void btnAnaSayfa_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOdemeAl_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show(
               "Ödeme alındıysa masa sonlandırılacaktır.! Onaylıyor musunuz? ",
               "Masa Kapatma Onayı",
               MessageBoxButtons.YesNo,
               MessageBoxIcon.Warning,
               MessageBoxDefaultButton.Button2);

            if (dr == DialogResult.Yes)
            {
                siparis.Durum = SiparisDurum.Odendi;
                siparis.KapanisZamani = DateTime.Now;
                siparis.OdenenTutar = siparis.ToplamTutar();
                Close();
            }
            siparis.Durum = SiparisDurum.Odendi;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (Control item in Controls)
            {
                if (item != sender)
                {
                    System.Threading.Thread.Sleep(100);
                    item.Hide();
                }

            }
        }

        private void dgvSiparisDetaylari_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                int rowIndex = dgvSiparisDetaylari.HitTest(e.X, e.Y).RowIndex;

                if (rowIndex > -1)
                {
                    dgvSiparisDetaylari.ClearSelection();
                    dgvSiparisDetaylari.Rows[rowIndex].Selected = true;
                    cmsSiparisDetay.Show(MousePosition);

                }
            }
        }
        private void tsmiSiparisDetaySil_Click(object sender, EventArgs e)
        {
            // Seçili elemanı kaldır.

            if(dgvSiparisDetaylari.SelectedRows.Count > 0)
            {
                var seciliSatir = dgvSiparisDetaylari.SelectedRows[0];
                var SipDetay = (SiparisDetay)seciliSatir.DataBoundItem;
                blSiparisDetaylar.Remove(SipDetay);
            }

            TutarGuncelle();
        }

        private void btnMasaTasi_Click(object sender, EventArgs e)
        {
            if(cboMasaNo.SelectedItem == null)
            {
                MessageBox.Show("Lütfen hedef masa noyu seçiniz.");
                return;
            }

            int eskiMasaNo = siparis.MasaNo;
            int hedefMasaNo = (int)cboMasaNo.SelectedItem;

            if (MasaTasiniyor != null)
            {
                var args = new MasaTasimaEventArgs
                {
                    TasinanSiparis = siparis,
                    EskiMasaNo = eskiMasaNo,
                    YeniMasaNo = hedefMasaNo
                };
                MasaTasiniyor(this, args);
            }

            siparis.MasaNo = hedefMasaNo;
            MasaNoGuncelle();
            MasaNolariYukle();
        }
    }

    public class MasaTasimaEventArgs : EventArgs
    {
        public Siparis TasinanSiparis { get; set; }
        public int EskiMasaNo { get; set; }
        public int YeniMasaNo { get; set; }
    }
}