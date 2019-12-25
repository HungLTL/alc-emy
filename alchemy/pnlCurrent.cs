using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace alchemy
{
    class pnlCurrent : Panel
    {
        Square current;
        public pnlCurrent()
        {
            DoubleBuffered = true;
            this.Size = new Size(200, 100);
            this.BorderStyle = BorderStyle.Fixed3D;
            this.Paint += pnlCurrent_Paint;
        }
        private void pnlCurrent_Paint (object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            current = pnlMain.getCurrentRune();
            g.DrawRectangle(Pens.Transparent, current.Rectangle());
            g.DrawImage(new Bitmap(current.getRune()), current.Rectangle());
        }
        public void setRune(string type)
        {
            current.setRune(type);
        }
    }
}
