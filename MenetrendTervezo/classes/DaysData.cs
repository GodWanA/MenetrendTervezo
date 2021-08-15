using System;
using System.Collections.Generic;
using System.Linq;

namespace MenetrendTervezo.classes
{
    public class DaysData
    {
        public string Nev { get; set; }
        public List<StreamData> Streamek { get; set; }
        public string ID { get; private set; }
        public int Index { get; set; }

        public DaysData(string nev, List<StreamData> streamek, int index)
        {
            this.Nev = nev;
            this.Streamek = streamek;
            this.ID = this.Nev + DateTime.Now.ToString("MMddHHmmss");
            this.Index = index;
        }

        public DaysData() { }

        public override string ToString()
        {
            return this.Nev + " (" + this.Streamek.Count + ")";
        }

        public DaysData Clone()
        {
            var uj = new DaysData();

            uj.Nev = this.Nev;
            uj.Streamek = this.Streamek;
            uj.ID = this.ID;
            uj.Index = this.Index;

            return uj;
        }

        public bool BenneVan(string id)
        {
            if (this.Streamek.Where(x => x.ID == id).FirstOrDefault() != null) return true;
            return false;
        }
    }
}
