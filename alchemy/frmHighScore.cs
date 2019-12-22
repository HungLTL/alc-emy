using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using System.IO;
using WMPLib;

namespace alchemy
{
    class frmHighScore : Form // Form chứa thông tin người chơi, điểm, thời gian, vv.
    {
        DataTable dt;
        DataGridView dgv;
        StreamReader file;
        Label lblDiff, lblSort;
        ComboBox cmbDiff, cmbSort;
        Button btnClear;
        bool IsSound;

        public frmHighScore(bool SoundStatus) //Lấy giá trị bool từ frmMain đễ xem người dùng có bật âm thanh hay không. 
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(680, 600);
            this.Text = "Archives";
            this.MinimumSize = this.Size;
            this.MaximumSize = this.Size;
            this.MaximizeBox = false;

            IsSound = SoundStatus;

            cmbDiff = new ComboBox(); // Hộp combobox này sẽ cho phép người dùng lọc ra theo độ khó.
            cmbDiff.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDiff.Items.Add("Apprentice");
            cmbDiff.Items.Add("Journeyman");
            cmbDiff.Items.Add("Master");
            cmbDiff.Items.Add(" ");
            cmbDiff.Width = 90;
            cmbDiff.Location = new Point(10, 40);
            cmbDiff.SelectedIndexChanged += cmbDiff_SelectedIndexChanged;
            this.Controls.Add(cmbDiff);

            lblDiff = new Label();
            lblDiff.Text = "Difficulty";
            lblDiff.Location = new Point(29, 25);
            this.Controls.Add(lblDiff);

            cmbSort = new ComboBox(); // Cho phép người dùng sắp xếp theo thời gian hoặc điểm, cao đến thấp.
            cmbSort.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSort.Items.Add("Score");
            cmbSort.Items.Add("Time");
            cmbSort.Width = cmbDiff.Width;
            cmbSort.Location = new Point(565, 40);
            cmbSort.SelectedIndexChanged += cmbSort_SelectedIndexChanged;
            this.Controls.Add(cmbSort);

            lblSort = new Label();
            lblSort.Text = "Sort by";
            lblSort.Location = new Point(590, 25);
            this.Controls.Add(lblSort);

            btnClear = new Button();
            btnClear.AutoSize = true;
            btnClear.Text = "Burn archives";
            btnClear.Location = new Point(288, 38);
            btnClear.Click += btnClear_Click;
            this.Controls.Add(btnClear);

            dgv = new DataGridView();
            dgv.Size = new Size(643, 478);
            dgv.ReadOnly = true;
            dgv.Location = new Point(10, 75);
            this.Controls.Add(dgv);

            // Thông tin về người chơi sẽ được lưu trong file score.txt. Khi người dùng muốn truy cập xem điểm,
            // hệ thống sẽ truy cập vào file này, đọc dữ liệu và truyền vào DataTable. DataTable này sẽ được 
            // DataGridView dùng làm DataSource.

            if (File.Exists("dat\\score.txt")) //Kiểm tra xem score.txt có tồn tại hay không. Nếu không có dòng này,
            {                             // trong trường hợp không có score.txt (do vừa mới cài đặt hoặc xóa),
                                          // chương trình sẽ báo lỗi Exception.
                using (file = new StreamReader("dat\\score.txt")) // Cần dùng using đễ hệ thống tự đóng lại StreamReader
                {                                           // sau khi dùng. Thiếu "using" sẽ dẫn đến lỗi "used by another process".
                    string[] column = file.ReadLine().Split('$'); // Thêm các cột vào DataTable. Tên cột có trong frmGameOver.cs
                    dt = new DataTable();
                    foreach (string c in column)
                    {
                        if (c == "Score")
                        {
                            dt.Columns.Add(c, typeof(int));
                        }
                        else
                            dt.Columns.Add(c);
                    }
                    string newline; //Thêm các giá trị các cột.
                    while ((newline = file.ReadLine()) != null)
                    {
                        DataRow dr = dt.NewRow();
                        string[] values = newline.Split('$');
                        for (int i = 0; i < values.Length; i++)
                        {
                            dr[i] = values[i];
                        }
                        dt.Rows.Add(dr);
                    }
                }
                dgv.DataSource = dt;
            }
        }

        private void btnClear_Click(object sender, EventArgs e) //Hàm xóa toàn bộ dữ liệu người chơi.
        {
            DialogResult res = MessageBox.Show("This will destroy all existing records.", "Are you sure?", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                FileInfo score = new FileInfo("dat\\score.txt");
                score.IsReadOnly = false;
                File.Delete("dat\\score.txt");
                this.Close();
                if (!IsSound) //Nếu người dùng không tắt âm thanh, IsSound trả về false. Khi này, hệ thống phát âm thanh
                              // "đốt giấy" để mô phỏng người dùng xóa dữ liệu, hay "đốt hết văn bản"
                {
                    WMPLib.WindowsMediaPlayer burn = new WMPLib.WindowsMediaPlayer();
                    burn.URL = "sounds\\burn.wav";
                    burn.controls.play();
                }
            }
        }

        private void cmbDiff_SelectedIndexChanged(object sender, EventArgs e) //Lọc các dòng có độ khó bằng giá trị cmbDiff.
        {
            if (cmbDiff.Text != " ") // Nếu người dùng để trống cmbDiff, xem như không lọc theo độ khó.
                (dgv.DataSource as DataTable).DefaultView.RowFilter = string.Format("Difficulty = '{0}'", cmbDiff.Text);
            else
            {
                (dgv.DataSource as DataTable).DefaultView.RowFilter = string.Empty;
            }
        }

        private void cmbSort_SelectedIndexChanged(object sender, EventArgs e) //Sắp xếp các dòng theo điểm hoặc thời gian từ cao đến thấp.
        {
            if (cmbSort.Text == "Score")
            {
                this.dgv.Sort(this.dgv.Columns["Score"], System.ComponentModel.ListSortDirection.Descending);
            }
            else
            {
                this.dgv.Sort(this.dgv.Columns["Time"], System.ComponentModel.ListSortDirection.Descending);
            }
            dgv.Refresh();
        }
    }
}
