using CefSharp;
using HtmlAgilityPack;
using MenetrendTervezo.classes;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MenetrendTervezo
{
    /// <summary>
    /// Interaction logic for IdopontMeneger.xaml
    /// </summary>
    public partial class IdopontMeneger : UserControl
    {
        public string _Idopont { get; private set; }

        public string _Bolt
        {
            get { return this.textbox_url.Text; }
        }

        public string _Cim
        {
            get { return this.textbox_cim.Text; }
        }

        public string _Kep
        {
            get { return this.combobox_kepek.Text; }
        }

        public string[] _Kepek
        {
            get { return this.combobox_kepek.Items.Cast<string>().ToArray(); }
        }

        public string _KepMod { get; private set; }

        private string _streamId;
        public string _StreamId
        {
            get { return this._streamId; }
            set
            {
                this._streamId = value;
                this.textblock_id.Text = value;
            }
        }

        public StackPanel _container;

        public DispatcherTimer timer = new DispatcherTimer();

        public IdopontMeneger(StackPanel container)
        {
            InitializeComponent();

            this._container = container;

            this._KepMod = ((ComboBoxItem)this.combobox_kepmod.SelectedItem).Content.ToString();
            this._Idopont = this.combobox_timespan.Text;

            this.timer.Interval = TimeSpan.FromSeconds(1);
            this.timer.Tick += this.Timer_Tick;
        }

        private Window _ParentWindow = null;

        public IdopontMeneger(StackPanel container, StreamData item)
        {
            InitializeComponent();

            this._container = container;

            this.textbox_cim.Text = item.Cim;
            this.combobox_kepek.ItemsSource = item.Kepek;
            this.combobox_kepek.Text = item.KepUrl;

            if (Uri.IsWellFormedUriString(item.KepUrl, UriKind.Absolute)) this.image_preview.Source = new BitmapImage(new Uri(item.KepUrl));

            this.textbox_url.Text = item.Bolt;

            this._KepMod = item.KepMeretezes;
            this._StreamId = item.ID;
            this._Idopont = item.Idopont;

            this.timer.Interval = TimeSpan.FromSeconds(1);
            this.timer.Tick += this.Timer_Tick;
        }

        private void FindParent()
        {
            FrameworkElement o = this;
            while (o != null)
            {
                if (o is Window)
                {
                    this._ParentWindow = o as Window;
                    break;
                }
                else o = o.Parent as FrameworkElement;
            }
        }

        private void button_torles_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this._container != null) this._container.Children.Remove(this);
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            switch (this._KepMod)
            {
                case "Contain":
                    this.combobox_kepmod.SelectedIndex = 0;
                    break;
                case "Cover":
                    this.combobox_kepmod.SelectedIndex = 1;
                    break;
            }


            if (this._Idopont != null) this.combobox_timespan.Text = this._Idopont;
            this.FindParent();
        }

        private void textbox_url_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                string s = this.textbox_url.Text;
                if (Uri.IsWellFormedUriString(s, UriKind.Absolute))
                {
                    Uri uri = new Uri(s);
                    this.browser.Address = "";
                    this.browser.Address = uri.AbsoluteUri;
                    this.IsEnabled = false;
                    this._ParentWindow.Cursor = Cursors.Wait;
                }
            }
        }

        private void combobox_kepmod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                var current = this.combobox_kepmod.SelectedItem as ComboBoxItem;

                if (current.Content.ToString() == "Cover") this.image_preview.Stretch = System.Windows.Media.Stretch.UniformToFill;
                else this.image_preview.Stretch = System.Windows.Media.Stretch.Uniform;

                this._KepMod = current.Content.ToString();
            }
        }

        private void browser_FrameLoadEnd(object sender, CefSharp.FrameLoadEndEventArgs e)
        {
            this.timer.Stop();
            this.timer.Start();
            //this.SetContent();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.timer.Stop();

            this.Dispatcher.Invoke(() =>
            {
                this.SetContent();
            });
        }

        private void combobox_timespan_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                this._Idopont = this.combobox_timespan.Text;
            }
        }

        private void combobox_kepek_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                string s = this.combobox_kepek.Text;
                if (Uri.IsWellFormedUriString(s, UriKind.Absolute))
                {
                    try
                    {
                        var kep = new BitmapImage(new Uri(s));
                        this.image_preview.Source = kep;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Hiba a kép betöltésekor!", MessageBoxButton.OK, MessageBoxImage.Error);
                        this.image_preview.Source = null;
                    }
                }
            }
        }

        private async void SetContent()
        {
            this.combobox_kepek.ItemsSource = null;

            Uri uri = new Uri(this.browser.Address);
            string oldal = Encoding.UTF8.GetString(Encoding.Default.GetBytes(await this.browser.GetMainFrame().GetSourceAsync()));

            string title = "";
            string image = "";

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(oldal);

            var titleNodes = doc.DocumentNode.SelectNodes("//meta").Where(x => x.Attributes["property"]?.Value == "og:title").Select(x => x.Attributes["content"]?.Value).FirstOrDefault();
            if (titleNodes != null) title = titleNodes;
            var imageNodes = doc.DocumentNode.SelectNodes("//meta").Where(x => x.Attributes["property"]?.Value == "og:image").Select(x => x.Attributes["content"]?.Value).FirstOrDefault();
            if (imageNodes != null) image = imageNodes.Replace("&amp;", "&");

            // címek:
            switch (uri.Host.ToLower())
            {
                case "store.steampowered.com":
                    if (this.textbox_cim.Text == "") this.textbox_cim.Text = title.Replace("a Steamen", "").Replace("on Steam", "").Trim();
                    break;
                case "www.microsoft.com":
                    if (this.textbox_cim.Text == "") this.textbox_cim.Text = title.Replace("megv�s�rl�sa � Microsoft Store hu-HU", "").Replace(" - Microsoft Store", "").Replace("Buy ", "").Trim();
                    break;
                case "www.minecraft.net":
                    if (this.textbox_cim.Text == "") this.textbox_cim.Text = title.Replace("Official Site", "").Trim();
                    break;
                case "www.epicgames.com":
                    if (this.textbox_cim.Text == "")
                    {
                        this.textbox_cim.Text =
                            title.Replace("- Epic Games Store", "")
                                 .Replace("&amp;", "&")
                                 .Replace("Pre-Purchase & Pre-Order", "")
                                 .Replace("| Download and Buy Today", "")
                                 .Trim();
                    }
                    break;
                case "store.ubi.com":
                    if (this.textbox_cim.Text == "")
                    {
                        title = doc.DocumentNode.SelectSingleNode("//title").InnerText.Trim();
                        this.textbox_cim.Text = title;
                    }
                    break;
                case "www.humblebundle.com":
                    if (this.textbox_cim.Text == "")
                    {
                        this.textbox_cim.Text = Regex.Replace(title, "from the Humble Store", "", RegexOptions.IgnoreCase).Replace("Buy ", "").Trim();
                    }
                    break;
                default:
                    if (this.textbox_cim.Text == "") this.textbox_cim.Text = (title.Contains("|") ? title.Remove(title.IndexOf('|')) : title).Trim();
                    break;
            }

            // og:iamge tagek:
            switch (uri.Host.ToLower())
            {
                case "store.steampowered.com":
                case "www.microsoft.com":
                    string s = image.Remove(image.LastIndexOf('?')).Trim();
                    if (!this.combobox_kepek.Items.Contains(s)) this.combobox_kepek.Items.Add(s);
                    break;
                default:
                    if (Uri.IsWellFormedUriString(image, UriKind.Absolute)) s = image.Trim();
                    else s = "https://" + uri.Host + image.Trim();

                    if (!this.combobox_kepek.Items.Contains(s)) this.combobox_kepek.Items.Add(s);
                    break;
            }

            // tovabbi képek:
            switch (uri.Host.ToLower())
            {
                case "store.steampowered.com":
                    var allImageNodes = doc.DocumentNode.SelectNodes("//a[@class='highlight_screenshot_link']");
                    if (allImageNodes != null)
                    {
                        foreach (HtmlNode item in allImageNodes)
                        {
                            string src = item.GetAttributeValue("href", null);

                            if (src != null && src != "")
                            {
                                if (src.Contains("?")) src = src.Remove(src.LastIndexOf('?'));
                                if (!this.combobox_kepek.Items.Contains(src)) this.combobox_kepek.Items.Add(src);
                            }
                        }
                    }
                    break;
                case "store.ubi.com":
                    allImageNodes = doc.DocumentNode.SelectNodes("//img[@class='media-slider-image swapped']");
                    if (allImageNodes != null)
                    {
                        foreach (HtmlNode item in allImageNodes)
                        {
                            string src = item.GetAttributeValue("src", null)?.Replace("&amp;", "&");
                            if (src != null && src != "")
                            {
                                if (src.Contains("?")) src = src.Remove(src.IndexOf('?'));
                                if (!this.combobox_kepek.Items.Contains(src)) this.combobox_kepek.Items.Add(src);
                            }
                        }
                    }
                    break;
                case "www.humblebundle.com":
                    allImageNodes = doc.DocumentNode.SelectNodes("//div[@class='carousel-image-container']//img");
                    if (allImageNodes != null)
                    {
                        foreach (HtmlNode item in allImageNodes)
                        {
                            string src = item.GetAttributeValue("data-lazy", null)?.Replace("&amp;", "&");
                            if (src != null && src != "")
                            {
                                if (!this.combobox_kepek.Items.Contains(src)) this.combobox_kepek.Items.Add(src);
                            }
                        }
                    }
                    break;
                default:
                    allImageNodes = doc.DocumentNode.SelectNodes("//img");
                    if (allImageNodes != null)
                    {
                        Regex r = new Regex("//img\\.youtube\\.com*");

                        foreach (HtmlNode item in allImageNodes)
                        {
                            string src = item.GetAttributeValue("src", null);
                            if (src != null && src != "")
                            {
                                if (r.IsMatch(src)) src = "https:" + src;

                                Uri u = new Uri(src, UriKind.RelativeOrAbsolute);
                                if (!u.IsAbsoluteUri) src = "https://" + uri.Host + "/" + src;

                                if (!this.combobox_kepek.Items.Contains(src)) this.combobox_kepek.Items.Add(src);
                            }
                        }
                    }

                    break;
            }

            if (this.combobox_kepek.SelectedIndex == -1) this.combobox_kepek.SelectedIndex = 0;

            this.IsEnabled = true;
            this._ParentWindow.Cursor = null;
        }
    }
}
