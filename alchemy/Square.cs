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
        int X;
        int Y;
        int Width;
        string rune; //Chứa đường dẫn đến hình ảnh các bùa. Sẽ được sử dụng cho DrawImage.

        bool IsFilled; // Kiểm tra ô bao giờ được ếm bùa chưa. Ảnh hưởng màu sắc và điểm. Mặc định
        // false, tức chưa từng được ếm bùa.

        public Square(int x, int y, int w, string r)
        {
            X = x;
            Y = y;
            Width = w;
            rune = r;
            IsFilled = false;
        }

        public int getX()
        {
            return X;
        }

        public int getY()
        {
            return Y;
        }

        public string getRune()
        {
            return rune;
        }

        public void setRune(string newRune)
        {
            rune = newRune;
        }

        public bool getFillStatus()
        {
            return IsFilled;
        }

        public void setFillStatus(bool stat)
        {
            IsFilled = stat;
        }

        public string getType() // Trả về loại bùa ô vuông đang chứa. Nếu ô rỗng, trả về chuỗi rỗng.
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

        public int getColor() // Các màu khác nhau được đánh dấu bằng số 0-9. Nếu ô không có bùa, hoặc nếu là bùa
                              // đặc biệt, trả về giá trị -1 vốn không có liên quan với màu gì hết.
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

        public Rectangle Rectangle() // Chiếu lớp Square thành Rectangle để sử dụng một số hàm trong lớp Rectangle.
        {
            return new Rectangle(X, Y, Width, Width);
        }
    }
}
