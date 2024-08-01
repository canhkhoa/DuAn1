using DAL;
using DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI
{
    public partial class FDatBanAn : Form
    {
        private ContextMenuStrip contextMenuStrip;
        public FDatBanAn()
        {
            InitializeComponent();
            LoadCategory();
            LoadTable();
            ResetUI();
            InitializeContextMenuStrip();
        }
        private void ResetUI()
        {
            flpCategory.Visible = false;
            flpFood.Visible = false;
            lsvBill.Visible = false;
            
            button1.Visible = true;
            flpTable.Visible = false;
            button2.Visible = false;
            txtTimKiem.Visible = false;
            btnTimKiem.Visible = false;
            txbTotalPrice.Visible = false;
            txtTenBan.Visible = false;
        }
        private void LoadCategory()
        {
            List<Category> categoryList = CategoryDAL.Instance.GetListCategory();
            foreach (Category item in categoryList)
            {
                //Button btn = new Button()
                //{
                //    Width = TableDAL.TableWidth,
                //    Height = TableDAL.TableHeight,
                //    Text = item.Name,
                //    Tag = item
                //};
                CustomButton btn = new CustomButton
                {
                    BorderRadius = 20,
                    BorderSize = 2,
                    BorderColor = Color.PaleVioletRed,
                    Size = new Size(150, 60),
                    Text = item.Name,
                    Tag = item,
                    ForeColor = Color.White,
                    BackColor = Color.Black
                };
                btn.Click += btnCategory_Click;

                flpCategory.Controls.Add(btn);
            }

        }
        private void LoadTable()
        {
            flpTable.Controls.Clear();
            List<Table> tableList = TableDAL.Instance.LoadTableList();
            foreach (Table item in tableList)
            {
                //Button btn = new Button()
                //{
                //    Width = 150,
                //    Height = 60,
                //    Text = item.TenBan + Environment.NewLine + item.TrangThai,
                //    Tag = item
                //};
                CustomButton btn = new CustomButton()
                {
                    ForeColor = Color.Black,
                    BorderRadius = 20,
                    BorderSize = 2,
                    BorderColor = Color.PaleVioletRed,
                    Size = new Size(150, 80),
                    Text = item.TenBan + Environment.NewLine + item.TrangThai,
                    Tag = item
                };
                btn.Click += btnTable_Click;

                btn.BackColor = item.TrangThai == "Có Người" ? Color.Red : Color.White;
                flpTable.Controls.Add(btn);
            }
        }
        private void InitializeContextMenuStrip()
        {
            contextMenuStrip = new ContextMenuStrip();



            ToolStripMenuItem rightClickItem = new ToolStripMenuItem("Loại bỏ một phần thức ăn");
            rightClickItem.Click += (sender, e) =>
            {
                btnFoodRight_Click(sender, e);
            };

            contextMenuStrip.Items.Add(rightClickItem);
        }
        private void ShowBill(int id)
        {
            lsvBill.Items.Clear();
            List<DTO.Menu> listBillInfo = MenuDAL.Instance.GetListMenuBYTable(id);
            float totalPrice = 0;
            foreach (DTO.Menu item in listBillInfo)
            {
                ListViewItem lsvItem = new ListViewItem(item.FoodName.ToString());
                lsvItem.SubItems.Add(item.Count.ToString());
                lsvItem.SubItems.Add(item.Price.ToString());
                lsvItem.SubItems.Add(item.TotalPrice.ToString());
                totalPrice += item.TotalPrice;

                lsvBill.Items.Add(lsvItem);
            }
            CultureInfo culture = new CultureInfo("vi-VN");
            txbTotalPrice.Text = totalPrice.ToString("c", culture);
        }
        private void btn_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Button button = sender as Button;
                contextMenuStrip.Tag = button; // Lưu button vào Tag của ContextMenuStrip để sử dụng sau này
                contextMenuStrip.Show(button, e.Location);
            }
        }
        private Image ByteArrayToImage(byte[] byteArray)
        {
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                return Image.FromStream(ms);
            }
        }
        private void btnCategory_Click(object sender, EventArgs e)
        {
            int categoryID = ((sender as Button).Tag as Category).ID;
            flpFood.Controls.Clear();
            List<Food> listFood = FoodDAL.Instance.GetFoodByCategoryID(categoryID);
            foreach (Food item in listFood)
            {
                CustomButton btn = new CustomButton
                {
                    Text = $"{item.TenMon}\n{item.Gia}",
                    BorderRadius = 20,
                    BorderSize = 2,
                    Size = new Size(150, 150),
                    Tag = item,
                    TextAlign = ContentAlignment.BottomCenter,
                    ImageAlign = ContentAlignment.TopCenter,
                    BackColor = Color.White,
                    ForeColor = Color.Black


                };
                Label lblText = new Label
                {
                    Size = new Size(180, 60),

                };

                // Convert the byte array to an Image and set it for the button if available
                if (item.HinhMonAn != null && item.HinhMonAn.Length > 0)
                {
                    try
                    {
                        btn.Image = new Bitmap(ByteArrayToImage(item.HinhMonAn), new Size(btn.Width, btn.Height - 60)); // Adjust image size to fit the button
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading image: " + ex.Message);
                    }
                }

                btn.Click += btnFood_Click;
                btn.MouseUp += btn_MouseUp;
                flpFood.Controls.Add(btn);
            }
        }

        private void btnFood_Click(object sender, EventArgs e)
        {
            Button button = contextMenuStrip.Tag as Button;
            Table table = lsvBill.Tag as Table;
            int idBill = BillDAL.Instance.GetBillIDByTableID(table.MaBan);
            int foodID = ((sender as Button).Tag as Food).IdMon;
            int count = 1;

            if (idBill == -1)
            {
                BillDAL.Instance.InsertBill(table.MaBan);
                BillInfoDAL.Instance.InserBillInfo(BillDAL.Instance.GetMaxIDBill(), foodID, count);
            }
            else
            {
                BillInfoDAL.Instance.InserBillInfo(idBill, foodID, count);
            }

            ShowBill(table.MaBan);
            LoadTable();
        }

        private void btnTable_Click(object sender, EventArgs e)
        {
            int tableID = ((sender as Button).Tag as Table).MaBan;
            lsvBill.Tag = (sender as Button).Tag;
            Table table = TableDAL.Instance.GetTablebyId(tableID).FirstOrDefault();
            if (table != null)
            {
                txtTenBan.Text = table.TenBan; // Assuming 'TenBan' is the property for the table name
            }

            ShowBill(tableID);
            flpCategory.Visible = true;
            flpFood.Visible = true;
            lsvBill.Visible = true;
            
            button2.Visible = true;
            btnTimKiem.Visible = true;
            txtTimKiem.Visible = true;
            txbTotalPrice.Visible = true;
            txtTenBan.Visible = true;
        }

        private void btnCheckOut_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            flpTable.Visible = true;
            button1.Visible = false;
            button2.Visible = true;
            LoadTable();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ResetUI();
        }
        private void btnFoodRight_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            Button button = contextMenuStrip.Tag as Button;
            Table table = lsvBill.Tag as Table;
            int idBill = BillDAL.Instance.GetBillIDByTableID(table.MaBan);
            int foodID = ((button.Tag as Food).IdMon);
            int count = -1; // Số lượng âm để xóa sản phẩm khỏi hóa đơn

            if (idBill == -1)
            {
                BillDAL.Instance.InsertBill(table.MaBan);
                BillInfoDAL.Instance.InserBillInfo(BillDAL.Instance.GetMaxIDBill(), foodID, count);
            }
            else
            {
                BillInfoDAL.Instance.InserBillInfo(idBill, foodID, count);
            }

            ShowBill(table.MaBan);
            LoadTable();
        }
    }
}
