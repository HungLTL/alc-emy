using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.ComponentModel;
using System.Media;

namespace alchemy
{
    class Loading : Form
    {
        ProgressBar pbar;
        BackgroundWorker bgw;
        SoundPlayer loading;
        public Loading()
        {
            this.Size = new Size(640, 480);
            this.ControlBox = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.Gray;
            this.Paint += Loading_Paint;

            pbar = new ProgressBar();
            pbar.Size = new Size(this.Width - 50, 30);
            pbar.Location = new Point(26, 420);
            pbar.Style = ProgressBarStyle.Continuous;
            pbar.ForeColor = Color.Goldenrod;
            pbar.BackColor = Color.Gray;
            this.Controls.Add(pbar);

            bgw = new BackgroundWorker();
            bgw.DoWork += bgw_DoWork;
            bgw.ProgressChanged += bgw_ProgressChanged;
            bgw.RunWorkerCompleted += bgw_RunWorkerCompleted;
            bgw.WorkerReportsProgress = true;
            bgw.RunWorkerAsync();

            loading = new SoundPlayer();
            loading.SoundLocation = "sounds\\loading.wav";
            loading.Play();
        }
        private void Loading_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.DrawImage(new Bitmap("images\\main.png"), new Point(0, 0));
        }
        private void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0;i<=100;i++)
            {
                bgw.ReportProgress(i);
                Thread.Sleep(98);
            }
        }
        private void bgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbar.Value = e.ProgressPercentage;
        }
        private void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Close();
        }
    }
}