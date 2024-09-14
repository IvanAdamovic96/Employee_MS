using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsApp1
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(UsernameTb.Text == "" || PasswordTb.Text == "")
            {
                MessageBox.Show("Username and password can't be empty!!!");
            }
            else if(UsernameTb.Text == "Admin" && PasswordTb.Text =="Admin")
            {
                Employees Obj = new Employees();
                Obj.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Username or password is incorrect !!!");
                UsernameTb.Text = "";
                PasswordTb.Text = "";
            }
        }

        private void ResetLbl_Click(object sender, EventArgs e)
        {
            UsernameTb.Text = "";
            PasswordTb.Text = "";
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
