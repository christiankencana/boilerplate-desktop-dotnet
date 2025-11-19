using boilerplate_desktop_dotnet.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace boilerplate_desktop_dotnet.Views.Roles
{
    public partial class UserRoleListForm : Form
    {
        public UserRoleListForm()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            var roles = RoleController.GetAllRoles();
            var users = UserController.GetAllUsers();

            cboRoles.DataSource = roles;
            cboRoles.DisplayMember = "Name";
            cboRoles.ValueMember = "Id";

            cboUsers.DataSource = users;
            cboUsers.DisplayMember = "Username";
            cboUsers.ValueMember = "Id";

            dgvUserRoles.DataSource = RoleController.GetUserRoles();
        }

        private void btnAssign_Click(object sender, EventArgs e)
        {
            if (cboUsers.SelectedItem == null || cboRoles.SelectedItem == null) return;
            int userId = (int)cboUsers.SelectedValue;
            int roleId = (int)cboRoles.SelectedValue;

            if (RoleController.AssignRoleToUser(userId, roleId))
            {
                MessageBox.Show("Assigned.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dgvUserRoles.DataSource = RoleController.GetUserRoles();
            }
            else MessageBox.Show("Assign failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (dgvUserRoles.SelectedRows.Count == 0) { MessageBox.Show("Select an assignment first."); return; }
            var ur = dgvUserRoles.SelectedRows[0].DataBoundItem;
            var userIdProp = ur.GetType().GetProperty("UserId");
            var roleIdProp = ur.GetType().GetProperty("RoleId");
            if (userIdProp == null || roleIdProp == null) return;
            int userId = (int)userIdProp.GetValue(ur);
            int roleId = (int)roleIdProp.GetValue(ur);

            if (RoleController.RemoveRoleFromUser(userId, roleId))
            {
                MessageBox.Show("Removed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dgvUserRoles.DataSource = RoleController.GetUserRoles();
            }
            else MessageBox.Show("Remove failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnClose_Click(object sender, EventArgs e) => Close();
    }
}
