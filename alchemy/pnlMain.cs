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
        int NumOfNotFilled = 81;
        int discard;
        int t = 0;

        WMPLib.WindowsMediaPlayer runeSound, discardSound, restoreSound, skullSound, completeSound, invalidSound;

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
        }

        public void Restart()
        {
            this.Paint += pnlMain_Paint;
            foreach (Square s in grid)
            {
                s.rune = string.Empty;
                s.IsFilled = false;
            }
            discard = 3;
            score = 0;
            t = 0;
            timer.Stop();
            NumOfNotFilled = 81;
            this.Invalidate();
        }

        private void GenerateGrid()
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

        private Square generateRndRune()
        {
            Random rnd = new Random();
            int Type;
            string rune_Type;
            string newRune = string.Empty;
            int Col;
            switch (difficulty)
            {
                case 2:
                    Type = rnd.Next(0, 13);
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
            switch (Type) {
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
                    rune_Type = "air";
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
                    rune_Type = "spi";
                    break;
                case 18:
                    rune_Type = "sku";
                    break;
                default:
                    rune_Type = "sto";
                    break;
            }
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
            
            currentRune = generateRndRune();
            currentRune.rune = "images\\sto.png";
        }

        private void PaintCorrectRune(Square sqr, Square current)
        {
            sqr.rune = current.rune;
            Graphics g = CreateGraphics();
            if (sqr.IsFilled == false)
            {
                sqr.IsFilled = true;
                g.FillRectangle(Brushes.Goldenrod, sqr.X, sqr.Y, sqr.Rectangle().Width - 1, sqr.Rectangle().Width - 1);
                NumOfNotFilled--;
            }
            g.DrawImage(new Bitmap(current.rune), sqr.Rectangle());
        }

        private Square checkRuneIntegrity(Square newRune, Square prevRune)
        {
            int NumOfStones = 0;
            for (int i = 0; i <= 80; i++)
            {
                if (grid[i].getType() == "sto")
                {
                    NumOfStones++;
                }
                if (NumOfStones == 2)
                    break;
            }
            do
            {
                newRune = generateRndRune();
            } while (((newRune.getType() == prevRune.getType()) && (newRune.getColor() == prevRune.getColor()))
                    || ((newRune.getType() == "sto") && (NumOfStones == 2)));
            return newRune;
        }

        private Square OnCorrectRune(Square sqr, Square curRune)
        {
            PaintCorrectRune(sqr, curRune);
            runeSound.controls.play();
            Square prevRune = curRune;
            curRune = checkRuneIntegrity(curRune, prevRune);
            if (discard < 3)
            {
                discard = discard + 1;
                restoreSound.controls.play();
            }
            return curRune;
        }

        private void pnlMain_MouseClick(object sender, MouseEventArgs e)
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
                        if (grid[i].IsFilled == false)
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
                            if (grid[i].IsFilled == true)
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
                            if (grid[k].rune == string.Empty)
                                NumOfEmpty++;
                        }
                        if ((NumOfEmpty == 81) && (NumOfNotFilled > 0))
                        {
                            currentRune.rune = "images\\sto.png";
                            PaintCorrectRune(grid[i], currentRune);
                            runeSound.controls.play();
                            Square prevRune = currentRune;
                            currentRune = checkRuneIntegrity(currentRune, prevRune);
                            score = score + scoreMod;
                            timer.Start();
                        }
                        else
                        {
                            if (currentRune.getType() != "sku")
                            {
                                if (grid[i].getType() == string.Empty)
                                {

                                    if (i == 0)
                                    {
                                        if ((grid[1].rune != string.Empty) || (grid[9].rune != string.Empty))
                                        {
                                            if ((((grid[1].getType() == "sto") || (grid[1].getType() == string.Empty) || (grid[1].getType() == currentRune.getType()) || (grid[1].getColor() == currentRune.getColor()))
                                                && ((grid[9].getType() == "sto") || (grid[9].getType() == string.Empty) || (grid[9].getType() == currentRune.getType()) || (grid[9].getColor() == currentRune.getColor())))
                                                || (currentRune.getType() == "sto"))
                                            {
                                                currentRune = OnCorrectRune(grid[i], currentRune);
                                                if ((grid[1].rune == string.Empty) || (grid[9].rune == string.Empty))
                                                {
                                                    score = score + scoreMod;
                                                }
                                                else
                                                    score = score + scoreMod * 2;
                                            }
                                            else
                                                invalidSound.controls.play();
                                        }
                                        else
                                            invalidSound.controls.play();
                                    }
                                    else
                                    {
                                        if (i == 8)
                                        {
                                            if ((grid[7].getType() != string.Empty) || (grid[17].getType() != string.Empty))
                                            {
                                                if ((((grid[7].getType() == "sto") || (grid[7].getType() == string.Empty) || (grid[7].getType() == currentRune.getType()) || (grid[7].getColor() == currentRune.getColor()))
                                                    && ((grid[17].getType() == "sto") || (grid[17].getType() == string.Empty) || (grid[17].getType() == currentRune.getType()) || (grid[17].getColor() == currentRune.getColor())))
                                                    || (currentRune.getType() == "sto"))
                                                {
                                                    currentRune = OnCorrectRune(grid[i], currentRune);
                                                    if ((grid[7].rune == string.Empty) || (grid[17].rune == string.Empty))
                                                    {
                                                        score = score + scoreMod;
                                                    }
                                                    else
                                                        score = score + scoreMod * 2;
                                                }
                                                else
                                                    invalidSound.controls.play();
                                            }
                                            else
                                                invalidSound.controls.play();
                                        }
                                        else
                                        {
                                            if (i == 72)
                                            {
                                                if ((grid[63].getType() != string.Empty) || (grid[73].getType() != string.Empty))
                                                {
                                                    if ((((grid[63].getType() == "sto") || (grid[63].getType() == string.Empty) || (grid[63].getType() == currentRune.getType()) || (grid[63].getColor() == currentRune.getColor()))
                                                        && ((grid[73].getType() == "sto") || (grid[73].getType() == string.Empty) || (grid[73].getType() == currentRune.getType()) || (grid[73].getColor() == currentRune.getColor())))
                                                        || (currentRune.getType() == "sto"))
                                                    {
                                                        currentRune = OnCorrectRune(grid[i], currentRune);
                                                        if ((grid[63].rune == string.Empty) || (grid[73].rune == string.Empty))
                                                        {
                                                            score = score + scoreMod;
                                                        }
                                                        else
                                                            score = score + scoreMod * 2;
                                                    }
                                                    else
                                                        invalidSound.controls.play();
                                                }
                                                else
                                                    invalidSound.controls.play();
                                            }
                                            else
                                            {
                                                if (i == 80)
                                                {
                                                    if ((grid[71].getType() != string.Empty) || (grid[79].getType() != string.Empty))
                                                    {
                                                        if ((((grid[71].getType() == "sto") || (grid[71].getType() == string.Empty) || (grid[71].getType() == currentRune.getType()) || (grid[71].getColor() == currentRune.getColor()))
                                                            && ((grid[79].getType() == "sto") || (grid[79].getType() == string.Empty) || (grid[79].getType() == currentRune.getType()) || (grid[79].getColor() == currentRune.getColor())))
                                                            || (currentRune.getType() == "sto"))
                                                        {
                                                            currentRune = OnCorrectRune(grid[i], currentRune);
                                                            if ((grid[63].rune == string.Empty) || (grid[73].rune == string.Empty))
                                                            {
                                                                score = score + scoreMod;
                                                            }
                                                            else
                                                                score = score + scoreMod * 2;
                                                        }
                                                        else
                                                            invalidSound.controls.play();
                                                    }
                                                    else
                                                        invalidSound.controls.play();
                                                }
                                                else
                                                {
                                                    if ((i >= 1) && (i <= 7))
                                                    {
                                                        if ((grid[i - 1].getType() != string.Empty) || (grid[i + 9].getType() != string.Empty) || (grid[i + 1].getType() != string.Empty))
                                                        {
                                                            if ((((grid[i - 1].getType() == "sto") || (grid[i - 1].getType() == string.Empty) || (grid[i - 1].getType() == currentRune.getType()) || (grid[i - 1].getColor() == currentRune.getColor()))
                                                                && ((grid[i + 9].getType() == "sto") || (grid[i + 9].getType() == string.Empty) || (grid[i + 9].getType() == currentRune.getType()) || (grid[i + 9].getColor() == currentRune.getColor()))
                                                                && ((grid[i + 1].getType() == "sto") || (grid[i + 1].getType() == string.Empty) || (grid[i + 1].getType() == currentRune.getType()) || (grid[i + 1].getColor() == currentRune.getColor())))
                                                                || (currentRune.getType() == "sto"))
                                                            {
                                                                currentRune = OnCorrectRune(grid[i], currentRune);
                                                                int mulMod = 0;
                                                                if (grid[i - 1].rune != string.Empty)
                                                                    mulMod++;
                                                                if (grid[i + 1].rune != string.Empty)
                                                                    mulMod++;
                                                                if (grid[i + 9].rune != string.Empty)
                                                                    mulMod++;
                                                                score = score + scoreMod * mulMod;
                                                            }
                                                            else
                                                                invalidSound.controls.play();
                                                        }
                                                        else
                                                            invalidSound.controls.play();
                                                    }
                                                    else
                                                    {
                                                        if ((i >= 73) && (i <= 79))
                                                        {
                                                            if ((grid[i - 1].getType() != string.Empty) || (grid[i - 9].getType() != string.Empty) || (grid[i + 1].getType() != string.Empty))
                                                            {
                                                                if ((((grid[i - 1].getType() == "sto") || (grid[i - 1].getType() == string.Empty) || (grid[i - 1].getType() == currentRune.getType()) || (grid[i - 1].getColor() == currentRune.getColor()))
                                                                    && ((grid[i - 9].getType() == "sto") || (grid[i - 9].getType() == string.Empty) || (grid[i - 9].getType() == currentRune.getType()) || (grid[i - 9].getColor() == currentRune.getColor()))
                                                                    && ((grid[i + 1].getType() == "sto") || (grid[i + 1].getType() == string.Empty) || (grid[i + 1].getType() == currentRune.getType()) || (grid[i + 1].getColor() == currentRune.getColor())))
                                                                    || (currentRune.getType() == "sto"))
                                                                {
                                                                    currentRune = OnCorrectRune(grid[i], currentRune);
                                                                    int mulMod = 0;
                                                                    if (grid[i - 1].rune != string.Empty)
                                                                        mulMod++;
                                                                    if (grid[i + 1].rune != string.Empty)
                                                                        mulMod++;
                                                                    if (grid[i - 9].rune != string.Empty)
                                                                        mulMod++;
                                                                    score = score + scoreMod * mulMod;
                                                                }
                                                                else
                                                                    invalidSound.controls.play();
                                                            }
                                                            else
                                                                invalidSound.controls.play();
                                                        }
                                                        else
                                                        {
                                                            if ((i == 9) || (i == 18) || (i == 27) || (i == 36) || (i == 45) || (i == 54) || (i == 63))
                                                            {
                                                                if ((grid[i - 9].getType() != string.Empty) || (grid[i + 9].getType() != string.Empty) || (grid[i + 1].getType() != string.Empty))
                                                                {
                                                                    if ((((grid[i - 9].getType() == "sto") || (grid[i - 9].getType() == string.Empty) || (grid[i - 9].getType() == currentRune.getType()) || (grid[i - 9].getColor() == currentRune.getColor()))
                                                                        && ((grid[i + 9].getType() == "sto") || (grid[i + 9].getType() == string.Empty) || (grid[i + 9].getType() == currentRune.getType()) || (grid[i + 9].getColor() == currentRune.getColor()))
                                                                        && ((grid[i + 1].getType() == "sto") || (grid[i + 1].getType() == string.Empty) || (grid[i + 1].getType() == currentRune.getType()) || (grid[i + 1].getColor() == currentRune.getColor())))
                                                                        || (currentRune.getType() == "sto"))
                                                                    {
                                                                        currentRune = OnCorrectRune(grid[i], currentRune);
                                                                        int mulMod = 0;
                                                                        if (grid[i - 9].rune != string.Empty)
                                                                            mulMod++;
                                                                        if (grid[i + 1].rune != string.Empty)
                                                                            mulMod++;
                                                                        if (grid[i + 9].rune != string.Empty)
                                                                            mulMod++;
                                                                        score = score + scoreMod * mulMod;
                                                                    }
                                                                    else
                                                                        invalidSound.controls.play();
                                                                }
                                                                else
                                                                    invalidSound.controls.play();
                                                            }
                                                            else
                                                            {
                                                                if ((i == 17) || (i == 26) || (i == 35) || (i == 44) || (i == 53) || (i == 62) || (i == 71))
                                                                {
                                                                    if ((grid[i - 9].getType() != string.Empty) || (grid[i + 9].getType() != string.Empty) || (grid[i - 1].getType() != string.Empty))
                                                                    {
                                                                        if ((((grid[i - 9].getType() == "sto") || (grid[i - 9].getType() == string.Empty) || (grid[i - 9].getType() == currentRune.getType()) || (grid[i - 9].getColor() == currentRune.getColor()))
                                                                            && ((grid[i + 9].getType() == "sto") || (grid[i + 9].getType() == string.Empty) || (grid[i + 9].getType() == currentRune.getType()) || (grid[i + 9].getColor() == currentRune.getColor()))
                                                                            && ((grid[i - 1].getType() == "sto") || (grid[i - 1].getType() == string.Empty) || (grid[i - 1].getType() == currentRune.getType()) || (grid[i - 1].getColor() == currentRune.getColor())))
                                                                            || (currentRune.getType() == "sto"))
                                                                        {
                                                                            currentRune = OnCorrectRune(grid[i], currentRune);
                                                                            int mulMod = 0;
                                                                            if (grid[i - 9].rune != string.Empty)
                                                                                mulMod++;
                                                                            if (grid[i - 1].rune != string.Empty)
                                                                                mulMod++;
                                                                            if (grid[i + 9].rune != string.Empty)
                                                                                mulMod++;
                                                                            score = score + scoreMod * mulMod;
                                                                        }
                                                                        else
                                                                            invalidSound.controls.play();
                                                                    }
                                                                    else
                                                                        invalidSound.controls.play();
                                                                }
                                                                else
                                                                {
                                                                    if ((grid[i - 9].getType() != string.Empty) || (grid[i + 9].getType() != string.Empty) || (grid[i - 1].getType() != string.Empty) || (grid[i + 1].getType() != string.Empty))
                                                                    {
                                                                        if ((((grid[i - 9].getType() == "sto") || (grid[i - 9].getType() == string.Empty) || (grid[i - 9].getType() == currentRune.getType()) || (grid[i - 9].getColor() == currentRune.getColor()))
                                                                            && ((grid[i + 9].getType() == "sto") || (grid[i + 9].getType() == string.Empty) || (grid[i + 9].getType() == currentRune.getType()) || (grid[i + 9].getColor() == currentRune.getColor()))
                                                                            && ((grid[i + 1].getType() == "sto") || (grid[i + 1].getType() == string.Empty) || (grid[i + 1].getType() == currentRune.getType()) || (grid[i + 1].getColor() == currentRune.getColor()))
                                                                            && ((grid[i - 1].getType() == "sto") || (grid[i - 1].getType() == string.Empty) || (grid[i - 1].getType() == currentRune.getType()) || (grid[i - 1].getColor() == currentRune.getColor())))
                                                                            || (currentRune.getType() == "sto"))
                                                                        {
                                                                            currentRune = OnCorrectRune(grid[i], currentRune);
                                                                            int mulMod = 0;
                                                                            if (grid[i - 1].rune != string.Empty)
                                                                                mulMod++;
                                                                            if (grid[i + 1].rune != string.Empty)
                                                                                mulMod++;
                                                                            if (grid[i + 9].rune != string.Empty)
                                                                                mulMod++;
                                                                            if (grid[i - 9].rune != string.Empty)
                                                                                mulMod++;
                                                                            score = score + scoreMod * mulMod;
                                                                        }
                                                                        else
                                                                            invalidSound.controls.play();
                                                                    }
                                                                    else
                                                                        invalidSound.controls.play();
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    invalidSound.controls.play();
                                }
                                if (NumOfNotFilled == 0)
                                {
                                    timer.Stop();
                                    MessageBox.Show("You win!\nTime elapsed: " + t.ToString());
                                }       
                            }
                            else
                            {
                                if (currentRune.getType() == "sku")
                                {
                                    if (grid[i].getType() != string.Empty)
                                    {
                                        grid[i].rune = string.Empty;
                                        g.FillRectangle(Brushes.Goldenrod, grid[i].X, grid[i].Y, grid[i].Rectangle().Width - 1, grid[i].Rectangle().Width - 1);
                                        Square prevRune = currentRune;
                                        currentRune = checkRuneIntegrity(currentRune, prevRune);
                                        skullSound.controls.play();
                                        if (discard < 3)
                                            discard++;
                                    }
                                }
                            }
                        }
                    }
                }
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
                            grid[j].rune = string.Empty;
                            g.FillRectangle(Brushes.Goldenrod, grid[j].X, grid[j].Y, grid[j].Rectangle().Width - 1, grid[j].Rectangle().Width - 1);
                        }
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
                            grid[k].rune = string.Empty;
                            g.FillRectangle(Brushes.Goldenrod, grid[k].X, grid[k].Y, grid[k].Rectangle().Width - 1, grid[k].Rectangle().Width - 1);
                        }
                        completeSound.controls.play();
                    }
                }
            }
            else
            {
                if (e.Button == MouseButtons.Right)
                {
                    if (NumOfNotFilled != 81)
                    {
                        discardSound.controls.play();
                        Square prevRune = currentRune;
                        currentRune = checkRuneIntegrity(currentRune, prevRune);
                        discard--;
                    }
                    else
                    {
                        invalidSound.controls.play();
                    }
                   
                }
            }
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            t++;
        }
    }
}
