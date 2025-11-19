using boilerplate_desktop_dotnet.Controllers;
using boilerplate_desktop_dotnet.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace boilerplate_desktop_dotnet.Views.Users
{
    public partial class UserForm : Form
    {
        private int? _userId;

        public UserForm(int? userId = null)
        {
            InitializeComponent();
            _userId = userId;
            if (_userId.HasValue) LoadUser(_userId.Value);
        }

        private void LoadUser(int id)
        {
            var user = UserController.GetUserById(id);
            if (user == null) return;
            // create simple controls on demand if designer empty
            if (Controls["txtUsername"] == null)
            {
                // minimal UI fallback
                var lbl = new Label { Text = "Username", Top = 10, Left = 10 };
                var txtUsername = new TextBox { Name = "txtUsername", Top = 30, Left = 10, Width = 300 };
                var lblEmail = new Label { Text = "Email", Top = 65, Left = 10 };
                var txtEmail = new TextBox { Name = "txtEmail", Top = 85, Left = 10, Width = 300 };
                var lblFull = new Label { Text = "Full Name", Top = 120, Left = 10 };
                var txtFull = new TextBox { Name = "txtFullName", Top = 140, Left = 10, Width = 300 };
                var lblPwd = new Label { Text = "Password (leave blank to keep)", Top = 175, Left = 10 };
                var txtPwd = new TextBox { Name = "txtPassword", Top = 195, Left = 10, Width = 300, PasswordChar = '*' };
                var btnSave = new Button { Text = "Save", Top = 230, Left = 10, Width = 100 };
                btnSave.Click += BtnSave_Click;

                Controls.Add(lbl); Controls.Add(txtUsername);
                Controls.Add(lblEmail); Controls.Add(txtEmail);
                Controls.Add(lblFull); Controls.Add(txtFull);
                Controls.Add(lblPwd); Controls.Add(txtPwd);
                Controls.Add(btnSave);
            }

            Controls["txtUsername"].Text = user.Username;
            Controls["txtEmail"].Text = user.Email;
            Controls["txtFullName"].Text = user.FullName;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var username = Controls["txtUsername"].Text.Trim();
            var email = Controls["txtEmail"].Text.Trim();
            var fullName = Controls["txtFullName"].Text.Trim();
            var pwd = Controls["txtPassword"].Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Username and Email required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_userId.HasValue)
            {
                var user = new User { Id = _userId.Value, Username = username, Email = email, FullName = fullName };
                if (!UserController.UpdateUser(user)) { MessageBox.Show("Update failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (!string.IsNullOrEmpty(pwd)) UserController.ChangePassword(user.Id, pwd);
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                var user = new User { Username = username, Email = email, FullName = fullName };
                if (!UserController.CreateUser(user, pwd)) { MessageBox.Show("Create failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
