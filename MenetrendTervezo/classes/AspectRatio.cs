using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenetrendTervezo.classes
{
    public class AspectRatio
    {
        public AspectRatio(int horizontal, int vertical)
        {
            Horizontal = horizontal;
            Vertical = vertical;
        }

        public int Horizontal { get; set; }
        public int Vertical { get; set; }

        public override string ToString()
        {
            return this.Horizontal + " : " + this.Vertical;
        }
    }
}
