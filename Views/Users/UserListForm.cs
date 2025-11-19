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
    public partial class UserListForm : Form
    {
        private BindingSource _bs = new BindingSource();

        public UserListForm()
        {
            InitializeComponent();
            dgvUsers.AutoGenerateColumns = true;
            dgvUsers.DataSource = _bs;
        }

        private void UserListForm_Load(object sender, EventArgs e)
        {
            LoadUsers();
        }

        private void LoadUsers()
        {
            var users = UserController.GetAllUsers();
            _bs.DataSource = users;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadUsers();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            var q = txtSearch.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(q))
            {
                _bs.DataSource = UserController.GetAllUsers();
                return;
            }

            var filtered = UserController.GetAllUsers()
                .Where(u => (u.Username ?? "").ToLower().Contains(q)
                         || (u.Email ?? "").ToLower().Contains(q)
                         || (u.FullName ?? "").ToLower().Contains(q))
                .ToList();

            _bs.DataSource = filtered;
        }

        private User GetSelectedUser()
        {
            if (dgvUsers.SelectedRows.Count == 0) return null;
            return dgvUsers.SelectedRows[0].DataBoundItem as User;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var form = new UserForm();
            if (form.ShowDialog() == DialogResult.OK) LoadUsers();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var user = GetSelectedUser();
            if (user == null)
            {
                MessageBox.Show("Select a user first.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var form = new UserForm(user.Id);
            if (form.ShowDialog() == DialogResult.OK) LoadUsers();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var user = GetSelectedUser();
            if (user == null)
            {
                MessageBox.Show("Select a user first.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show($"Delete user '{user.Username}'?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                if (UserController.DeleteUser(user.Id))
                {
                    MessageBox.Show("Deleted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadUsers();
                }
                else MessageBox.Show("Delete failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
