using CefSharp;
using MenetrendTervezo.classes;
using MenetrendTervezo.windows;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace MenetrendTervezo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string link = Environment.CurrentDirectory + "\\html_sablon\\index.html";
        public DispatcherTimer timer { get; set; } = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            this.combobox_week.ItemsSource = Week.getWeeks();
            this.combobox_week.SelectedItem = Week.getCurrentWeek();

            this.combobox_arany.ItemsSource = new AspectRatio[] {
                new AspectRatio(21,9),
                new AspectRatio(16,9),
                new AspectRatio(5,4),
                new AspectRatio(4,3),
                new AspectRatio(2,1),
                new AspectRatio(1,1),
            };

            this.combobox_arany.SelectedIndex = 1;
            this.textbox_w.Text = "1920";
            this.textbox_h.Text = "1080";

            this.chrome.BrowserSettings = new BrowserSettings(true);
            //this.chrome.Address = "http://127.0.0.1:5500/";
            this.chrome.Address = link;

            this.timer.Interval = TimeSpan.FromMilliseconds(500);
            this.timer.Tick += this.Timer_Tick;
            this.timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.timer.Stop();

            if (this.chrome.CanExecuteJavascriptInMainFrame)
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.SetDate();
                });
            }
            else
            {
                timer.Start();
            }
        }

        private static string formatDate(DateTime dt)
        {
            return dt.ToString("MMMM dd.");
        }

        private void SetDate()
        {
            this.Dispatcher.Invoke(() =>
            {
                Week c = this.combobox_week.SelectedItem as Week;
                this.chrome.GetMainFrame().ExecuteJavaScriptAsync("setDate('" + formatDate(c.Start) + "','" + formatDate(c.End) + "')");
            });
        }

        private void combobox_week_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (this.IsLoaded) SetDate();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Meretez();
        }

        private void Meretez()
        {
            var arany = this.combobox_arany.SelectedItem as AspectRatio;

            if (arany != null)
            {
                double w = this.grid_browser.ActualWidth;
                double h = this.row.ActualHeight;

                if (w / arany.Horizontal * arany.Horizontal != h)
                {
                    h = w / arany.Horizontal * arany.Vertical;
                    if (h > this.row.ActualHeight)
                    {
                        h = this.row.ActualHeight;
                        w = h / arany.Vertical * arany.Horizontal;
                    }
                }
                else
                {
                    if (w / arany.Horizontal * arany.Vertical > h) w = h / arany.Vertical * arany.Horizontal;
                    else h = w / arany.Vertical * arany.Horizontal;
                }

                this.chrome.Width = w;
                this.chrome.Height = h;
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            this.Meretez();
        }

        private void addDay(DaysData nap)
        {
            string javaScript =
                "addDay(" +
                "\"" + nap.Nev.Replace("\"", "\\\"") + "\"," +
                "\"" + nap.ID.Replace("\"", "\\\"") + "\"" +
                ");";
            this.chrome.GetMainFrame().ExecuteJavaScriptAsync(javaScript);
        }

        private void removeDay(DaysData nap)
        {
            string javaScript = "" +
                "removeDay(\"" + nap.ID.Replace("\"", "\\\"") + "\");";
            this.chrome.GetMainFrame().ExecuteJavaScriptAsync(javaScript);
        }

        private void addStream(string dayID, StreamData data)
        {
            string s = "addStream(" +
                "\"" + dayID.Replace("\"", "\\\"") + "\"," +
                "\"" + data.Idopont.Replace("\"", "\\\"") + "\"," +
                "\"" + data.KepUrl.Replace("\"", "\\\"") + "\"," +
                "\"" + data.KepMeretezes.Replace("\"", "\\\"") + "\"," +
                "\"" + data.Cim.Replace("\"", "\\\"") + "\"," +
                "\"" + data.ID.Replace("\"", "\\\"") + "\"" +
                ");";

            this.chrome.GetMainFrame().ExecuteJavaScriptAsync(s);
        }

        private void removeSteam(string streamID)
        {
            string javaScript = "removeStream(\"" + streamID.Replace("\"", "\\\"") + "\");";
            this.chrome.GetMainFrame().ExecuteJavaScriptAsync(javaScript);
        }

        private void setStreamTime(StreamData data)
        {
            string javaScript = "setStreamTime(\"" + data.ID.Replace("\"", "\\\"") + "\", \"" + data.Idopont + "\");";
            this.chrome.GetMainFrame().ExecuteJavaScriptAsync(javaScript);
        }

        private void setStreamTitle(StreamData data)
        {
            string javaScript = "setStreamTitle(\"" + data.ID.Replace("\"", "\\\"") + "\", \"" + data.Cim + "\");";
            this.chrome.GetMainFrame().ExecuteJavaScriptAsync(javaScript);
        }

        private void setStreamImage(StreamData data)
        {
            string javaScript = "setStreamImage(\"" + data.ID.Replace("\"", "\\\"") + "\", \"" + data.KepUrl + "\");";
            this.chrome.GetMainFrame().ExecuteJavaScriptAsync(javaScript);
        }

        private void setStreamImageSizeMode(StreamData data)
        {
            string javaScript = "setStreamImageSizeMode(\"" + data.ID.Replace("\"", "\\\"") + "\", \"" + data.KepMeretezes + "\");";
            this.chrome.GetMainFrame().ExecuteJavaScriptAsync(javaScript);
        }

        private void setDay(DaysData data)
        {
            string javaScript = "setDay(\"" + data.ID.Replace("\"", "\\\"") + "\",\"" + data.Nev.Replace("\"", "\\\"") + "\");";
            this.chrome.GetMainFrame().ExecuteJavaScriptAsync(javaScript);
        }

        private void chrome_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            this.SetDate();
        }

        private void button_napAdd_Click(object sender, RoutedEventArgs e)
        {
            var ablak = new NapEditor();
            ablak.Height = this.ActualHeight * 0.9;
            ablak.Owner = this;
            if (ablak.ShowDialog() == true)
            {
                //MessageBox.Show("Nincs kész!");
                this.listbox_napok.Items.Add(ablak._ReturnData);

                this.addDay(ablak._ReturnData);
                foreach (var item in ablak._ReturnData.Streamek)
                {
                    this.addStream(ablak._ReturnData.ID, item);
                }
            }
        }

        private void listbox_napok_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                DaysData cur = this.listbox_napok.SelectedItem as DaysData;

                if (cur != null) this.setEndbleButtons(true);
                else this.setEndbleButtons(false);
            }
        }

        private void setEndbleButtons(bool val)
        {
            this.button_napEdit.IsEnabled = val;
            this.button_napRemove.IsEnabled = val;
        }

        private void button_napRemove_Click(object sender, RoutedEventArgs e)
        {
            DaysData cur = this.listbox_napok.SelectedItem as DaysData;
            this.removeDay(cur);
            this.listbox_napok.Items.Remove(cur);
            if (this.listbox_napok.Items.Count == 0) NapEditor._UtolsoNap = null;
        }

        private void button_napEdit_Click(object sender, RoutedEventArgs e)
        {
            DaysData cur = this.listbox_napok.SelectedItem as DaysData;
            int index = this.listbox_napok.SelectedIndex;
            var ablak = new NapEditor(cur);
            ablak.Height = this.ActualHeight * 0.9;
            ablak.Owner = this;
            if (ablak.ShowDialog() == true)
            {
                var uj = ablak._ReturnData.Clone();

                foreach (string item in ablak._Toroltek)
                {
                    this.removeSteam(item);
                }

                foreach (string item in ablak._Modositottak)
                {
                    var c = ablak._ReturnData.Streamek.Where(x => x.ID == item).FirstOrDefault();
                    if (c != null)
                    {
                        this.setStreamTime(c);
                        this.setStreamTitle(c);
                        this.setStreamImage(c);
                        this.setStreamImageSizeMode(c);
                    }
                }

                foreach (string item in ablak._Ujak)
                {
                    var c = ablak._ReturnData.Streamek.Where(x => x.ID == item).FirstOrDefault();
                    if (c != null)
                    {
                        this.addStream(ablak._ReturnData.ID, c);
                    }
                }

                this.setDay(uj);
                this.listbox_napok.Items[index] = uj;
            }
        }

        private void combobox_arany_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            this.Meretez();
            int w, h;
            if (int.TryParse(this.textbox_h.Text, out h))
            {
                var arany = this.combobox_arany.SelectedItem as AspectRatio;
                w = (int)Math.Floor((double)h / arany.Vertical * arany.Horizontal);
                this.textbox_w.Text = w.ToString();
            }
        }

        private async void button_export_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG kép|*.png|JPEG kép|*.jpg";
            saveFileDialog.FileName = "Menetrend";

            if (saveFileDialog.ShowDialog() == true)
            {
                string html = await this.chrome.GetMainFrame().GetSourceAsync();

                do
                {
                    int start = html.IndexOf("<script>");
                    int end = html.IndexOf("</script>") + "</script>".Length;
                    string sub = html.Substring(start, end - start);

                    html = html.Replace(sub, "");
                } while (html.Contains("<script>"));

                string mappa = Directory.GetCurrentDirectory();
                Uri m = new Uri(mappa);

                html = html.Replace("./images", m.AbsoluteUri + "/html_sablon/images");

                int w, h;

                if (int.TryParse(this.textbox_h.Text, out h) && int.TryParse(this.textbox_w.Text, out w))
                {
                    HTMLrender.Render(html, w, h, saveFileDialog.FileName);
                }
            }
        }
    }
}
