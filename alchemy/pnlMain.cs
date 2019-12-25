using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using WMPLib;

namespace alchemy
{
    class pnlMain : Panel
    { 
        int W;
        Timer timer;
        List<Square> grid = new List<Square>();

        static Square currentRune;

        int score = 0;
        int difficulty = 1;
        int NumOfNotFilled = 81; // Đếm số ô chưa được tô màu. Giá trị mặc định 81 do bàn cơ 9x9.
        int discard; // Số lần người chơi được phép tiêu hủy bùa. Mặc định bằng 3.
        int t = 0; // Thời gian chơi.
        bool IsPaused = false, IsMuted = false, ClicksDisabled = false; // Một số giá trị bool ảnh hưởng âm thanh và
        // sự kiện click.

        WMPLib.WindowsMediaPlayer runeSound, discardSound, restoreSound, skullSound, completeSound, invalidSound, criticalSound, failSound, successSound;

        public int getScore()
        {
            return score;
        }

        public int getDiscard()
        {
            return discard;
        }

        public int getTime()
        {
            return t;
        }

        public void setDifficulty(int diff)
        {
            difficulty = diff;
        }

        public bool getPause()
        {
            return IsPaused;
        }

        public void setPause(bool status)
        {
            IsPaused = status;
        }

        public bool getMuteStatus()
        {
            return IsMuted;
        }

        public void setMuteStatus(bool status)
        {
            IsMuted = status;
        }

        static public Square getCurrentRune()
        {
            return currentRune;
        }

        public pnlMain(int x, int y, int width)
        {
            DoubleBuffered = true;
            this.Location = new Point(x, y);
            W = width;
            this.Size = new Size(width, width);
            this.MouseClick += pnlMain_MouseClick;
            this.Paint += pnlMain_Paint;

            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += timer_Tick;
            discard = 3;

            // Tạo các WMP từ WMPLib để phát ra âm thanh. Hàm stop() được dùng để các WMP không chạy khi bắt đầu form.

            runeSound = new WMPLib.WindowsMediaPlayer();
            runeSound.URL = "sounds\\rune_placed.wav";
            runeSound.controls.stop();

            discardSound = new WMPLib.WindowsMediaPlayer();
            discardSound.URL = "sounds\\rune_discarded.wav";
            discardSound.controls.stop();

            restoreSound = new WMPLib.WindowsMediaPlayer();
            restoreSound.URL = "sounds\\discard_restore.wav";
            restoreSound.controls.stop();

            skullSound = new WMPLib.WindowsMediaPlayer();
            skullSound.URL = "sounds\\skull.wav";
            skullSound.controls.stop();

            completeSound = new WMPLib.WindowsMediaPlayer();
            completeSound.URL = "sounds\\row_column_complete.wav";
            completeSound.controls.stop();

            invalidSound = new WMPLib.WindowsMediaPlayer();
            invalidSound.URL = "sounds\\invalid.wav";
            invalidSound.controls.stop();

            criticalSound = new WMPLib.WindowsMediaPlayer();
            criticalSound.URL = "sounds\\critical.wav";
            criticalSound.settings.setMode("loop", true); // Âm thanh nguy cấp được đặt chế độ vòng lặp
            criticalSound.controls.stop();

            failSound = new WMPLib.WindowsMediaPlayer();
            failSound.URL = "sounds\\fail.wav";
            failSound.controls.stop();

            successSound = new WMPLib.WindowsMediaPlayer();
            successSound.URL = "sounds\\success.wav";
            successSound.controls.stop();
        }

        public void Restart() // Hàm khởi động lại game.
        {
            this.Paint += pnlMain_Paint; // Tô lại pnlMain.
            foreach (Square s in grid) // Xóa toàn bộ các bùa khỏi bàn cờ và đặt lại Square.IsFilled = false.
            {
                s.setRune(string.Empty);
                s.setFillStatus(false);
            }
            discard = 3;
            score = 0;
            t = 0;
            IsPaused = false;
            timer.Stop();
            ClicksDisabled = false; // Khi thắng game, ClicksDisabled = true để người chơi không thể đặt thêm bùa lên
            // bàn cở. Restart đặt lại thuộc tính này thành false để người dùng có thể chơi lại.
            NumOfNotFilled = 81;
            criticalSound.controls.stop(); // Trường hợp người chơi bắt đầu bàn cờ mới trong khi game cũ đang trong tình
            // trạng sắp thua, câu lệnh này sẽ buộc WMP chi phối âm thanh sắp thua phải tắt.
            this.Invalidate();
        }

        private void GenerateGrid() // Hàm tạo bàn cờ.
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 1; j <= 9; j++)
                {
                    Square sqr = new Square((W / 9) * (j - 1), (W / 9) * i, W / 9, string.Empty);
                    grid.Add(sqr);
                }
            }
        }

        private Square generateRndRune() // Hàm tạo bùa bất kỳ.
        {
            Random rnd = new Random();
            int Type;
            string rune_Type;
            string newRune = string.Empty;
            int Col;
            switch (difficulty) // Tùy vào độ khó mà giới hạn về loại bùa sẽ khác nhau.
            {
                case 2:
                    Type = rnd.Next(0, 14);
                    Col = rnd.Next(0, 7);
                    break;
                case 3:
                    Type = rnd.Next(0, 19);
                    Col = rnd.Next(0, 10);
                    break;
                default:
                    Type = rnd.Next(0, 7);
                    Col = rnd.Next(0, 4);
                    break;
            }
            switch (Type) { // Tạo loại bùa dựa trên kết quả của Type. Một số giá trị sẽ chỉ được sử dụng tại những cấp độ cao hơn.
                case 1:
                    rune_Type = "ari";
                    break;
                case 2:
                    rune_Type = "tau";
                    break;
                case 3:
                    rune_Type = "gem";
                    break;
                case 4:
                    rune_Type = "can";
                    break;
                case 5:
                    rune_Type = "leo";
                    break;
                case 6:
                    rune_Type = "vir";
                    break;
                case 7:
                    rune_Type = "lib";
                    break;
                case 8:
                    rune_Type = "sco";
                    break;
                case 9:
                    rune_Type = "sag";
                    break;
                case 10:
                    rune_Type = "cap";
                    break;
                case 11:
                    rune_Type = "aqu";
                    break;
                case 12:
                    rune_Type = "pis";
                    break;
                case 13:
                    rune_Type = "sku";
                    break;
                case 14:
                    rune_Type = "fir";
                    break;
                case 15:
                    rune_Type = "wat";
                    break;
                case 16:
                    rune_Type = "ear";
                    break;
                case 17:
                    rune_Type = "air";
                    break;
                case 18:
                    rune_Type = "spi";
                    break;
                default:
                    rune_Type = "sto";
                    break;
            }
            // Dựng string newRune tạo đường dẫn đến file hình trong folder images. Các bùa bình thường có format loại_sốmàu.png, riêng 2 bùa đặc biệt mang format loại.png.
            if ((rune_Type != "sto") && (rune_Type != "sku"))
                newRune = string.Format("images\\" + rune_Type + "\\" + rune_Type + "_" + Col.ToString() + ".png");
            else
                newRune = string.Format("images\\" + rune_Type + ".png");
            return new Square(0, 0, W / 9, newRune);
        }

        private void pnlMain_Paint(object sender, PaintEventArgs e)
        {
            GenerateGrid();
            Graphics g = e.Graphics;
            foreach (Square s in grid)
            {
                Pen p = new Pen(Brushes.Black, 2.0F);
                g.DrawRectangle(p, s.Rectangle());
                g.FillRectangle(Brushes.Gray, s.Rectangle());
            }
            currentRune = generateRndRune(); // Khi panel được tô màu, đặt bùa hiện tại là viên đá.
            currentRune.setRune("images\\sto.png");
        }

        private void PaintCorrectRune(Square sqr, Square current) // sqr là ô hiện tại được chọn, current là bùa hiện tại đang được sử dụng.
        {
            sqr.setRune(current.getRune());
            Graphics g = CreateGraphics();
            if (!sqr.getFillStatus()) // Nếu !IsFilled, đặt IsFilled = true và tô background màu vàng, sau đó trừ NumOfNotFilled.
            {
                sqr.setFillStatus(true);
                g.FillRectangle(Brushes.Goldenrod, sqr.getX(), sqr.getY(), sqr.Rectangle().Width - 1, sqr.Rectangle().Width - 1);
                NumOfNotFilled--;
            }
            g.DrawImage(new Bitmap(current.getRune()), sqr.Rectangle()); // "Ếm" bùa cho ô vuông. Dòng này phải đi sau FillRectangle, nếu không
            // nền vàng sẽ tô đè lên bùa.
            if (!IsMuted) // Nếu !IsMuted, chơi file âm thanh ếm bùa.
            {
                runeSound.controls.stop();
                runeSound.controls.play();
            }
        }

        private Square checkRuneIntegrity(Square newRune, Square prevRune) // So sánh bùa mới tạo với bùa hiện tại. 
        {
            int NumOfStones = 0;
            int MaxStones;
            int Emp = 0;

            if (difficulty == 3) // Độ khó cao nhất cho phép 3 viên đá, còn lại 2
                MaxStones = 3;
            else
                MaxStones = 2;

            for (int i = 0; i <= 80; i++) // Đếm số viên đá đang có trên bàn cờ. Nếu số viên đá bằng số viên tối đa cho phép, dừng vòng lập.
            {
                if (grid[i].getType() == "sto")
                {
                    NumOfStones++;
                }
                if (NumOfStones == MaxStones)
                    break;
            }

            for (int i = 0; i <= 80; i++) // Đếm số ô trống trên bàn cờ.
            {
                if (grid[i].getRune() == string.Empty)
                    Emp++;
            }

            do
            {
                newRune = generateRndRune();
            } while (((newRune.getType() == prevRune.getType()) && (newRune.getColor() == prevRune.getColor()))
                    || ((newRune.getType() == "sto") && (NumOfStones == MaxStones))
                    || (newRune.getType() == "sku") && (Emp == 80));
            // Bắt hệ thống tạo lại bùa mới nếu bùa mới trò chơi tạo ra cùng loại với màu của bùa hiện tại, hoặc
            // nếu bùa mới là viên đá khi bàn cờ đã đủ số viên đá tối đa, hoặc nếu trò chơi tạo ra sọ trong khi bàn
            // cờ chỉ có 1 ô có bùa (tức 80 ô trống).
            return newRune;
        }

        private Square OnCorrectRune(Square sqr, Square curRune) // Sự kiện xảy ra nếu ô muốn ếm bùa thỏa điều kiện.
        {
            PaintCorrectRune(sqr, curRune); // Tô ô hiện tại với curRune.
            Square prevRune = curRune; // Tạo ô prevRune mới có giá trị bằng curRune.
            curRune = checkRuneIntegrity(curRune, prevRune); // Tạo bùa mới từ curRune, đối chiếu với prevRune hoặc bàn cờ xem có hợp lệ hay không.
            if (discard < 3) // Nếu người chơi dưới 3 lần hủy bùa, đặt bùa mới sẽ cho thêm 1 hủy nữa. Phát tiếng "bù số lần tiêu hủy" nếu người chơi không tắt tiếng.
            {
                discard = discard + 1;
                if (IsMuted == false)
                {
                    restoreSound.controls.stop();
                    restoreSound.controls.play();
                }
            }
            return curRune;
        }

        private void pnlMain_MouseClick(object sender, MouseEventArgs e)
        {
            if ((!IsPaused) && (!ClicksDisabled))
            {
                Graphics g = CreateGraphics();
                if (e.Button == MouseButtons.Left)
                {
                    Point cursorPos = this.PointToClient(Cursor.Position);
                    for (int i = 0; i <= 80; i++)
                    {
                        if (grid[i].Rectangle().Contains(cursorPos))
                        {
                            int scoreMod = 0;
                            if (!grid[i].getFillStatus()) // Điểm nhận được khi ếm bùa ô. Điểm phụ thuộc vào độ khó và liệu ô đó có phủ vàng không.
                            {
                                switch (difficulty)
                                {
                                    case 2:
                                        scoreMod = 10;
                                        break;
                                    case 3:
                                        scoreMod = 20;
                                        break;
                                    default:
                                        scoreMod = 5;
                                        break;
                                }
                            }
                            else
                            {
                                if (grid[i].getFillStatus())
                                {
                                    switch (difficulty)
                                    {
                                        case 2:
                                            scoreMod = 4;
                                            break;
                                        case 3:
                                            scoreMod = 8;
                                            break;
                                        default:
                                            scoreMod = 2;
                                            break;
                                    }
                                }
                            }
                            int NumOfEmpty = 0;
                            for (int k = 0; k <= 80; k++)
                            {
                                if (grid[k].getRune() == string.Empty)
                                    NumOfEmpty++;
                            } // Nếu toàn bộ bàn cờ không có bùa nhưng vẫn còn ô chưa tô vàng, bất kỳ lá bùa nào ếm lên bảng sẽ biến thành một viên đá, bảo đảm game còn chơi được.
                            if ((NumOfEmpty == 81) && (NumOfNotFilled > 0))
                            {
                                currentRune.setRune("images\\sto.png");
                                PaintCorrectRune(grid[i], currentRune);
                                Square prevRune = currentRune;
                                currentRune = checkRuneIntegrity(currentRune, prevRune);
                                score = score + scoreMod;
                                timer.Start(); // Bắt đầu đồng hồ tính giờ, thường sau khi ếm bùa đầu tiên.
                            }
                            else
                            {
                                if (currentRune.getType() != "sku")
                                {
                                    if (grid[i].getType() == string.Empty) // Nếu bùa hiện tại không phải dạng sọ, kiểm tra ô có trống không.
                                    {
                                        List<Square> adjSqr = new List<Square>(); // Tạo danh sách chứa các ô liền kề ô được chọn.
                                        for (int k = 0; k <= 80; k++)
                                        {
                                            if (Rectangle.Intersect(grid[k].Rectangle(), grid[i].Rectangle()) != Rectangle.Empty) // Chỉ xét các ô tiếp xúc với ô được chọn.
                                            {
                                                if ((k == i) || (k == i - 10) || (k == i + 10) || (k == i - 8) || (k == i + 8))
                                                    continue;
                                                else
                                                {
                                                    adjSqr.Add(grid[k]); // Nếu các ô liền kề không phải tiếp xúc chéo hoặc là chính ô được chọn, thêm vào adjSqr.
                                                }
                                            }
                                        }
                                        bool AdjExists = false;
                                        foreach (Square s in adjSqr) // Kiểm tra xem ô được chọn có tiếp xúc với bùa có sẵn không.
                                        {
                                            if (s.getRune() != string.Empty)
                                            {
                                                AdjExists = true;
                                                break;
                                            }
                                        }
                                        if (AdjExists)
                                        {
                                            if (currentRune.getType() == "sto") // Nếu bùa hiện tại là đá, cho phép đặt nếu liền kề ít nhất một bùa khác.
                                            {
                                                currentRune = OnCorrectRune(grid[i], currentRune);
                                                score = score + scoreMod;
                                            }
                                            else
                                            {
                                                bool Valid = true;
                                                int multiplier = 0;
                                                foreach (Square s in adjSqr)
                                                {
                                                    if (s.getRune() == string.Empty)
                                                        continue;
                                                    if ((s.getType() != "sto") && (s.getType() != currentRune.getType()) && (s.getColor() != currentRune.getColor()))
                                                    {
                                                        Valid = false;
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        multiplier++;
                                                    }
                                                    // Lờ các ô trống. Nếu trong các ô tiếp xúc với ô được chọn có một ô không tương thích (khác loại, khác màu và không phải là
                                                    // đá), xem như không hợp lệ và kết thúc vòng lặp. Ngược lại, multiplier +1. Int multiplier sẽ được dùng để tính điểm.
                                                }
                                                if (Valid) // Nếu hợp lệ, ếm bùa lên ô và người chơi nhận điểm tương ứng với độ khó. Điểm có thể được x2, x3 hay x4 tùy vào số ô có bùa tiếp xúc với nó.
                                                {
                                                    currentRune = OnCorrectRune(grid[i], currentRune);
                                                    score = score + scoreMod * multiplier;
                                                }
                                                else
                                                {
                                                    if (!IsMuted)
                                                        invalidSound.controls.stop();
                                                        invalidSound.controls.play();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (!IsMuted)
                                                invalidSound.controls.stop();
                                                invalidSound.controls.play();
                                        }
                                        adjSqr.Clear();
                                    }
                                    else
                                    {
                                        if (!IsMuted)
                                            invalidSound.controls.stop();
                                            invalidSound.controls.play();
                                    }
                                }
                                else
                                {
                                    if (currentRune.getType() == "sku")
                                    {
                                        if (grid[i].getType() != string.Empty) // Nếu bùa hiện tại là sọ, kiểm tra xem ô người chơi chọn có bùa không, do bùa sọ có chức năng xóa bùa trên bảng.
                                        {
                                            grid[i].setRune(string.Empty);
                                            g.FillRectangle(Brushes.Goldenrod, grid[i].getX(), grid[i].getY(), grid[i].Rectangle().Width - 1, grid[i].Rectangle().Width - 1);
                                            Square prevRune = currentRune;
                                            currentRune = checkRuneIntegrity(currentRune, prevRune);
                                            if (!IsMuted)
                                            {
                                                skullSound.controls.stop();
                                                skullSound.controls.play();
                                            }
                                            if (discard < 3)
                                                discard++;
                                        }
                                        else
                                        {
                                            if (!IsMuted)
                                                invalidSound.controls.stop();
                                                invalidSound.controls.play();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // Các vòng lập dưới đây kiểm tra xem có hàng hay cột nào có đầy đủ bùa chưa. Nếu có, hàng/cột đó được
                    // xóa hết bùa và người chơi nhận một lượng điểm lớn.
                    for (int i = 0; i < 9; i++)
                    {
                        int OccupiedSquares = 0;
                        for (int j = i; j <= 80; j = j + 9)
                        {
                            if (grid[j].getType() != string.Empty)
                                OccupiedSquares++;
                        }
                        if (OccupiedSquares == 9)
                        {
                            discard = 3;
                            switch (difficulty)
                            {
                                case 2:
                                    score = score + 90;
                                    break;
                                case 3:
                                    score = score + 180;
                                    break;
                                default:
                                    score = score + 45;
                                    break;
                            }
                            for (int j = i; j <= 80; j = j + 9)
                            {
                                grid[j].setRune(string.Empty);
                                g.FillRectangle(Brushes.Goldenrod, grid[j].getX(), grid[j].getY(), grid[j].Rectangle().Width - 1, grid[j].Rectangle().Width - 1);
                            }
                            if (!IsMuted)
                                completeSound.controls.stop();
                                completeSound.controls.play();
                        }
                    }
                    for (int i = 0; i <= 72; i = i + 9)
                    {
                        int OccupiedSquares = 0;
                        for (int k = i; k <= i + 8; k++)
                        {
                            if (grid[k].getType() != string.Empty)
                                OccupiedSquares++;
                        }
                        if (OccupiedSquares == 9)
                        {
                            discard = 3;
                            switch (difficulty)
                            {
                                case 2:
                                    score = score + 90;
                                    break;
                                case 3:
                                    score = score + 180;
                                    break;
                                default:
                                    score = score + 45;
                                    break;
                            }
                            for (int k = i; k <= i + 8; k++)
                            {
                                grid[k].setRune(string.Empty);
                                g.FillRectangle(Brushes.Goldenrod, grid[k].getX(), grid[k].getY(), grid[k].Rectangle().Width - 1, grid[k].Rectangle().Width - 1);
                            }
                            if (!IsMuted)
                                completeSound.controls.stop();
                                completeSound.controls.play();
                        }
                    }
                    if (discard > 0)
                    {
                        if (!IsMuted)
                            criticalSound.controls.stop();
                    }
                    if (NumOfNotFilled == 0) // Nếu toàn bộ bảng được vàng hóa, dừng thời gian, tạo form nhập điểm với Win = true
                                             // ClicksDisabled có công dụng ngăn người chơi thay đổi bàn cờ.
                    {
                        timer.Stop();
                        if (!IsMuted)
                        {
                            successSound.controls.play();
                        }
                        frmGameOver frmGO = new frmGameOver(true, score, t, difficulty);
                        frmGO.Show();
                        ClicksDisabled = true;
                    }
                }
                else
                {
                    if (e.Button == MouseButtons.Right) // Nếu người chơi nhấp chuột phải, bắt đầu sự kiện hủy bùa hiện tại.
                    {
                        if (NumOfNotFilled != 81) // Không cho người chơi hủy bùa nếu tất cả 81 ô đều xám (chưa bắt đầu trò chơi)
                        {
                            if (!IsMuted)
                            {
                                discardSound.controls.stop();
                                discardSound.controls.play();
                            }
                            Square prevRune = currentRune;
                            currentRune = checkRuneIntegrity(currentRune, prevRune);
                            discard--;
                            if (discard == 0) // Khi người chơi hết số lần hủy cho phép, chơi âm thanh nguy cấp.
                            {
                                if (!IsMuted)
                                {
                                    criticalSound.controls.play();
                                }
                            }
                            if (discard < 0)
                            {
                                timer.Stop();
                                criticalSound.controls.stop();
                                if (!IsMuted)
                                {
                                    failSound.controls.play();
                                }
                                frmGameOver frmGO = new frmGameOver(false, score, t, difficulty);
                                frmGO.Show();
                                ClicksDisabled = true;
                            }
                        }
                        else
                        {
                            if (!IsMuted)
                                invalidSound.controls.stop();
                                invalidSound.controls.play();
                        }

                    }
                }
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (!IsPaused)
                t++;
        }
    }
}