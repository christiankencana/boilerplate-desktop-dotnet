using boilerplate_desktop_dotnet.Utilities;
using boilerplate_desktop_dotnet.Views.Auth;
using boilerplate_desktop_dotnet.Views.Roles;
using boilerplate_desktop_dotnet.Views.Users;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace boilerplate_desktop_dotnet.Views.Dashboard
{
    public partial class DashboardForm : Form
    {
        public DashboardForm()
        {
            InitializeComponent();
        }

        private void DashboardForm_Load(object sender, EventArgs e)
        {
            if (!SessionManager.IsLoggedIn)
            {
                MessageBox.Show("Please login first", "Unauthorized",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                RedirectToLogin();
                return;
            }

            var user = SessionManager.CurrentUser;
            lblWelcome.Text = $"Welcome, {user.FullName ?? user.Username}!";
            lblUserInfo.Text = $"Logged in as: {user.Email} | Role: {(SessionManager.IsAdmin ? "Admin" : "User")}";
            lblStatus.Text = $"Ready | User: {user.Username}";

            // Hide admin menus if not admin
            if (!SessionManager.IsAdmin)
            {
                menuUsers.Visible = false;
                menuRoles.Visible = false;
                menuUserRoles.Visible = false;
            }
        }
        private void RedirectToLogin()
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }

        private void menuLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to logout?", "Confirm Logout",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                SessionManager.ClearSession();
                RedirectToLogin();
            }
        }

        private void menuExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to exit?", "Confirm Exit",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void menuUsers_Click(object sender, EventArgs e)
        {
            if (!SessionManager.IsAdmin)
            {
                MessageBox.Show("You don't have permission to access this feature", "Access Denied",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UserListForm userListForm = new UserListForm();
            userListForm.ShowDialog();
        }

        private void menuRoles_Click(object sender, EventArgs e)
        {
            if (!SessionManager.IsAdmin)
            {
                MessageBox.Show("You don't have permission to access this feature", "Access Denied",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            RoleListForm roleListForm = new RoleListForm();
            roleListForm.ShowDialog();
        }

        private void menuUserRoles_Click(object sender, EventArgs e)
        {
            if (!SessionManager.IsAdmin)
            {
                MessageBox.Show("You don't have permission to access this feature", "Access Denied",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UserRoleListForm userRoleListForm = new UserRoleListForm();
            userRoleListForm.ShowDialog();
        }

        private void DashboardForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to exit?", "Confirm Exit",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    Application.Exit();
                }
            }
        }
    }
}
