using System;
using System.IO;
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
        Label lblScore, lblDiscard, lblTime;
        Timer timerTimer, timerFunc;
        Button btnRestart, btnPause, btnMusic, btnSound, btnHelp, btnArchive;
        ComboBox difficulty;
        public frmMain()
        {
            this.Size = new Size(1024, 768);
            this.Text = "ALC#EMY";
            this.StartPosition = FormStartPosition.CenterScreen;

            panel = new pnlMain(0, 0, 729);
            panel.MouseClick += panel_MouseClick;
            this.Controls.Add(panel);

            panel2 = new pnlCurrent();
            panel2.Location = new Point(826, 40);
            panel2.Size = new Size(85, 85);
            panel2.BackColor = Color.Gray;
            this.Controls.Add(panel2);

            lblScore = new Label();
            lblScore.AutoSize = true;
            lblScore.Location = new Point(780, 160);
            lblScore.Text = "Score: 0";
            lblScore.Font = new Font("Microsoft Sans Serif", 16);
            lblScore.Show();
            lblScore.Refresh();
            this.Controls.Add(lblScore);

            lblDiscard = new Label();
            lblDiscard.AutoSize = true;
            lblDiscard.Location = new Point(780, 225);
            lblDiscard.Text = "Forge capacity: 3";
            lblDiscard.Font = lblScore.Font;
            lblDiscard.Show();
            lblDiscard.Refresh();
            this.Controls.Add(lblDiscard);

            lblTime = new Label();
            lblTime.AutoSize = true;
            lblTime.Location = new Point(780, 290);
            lblTime.Text = "Time: 0";
            lblTime.Font = lblScore.Font;
            lblTime.Show();
            lblTime.Refresh();
            this.Controls.Add(lblTime);

            difficulty = new ComboBox();
            difficulty.DropDownStyle = ComboBoxStyle.DropDownList;
            difficulty.Width = 130;
            difficulty.Items.Add("Apprentice");
            difficulty.Items.Add("Journeyman");
            difficulty.Items.Add("Master");
            difficulty.Font = new Font("Microsoft Sans Serif", 14);
            difficulty.SelectedItem = "Apprentice";
            difficulty.Location = new Point(804, 350);
            difficulty.DropDown += difficulty_DropDown;
            difficulty.SelectedIndexChanged += difficulty_SelectedIndexChanged;
            this.Controls.Add(difficulty);

            btnRestart = new Button();
            btnRestart.BackgroundImage = Image.FromFile("images\\restart.png");
            btnRestart.Size = new Size(55, 55);
            btnRestart.Location = new Point(811, 475);
            btnRestart.Click += btnRestart_Click;
            this.Controls.Add(btnRestart);

            btnArchive = new Button();
            btnArchive.BackgroundImage = Image.FromFile("images\\archive.png");
            btnArchive.Size = new Size(55, 55);
            btnArchive.Location = new Point(866, 475);
            btnArchive.Click += btnArchive_Click;
            this.Controls.Add(btnArchive);

            btnPause = new Button();
            btnPause.BackgroundImage = Image.FromFile("images\\pause.png");
            btnPause.Size = new Size(55, 55);
            btnPause.Location = new Point(811, 525);
            btnPause.Click += btnPause_Click;
            this.Controls.Add(btnPause);

            btnMusic = new Button();
            btnMusic.BackgroundImage = Image.FromFile("images\\music.png");
            btnMusic.Size = new Size(55, 55);
            btnMusic.Location = new Point(811, 580); 
            btnMusic.Click += btnMusic_Click;
            this.Controls.Add(btnMusic);

            btnSound = new Button();
            btnSound.BackgroundImage = Image.FromFile("images\\sound.png");
            btnSound.Size = new Size(55, 55);
            btnSound.Location = new Point(866, 580);
            btnSound.Click += btnSound_Click;
            this.Controls.Add(btnSound);

            btnHelp = new Button();
            btnHelp.BackgroundImage = Image.FromFile("images\\help.png");
            btnHelp.Size = new Size(55, 55);
            btnHelp.Location = new Point(866, 525);
            btnHelp.Click += btnHelp_Click;
            this.Controls.Add(btnHelp);

            timerTimer = new Timer();
            timerTimer.Interval = 1;
            timerTimer.Tick += timerTimer_Tick;
            timerTimer.Start();

            timerFunc = new Timer();
            timerFunc.Interval = 20;
            timerFunc.Tick += timerFunc_Tick;
            timerFunc.Start();

            mainPlay = new SoundPlayer();
            mainPlay.SoundLocation = "sounds\\main.wav";
            mainPlay.PlayLooping();
        }

        private void panel_MouseClick(object sender, MouseEventArgs e)
        {
            panel2.Refresh(); // Cập nhật pnlCurrent mỗi lần click vào pnlMain để lấy bùa hiện tại mới
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            panel.Restart(); // Gọi hàm Restart của pnlMain để vẽ lại trò chơi
            panel2.setRune("images\\sto.png");
            panel2.Refresh(); // Đặt biểu tượng hiện tại của pnlCurrent thành viên đá
            btnPause.BackgroundImage = Image.FromFile("images\\pause.png"); // Nếu trò chơi đang tạm dừng, tự động
            // chuyển hình của nút Pause để đồng bộ với việc pnlMain.Restart() đặt IsPaused = false.
        }

        private void btnArchive_Click(object sender, EventArgs e)
        {
            if (!panel.getPause())
                btnPause_Click(this, null); // Tự động tạm dừng trò chơi khi mở bảng High Score.
            frmHighScore frmHS = new frmHighScore(panel.getMuteStatus()); // getMuteStatus để quyết định xem pnlHighScore
            // có phát tiếng cháy khi xóa dữ liệu không
            frmHS.Show();
        }

        private void btnMusic_Click(object sender, EventArgs e)
        {
            if (btnMusic.BackColor != Color.Red) // Tắt nhạc
            {
                btnMusic.BackColor = Color.Red;
                mainPlay.Stop();
            }
            else // Mở nhạc
            {
                btnMusic.BackColor = this.BackColor;
                mainPlay.PlayLooping();
            }
        }

        private void btnSound_Click(object sender, EventArgs e)
        {
            if (panel.getMuteStatus()) // Mở tiếng
            {
                panel.setMuteStatus(false);
                btnSound.BackColor = this.BackColor;
            }
            else // Tắt tiếng
            {
                panel.setMuteStatus(true);
                btnSound.BackColor = Color.Red;
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (panel.getPause()) // Nếu pnlMain đang Pause, tắt Pause đi và chuyển hình của btnPause.
            {
                panel.setPause(false);
                btnPause.BackgroundImage = Image.FromFile("images\\pause.png");
            }
            else
            {
                panel.setPause(true);
                btnPause.BackgroundImage = Image.FromFile("images\\play.png");
            }
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            if (!panel.getPause())
                btnPause_Click(this, null); // Tạm dừng trò chơi khi đang đọc hướng dẫn.
            string helpText = File.ReadAllText("help.txt"); // Đọc hướng dẫn từ file help.txt.
            MessageBox.Show(helpText);
        }

        private void timerTimer_Tick(object sender, EventArgs e)
        {
            lblTime.Text = "Time: " + panel.getTime().ToString();
            lblTime.Refresh();
        }

        private void timerFunc_Tick(object sender, EventArgs e)
        {
            lblScore.Text = "Score: " + panel.getScore().ToString();
            lblScore.Refresh();
            lblDiscard.Text = "Forge capacity: " + panel.getDiscard().ToString();
            switch (panel.getDiscard()) // Đổi chữ của lblDiscard tùy vào số lần tiêu hủy bùa còn lại của người chơi.
            {
                case 0:
                    lblDiscard.ForeColor = Color.Red;
                    lblDiscard.Text = "OVERHEATING!";
                    break;
                case -1:
                    lblDiscard.ForeColor = Color.Red;
                    lblDiscard.Text = "MELTDOWN!";
                    break;
                default:
                    lblDiscard.ForeColor = Color.Black;
                    break;
            }
        }

        private void difficulty_DropDown(object sender, EventArgs e)
        {
            btnPause_Click(this, null);
            MessageBox.Show("Changing the difficulty will start a new board, and your current records will not be archived.", "Notice");
        }

        private void difficulty_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (difficulty.SelectedIndex)
            {
                case 1:
                    panel.setDifficulty(2);
                    break;
                case 2:
                    panel.setDifficulty(3);
                    break;
                default:
                    panel.setDifficulty(1);
                    break;
            }
            btnRestart_Click(this, null); // Khi người chơi đồng ý đổi độ khó, tự khởi động lại trò chơi để áp dụng độ khó mới.
        }
    }
}
