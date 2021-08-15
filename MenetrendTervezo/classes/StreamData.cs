using System;

namespace MenetrendTervezo.classes
{
    public class StreamData
    {
        private static int num = 0;

        public StreamData(string idopont, string bolt, string cim, string kepUrl, string kepMeretezes, string[] kepek)
        {
            this.Idopont = idopont;
            this.Bolt = bolt;
            this.Cim = cim;
            this.KepUrl = kepUrl;
            this.KepMeretezes = kepMeretezes;
            this.Kepek = kepek;
            this.ID = "stream" + DateTime.Now.ToString("MMddHHmmssFFFFFFF") + (++num);
        }

        public string Idopont { get; set; }
        public string Bolt { get; set; }
        public string Cim { get; set; }
        public string KepUrl { get; set; }
        public string KepMeretezes { get; set; }
        public string ID { get; private set; }
        public string[] Kepek { get; set; }
    }
}
