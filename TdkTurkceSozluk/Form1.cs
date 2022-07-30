using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;
using HtmlAgilityPack;

namespace TdkTurkceSozluk
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            webBrowser1.Document.GetElementById("tdk-srch-input").SetAttribute("value", txt_Ara.Text);
            webBrowser1.Document.GetElementById("tdk-search-btn").InvokeMember("Click");
            await PageLoad(2); // aramanın yüklenmesini bekliyoruz.

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(webBrowser1.DocumentText);
            HtmlElement html_maddeler = webBrowser1.Document.GetElementById("maddeler0");
            try
            {
                textBox1.Text = (html_maddeler.InnerText).Replace(@"GÜNCEL TÜRKÇE SÖZLÜK", "").Replace("Birleşik Kelimeler", "");
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("TDK'da böyle bir kelime yer almamaktadır.", "Kelime Yok!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                txt_Ara.Clear();
                textBox1.Clear();
            }
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            webBrowser1.ScriptErrorsSuppressed = true;
            string url = "https://sozluk.gov.tr/";
            this.webBrowser1.Navigate(url);
            await PageLoad(1);
        }

        private async Task PageLoad(int TimeOut) // sayfayı bekletme
        {
            TaskCompletionSource<bool> PageLoaded = null;
            PageLoaded = new TaskCompletionSource<bool>();
            int TimeElapsed = 0;
            webBrowser1.DocumentCompleted += (s, e) =>
            {
                if (webBrowser1.ReadyState != WebBrowserReadyState.Complete) return;
                if (PageLoaded.Task.IsCompleted) return; PageLoaded.SetResult(true);
            };
            while (PageLoaded.Task.Status != TaskStatus.RanToCompletion)
            {
                await Task.Delay(10);
                TimeElapsed++;
                if (TimeElapsed >= TimeOut * 100) PageLoaded.TrySetResult(true);
            }


        }
    }
}
