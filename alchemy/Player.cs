using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alchemy
{
    class Player
    {
        string Name { get; set; }
        int Score { get; set; }
        TimeSpan time { get; set; }
        string Difficulty { get; set; }
        bool Win { get; set; }

        public Player()
        {
            
        }

        public Player(string name, int scr, int t, int diff, bool win)
        {
            Name = name;
            Score = scr;
            TimeSpan ts = TimeSpan.FromSeconds(t); //Chuyển thời gian hoàn thành ra giờ:phút:giây.
            time = ts;
            switch (diff)
            {
                case 2:
                    Difficulty = "Journeyman";
                    break;
                case 3:
                    Difficulty = "Master";
                    break;
                default:
                    Difficulty = "Apprentice";
                    break;
            }
            Win = win;
        }

        public string writePlayer() //Hàm này sẽ viết thông tin người chơi ra file.
        {
            DateTime date = DateTime.Now;
            string status = string.Empty;
            if (Win) //Giá trị của "Win" sẽ quyết định giá trị của cột "Result" trong bảng xếp hạng.
                status = "Succeeded";
            else
                status = "Failed";
            return Name + "$" + date.ToString("dd/MM/yyyy") + "$" +  Difficulty + "$" + Score.ToString() + "$" + time.ToString() + "$" + status;
            // Tại đây, ký hiệu '$' được sử dụng để phân biệt giữa các cột. Trong frmGameOver, hàm string.Split()
            // sẽ được sử dụng để tách các cột ra lấy giá trị.
        }
    }
}
