using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Media;

namespace alchemy
{
    class frmMain : Form
    {
        SoundPlayer mainPlay;
        pnlMain panel;
        pnlCurrent panel2;
        Button btnRestart;
        Label lblScore, lblDiscard, lblTime;
        MainMenu menu;
        Timer timerTimer, timerScore;
        int score = 0;
        public frmMain()
        {
            this.Size = new Size(1024, 768);
            this.Text = "ALC#EMY";
            this.StartPosition = FormStartPosition.CenterScreen;

            panel = new pnlMain(0, 0, 630);
            panel.MouseClick += panel_MouseClick;
            this.Controls.Add(panel);

            panel2 = new pnlCurrent();
            panel2.Location = new Point(700, 20);
            panel2.Size = new Size(74, 74);
            this.Controls.Add(panel2);

            lblScore = new Label();
            lblScore.AutoSize = true;
            lblScore.Location = new Point(800, 200);
            lblScore.Text = "Score: 0";
            lblScore.Show();
            lblScore.Refresh();
            this.Controls.Add(lblScore);

            lblDiscard = new Label();
            lblDiscard.AutoSize = true;
            lblDiscard.Location = new Point(800, 250);
            lblDiscard.Text = "Discarded: 3";
            lblDiscard.Show();
            lblDiscard.Refresh();
            this.Controls.Add(lblDiscard);

            lblTime = new Label();
            lblTime.AutoSize = true;
            lblTime.Location = new Point(800, 300);
            lblTime.Text = "Time: 0";
            lblTime.Show();
            lblTime.Refresh();
            this.Controls.Add(lblTime);

            btnRestart = new Button();
            btnRestart.Text = "New Game";
            btnRestart.Location = new Point(800, 350);
            btnRestart.AutoSize = true;
            btnRestart.Click += btnRestart_Click;;
            this.Controls.Add(btnRestart);

            timerTimer = new Timer();
            timerTimer.Interval = 1;
            timerTimer.Tick += timerTimer_Tick;
            timerTimer.Start();

            timerScore = new Timer();
            timerScore.Interval = 20;
            timerScore.Tick += timerScore_Tick;
            timerScore.Start();

            mainPlay = new SoundPlayer();
            mainPlay.SoundLocation = "sounds\\main.wav";
            mainPlay.PlayLooping();

            menu = new MainMenu();
            MenuItem Game = menu.MenuItems.Add("&Game");
            Game.MenuItems.Add(new MenuItem("&New Game"));
            Game.MenuItems.Add(new MenuItem("&Difficulty"));
            Game.MenuItems.Add(new MenuItem("&Music"));
            Game.MenuItems.Add(new MenuItem("&Sound"));
            Game.MenuItems.Add(new MenuItem("&Exit"));
            MenuItem Extras = menu.MenuItems.Add("&Extras");
            Extras.MenuItems.Add(new MenuItem("&Help!"));
            Extras.MenuItems.Add(new MenuItem("&High Scores"));
            Extras.MenuItems.Add(new MenuItem("&About..."));
            this.Menu = menu;
        }

        private void panel_MouseClick(object sender, MouseEventArgs e)
        {
            lblDiscard.Text = "Discarded: " + panel.getDiscard().ToString();
            panel2.Refresh();
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            panel.Restart();
            lblScore.Refresh();
            lblDiscard.Refresh();
            panel2.setRune("images\\sto.png");
            panel2.Refresh();
        }

        private void timerTimer_Tick(object sender, EventArgs e)
        {
            lblTime.Text = "Time: " + panel.getTime().ToString();
            lblTime.Refresh();
        }

        private void timerScore_Tick(object sender, EventArgs e)
        {
            while (score < panel.getScore())
            {
                score++;
                lblScore.Text = "Score: " + score.ToString();
                lblScore.Refresh();
            }
        }
    }
}
