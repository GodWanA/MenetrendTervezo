using MenetrendTervezo.classes;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MenetrendTervezo.windows
{
    /// <summary>
    /// Interaction logic for NapEditor.xaml
    /// </summary>
    public partial class NapEditor : Window
    {
        public static string _UtolsoNap { get; set; }

        public DaysData _ReturnData { get; set; }
        public List<string> _Toroltek { get; set; }
        public List<string> _Modositottak { get; set; }
        public List<string> _Ujak { get; set; }

        public NapEditor()
        {
            InitializeComponent();
            this.UjIdopont();
            this.FindDay(_UtolsoNap);
            if (_UtolsoNap != null) this.comboBox_nap.SelectedIndex++;
        }

        private void FindDay(string day)
        {
            if (_UtolsoNap != null)
            {
                for (int i = 0; i < this.comboBox_nap.Items.Count; i++)
                {
                    ComboBoxItem item = this.comboBox_nap.Items[i] as ComboBoxItem;
                    if (item.Content.ToString() == day)
                    {
                        this.comboBox_nap.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        public NapEditor(DaysData data)
        {
            InitializeComponent();
            ///this.NextDay();

            this._ReturnData = data;
            this.FindDay(this._ReturnData.Nev);

            foreach (StreamData item in data.Streamek)
            {
                this.UjIdopont(item);
            }
        }

        private void button_ujIdopont_Click(object sender, RoutedEventArgs e)
        {
            this.UjIdopont();
        }

        private void UjIdopont()
        {
            int index = this.stackpanel_times.Children.IndexOf(this.button_ujIdopont);
            var tmp = new IdopontMeneger(this.stackpanel_times);
            this.stackpanel_times.Children.Insert(index, tmp);
            this.scrollviewer.ScrollToEnd();
        }

        private void UjIdopont(StreamData item)
        {
            int index = this.stackpanel_times.Children.IndexOf(this.button_ujIdopont);
            var tmp = new IdopontMeneger(this.stackpanel_times, item);
            this.stackpanel_times.Children.Insert(index, tmp);
            this.scrollviewer.ScrollToEnd();
        }

        private void button_save_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();

            if (this._ReturnData == null)
            {
                var list = new List<StreamData>();

                foreach (var item in this.stackpanel_times.Children)
                {
                    if (item is IdopontMeneger)
                    {
                        var current = item as IdopontMeneger;
                        var tmp = new StreamData(current._Idopont, current._Bolt, current._Cim, current._Kep, current._KepMod, current._Kepek);
                        list.Add(tmp);
                    }
                }

                string s = ((ComboBoxItem)this.comboBox_nap.SelectedItem).Content.ToString();
                this._ReturnData = new DaysData(s, list, this.comboBox_nap.SelectedIndex);
            }
            else
            {
                string s = ((ComboBoxItem)this.comboBox_nap.SelectedItem).Content.ToString();
                this._ReturnData.Nev = s;
                this._ReturnData.Index = this.comboBox_nap.SelectedIndex;

                this._Toroltek = new List<string>();
                this._Modositottak = new List<string>();
                this._Ujak = new List<string>();

                var array = this._ReturnData.Streamek.ToArray();
                foreach (StreamData item in array)
                {
                    if (!this.BenneVan(item))
                    {
                        this._Toroltek.Add(item.ID);
                        this._ReturnData.Streamek.Remove(item);
                    }
                    else
                    {
                        this._Modositottak.Add(item.ID);
                    }
                }

                array = this._ReturnData.Streamek.ToArray();
                foreach (var item in array)
                {
                    var cur = this.GetItem(item);

                    if (cur != null)
                    {
                        item.Cim = cur._Cim;
                        item.Idopont = cur._Idopont;
                        item.KepMeretezes = cur._KepMod;
                        item.KepUrl = cur._Kep;
                        item.Bolt = cur._Bolt;
                        item.Kepek = cur._Kepek;
                    }
                }

                foreach (var item in stackpanel_times.Children)
                {
                    if (item is IdopontMeneger)
                    {
                        var cur = item as IdopontMeneger;
                        if (!this._ReturnData.BenneVan(cur._StreamId))
                        {
                            var uj = new StreamData(cur._Idopont, cur._Bolt, cur._Cim, cur._Kep, cur._KepMod, cur._Kepek);
                            this._Ujak.Add(uj.ID);
                            this._ReturnData.Streamek.Add(uj);
                        }
                    }
                }
            }

            NapEditor._UtolsoNap = _ReturnData.Nev;
        }

        private bool BenneVan(StreamData data)
        {
            foreach (var item in this.stackpanel_times.Children)
            {
                if (item is IdopontMeneger)
                {
                    var cur = item as IdopontMeneger;
                    if (cur._StreamId == data.ID) return true;
                }
            }

            return false;
        }

        private IdopontMeneger GetItem(StreamData data)
        {
            foreach (var item in this.stackpanel_times.Children)
            {
                if (item is IdopontMeneger)
                {
                    var ret = item as IdopontMeneger;
                    if (data.ID == ret._StreamId) return ret;
                }
            }

            return null;
        }

        private void button_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.comboBox_nap.Focus();
        }

        private void comboBox_nap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = this.comboBox_nap.SelectedItem as ComboBoxItem;
            this.Title = item.Content.ToString() + "i streamek:";
        }
    }
}
