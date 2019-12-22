using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace alchemy
{
    class frmGameOver : Form
    {
        TextBox boxName;
        Label lblText;
        Button btnSubmit;

        int Score;
        int Time;
        int Difficulty;
        bool Win;

        // Khi pnlMain đạt điều kiện kết thúc trò chơi, pnlMain kêu gọi hàm khởi tạo frmGameOver có chức năng giống
        // một MessageBox sẽ xuất hiện, hiển thị điểm, thời gian và yêu cầu người chơi nhập tên vào. 

        public frmGameOver(bool win, int score, int time, int diff) // "win" nhận kết quả trò chơi từ pnlMain để quyết
                                                                    // định giá trị cột Result, và diff để quyết định
                                                                    // giá trị cột Difficulty.
        {
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            Win = win;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.AutoSize = true;
            if (win)
                this.Text = "Success!";
            else
                this.Text = "Failure!";

            lblText = new Label();
            lblText.Location = new Point(10, 15);
            lblText.AutoSize = true;
            string status = string.Empty; // Dòng chữ của lblText thể hiện kết quả của người chơi tùy vào giá trị Win.
            if (win)
            {
                status = "Board successfully completed! The Council shall reward you handsomely for your efforts.\n\n";
            }
            else
            {
                status = "You have caused a meltdown. The Council will remember this failure, and punish you accordingly.\n\n";
            }
            Score = score;
            Time = time;
            Difficulty = diff;
            lblText.Text = status + "Score: " + Score.ToString() + "\n\nTime elapsed: " + (TimeSpan.FromSeconds(Time)).ToString() /* Chuyển giá trị giây của trò chơi sang TimeSpan */ + "\n\nPlease enter your name so we can archive your records.";
            lblText.Show();
            this.Controls.Add(lblText);

            boxName = new TextBox();
            boxName.Width = this.Width - 35;
            boxName.Location = new Point(10, 120);
            boxName.KeyPress += boxName_KeyPress;
            this.Controls.Add(boxName);

            btnSubmit = new Button();
            btnSubmit.AutoSize = true;
            btnSubmit.Text = "Submit";
            btnSubmit.Location = new Point(10, 150);
            btnSubmit.Click += btnSubmit_Click;
            this.Controls.Add(btnSubmit);
        }

        private void boxName_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar); // Chỉ cho phép nhập chữ số. Hạn chế gây sự cố ghi dữ liệu nếu người dùng lỡ sử dụng '$'.
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (boxName.Text != null) // Kiểm tra boxName có trống hay không. Nếu không trống, dữ liệu sẽ không được nhập vào score.txt.
            {
                Player player = new Player(boxName.Text, Score, Time, Difficulty, Win); // Tạo lớp Player chứa toàn bộ thông tin người chơi.
                if (!File.Exists("dat\\score.txt")) //Kiểm tra xem có tồn tại score.txt không.
                {
                    using (StreamWriter output = new StreamWriter("dat\\score.txt", true)) //Nếu không tồn tại, tạo file score.txt mới, với dòng đâu tiên mang tên các cột.
                    {
                        File.SetAttributes("dat\\score.txt", FileAttributes.Hidden | FileAttributes.ReadOnly | FileAttributes.Encrypted);
                        output.WriteLine("Name$Date$Difficulty$Score$Time$Result");
                    }
                }
                FileInfo score = new FileInfo("dat\\score.txt");
                score.IsReadOnly = false;
                using (StreamWriter output = new StreamWriter("dat\\score.txt", true))
                {
                    output.WriteLine(player.writePlayer()); // Gọi writePlayer của lớp Player để viết thông tin người chơi vào score.txt.
                }
                score.IsReadOnly = true;
                this.Close();
            }
        }
    }
}
