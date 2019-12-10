using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace alchemy
{
    class Square
    {
        public int X { get; set; }
        public int Y { get; set; }
        int Width { get; set; }
        public string rune { get; set; }

        public bool IsFilled { get; set; }

        public Square(int x, int y, int w, string r)
        {
            X = x;
            Y = y;
            Width = w;
            rune = r;
            IsFilled = false;
        }

        public string getType()
        {
            string type = string.Empty;
            if (rune.Contains("ari"))
                type = "ari";
            if (rune.Contains("tau"))
                type = "tau";
            if (rune.Contains("gem"))
                type = "gem";
            if (rune.Contains("can"))
                type = "can";
            if (rune.Contains("leo"))
                type = "leo";
            if (rune.Contains("vir"))
                type = "vir";
            if (rune.Contains("sag"))
                type = "sag";
            if (rune.Contains("lib"))
                type = "lib";
            if (rune.Contains("sco"))
                type = "sco";
            if (rune.Contains("cap"))
                type = "cap";
            if (rune.Contains("aqu"))
                type = "aqu";
            if (rune.Contains("pis"))
                type = "pis";
            if (rune.Contains("air"))
                type = "air";
            if (rune.Contains("fir"))
                type = "fir";
            if (rune.Contains("ear"))
                type = "ear";
            if (rune.Contains("spi"))
                type = "spi";
            if (rune.Contains("wat"))
                type = "wat";
            if (rune.Contains("sku"))
                type = "sku";
            if (rune.Contains("sto"))
                type = "sto";
            return type;
        }

        public int getColor()
        {
            int col = -1;
            if ((rune == string.Empty) || (rune == "sku.png") || (rune == "sto.png"))
                col = -1;
            if (rune.Contains("1"))
                col = 1;
            if (rune.Contains("2"))
                col = 2;
            if (rune.Contains("3"))
                col = 3;
            if (rune.Contains("4"))
                col = 4;
            if (rune.Contains("5"))
                col = 5;
            if (rune.Contains("6"))
                col = 6;
            if (rune.Contains("7"))
                col = 7;
            if (rune.Contains("8"))
                col = 8;
            if (rune.Contains("9"))
                col = 9;
            if (rune.Contains("0"))
                col = 0;
            return col;
        }

        public Rectangle Rectangle()
        {
            return new Rectangle(X, Y, Width, Width);
        }

        public void Location(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
