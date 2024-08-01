using BUS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI
{
    public partial class FDoiMatKhau : Form
    {
        private string gmail;
        Thread th;
        NhanVienBUS busnhanvien = new NhanVienBUS();
        NhanVienDNBUS NhanVienDNBUS = new NhanVienDNBUS();
        public FDoiMatKhau()
        {
            InitializeComponent();
        }
        public FDoiMatKhau(string user)
        {
            InitializeComponent();
            this.gmail = user;
            txt_taikhoan.Enabled = false;  
        }

        private void FDoiMatKhau_Load(object sender, EventArgs e)
        {
            txt_taikhoan.Text = gmail;
        }
        public string encryption(string password)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] encrypt;
            UTF8Encoding encode = new UTF8Encoding();
            //encrypt the given password string into Encrypted data  
            encrypt = md5.ComputeHash(encode.GetBytes(password));
            StringBuilder encryptdata = new StringBuilder();
            //Create a new string by using the encrypted data  
            for (int i = 0; i < encrypt.Length; i++)
            {
                encryptdata.Append(encrypt[i].ToString());
            }
            return encryptdata.ToString();
        }

        private void btnDoiMatKhau_Click(object sender, EventArgs e)
        {
            if (txtMatKhauCu.Text.Trim().Length == 0)// kiem tra phai nhap data
            {
                MessageBox.Show("Bạn phải nhập mật khẩu cũ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtMatKhauCu.Focus();
                return;
            }
            else if (txtMatKhauMoi.Text.Trim().Length == 0)
            {
                MessageBox.Show("Bạn phải nhập mật khẩu mới", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtMatKhauMoi.Focus();
                return;
            }
            else if (txtMatKhauMoi2.Text.Trim().Length == 0)
            {
                MessageBox.Show("Bạn phải nhập lại mật khẩu mới", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtMatKhauMoi2.Focus();
                return;
            }
            else if (txtMatKhauMoi2.Text.Trim() != txtMatKhauMoi.Text.Trim())
            {
                MessageBox.Show("Bạn phải nhập mật khẩu và nhập lại mật khẩu giống nhau", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtMatKhauMoi.Focus();
                return;
            }
            else
            {
                if (MessageBox.Show("Bạn có chắc muốn cập nhật mật khẩu", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    //do something if YES
                    string matkhaumoi = NhanVienDNBUS.EncryptPassword(txtMatKhauMoi.Text);
                    string matkhaucu = NhanVienDNBUS.EncryptPassword(txtMatKhauCu.Text);
                    if (busnhanvien.UpdateMatKhau(txt_taikhoan.Text, matkhaucu, matkhaumoi))
                    {
                        //FHub.profile = 1;// cập nhật pass thành cong
                        //FHub.session = 0;//đưa về tình trạng chưa đăng nhâp

                        MessageBox.Show("Cập nhật mật khẩu thành công, bạn cần phải đăng nhập lại");
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Mât khẩu cũ không đúng,Cập nhật mật khẩu không thành công");
                        txtMatKhauCu.Text = null;
                       txtMatKhauMoi.Text = null;
                        txtMatKhauMoi2.Text = null;
                    }
                }
                else
                {
                    //do something if NO
                    txtMatKhauCu.Text = null;
                    txtMatKhauMoi.Text = null;
                    txtMatKhauMoi2.Text = null;
                }
            }
        }
    }
}
