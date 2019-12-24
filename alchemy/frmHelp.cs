using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AxWMPLib;

namespace alchemy
{
    public partial class frmHelp : Form
    {
        public frmHelp()
        {
            InitializeComponent();
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            cmbHelp.Items.Add("Overview");
            cmbHelp.Items.Add("Runes");
            cmbHelp.Items.Add("Stones");
            cmbHelp.Items.Add("Skulls");
            cmbHelp.Items.Add("Forge");
            cmbHelp.SelectedIndexChanged += cmbHelp_SelectedIndexChanged;
        }

        private void cmbHelp_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(cmbHelp.SelectedIndex)
            {
                case 1:
                    axWindowsMediaPlayer1.URL = "help\\runes_basic.mp4";
                    break;
                case 2:
                    axWindowsMediaPlayer1.URL = "help\\runes_stone.mp4";
                    break;
                case 3:
                    axWindowsMediaPlayer1.URL = "help\\runes_skull.mp4";
                    break;
                case 4:
                    axWindowsMediaPlayer1.URL = "help\\forge.mp4";
                    break;
                default:
                    axWindowsMediaPlayer1.URL = "help\\overview.mp4";
                    break;

            }
        }
    }
}
