using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace alchemy
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            frmMain frm = new frmMain();
            Application.Run(frm);
        }
    }
}
