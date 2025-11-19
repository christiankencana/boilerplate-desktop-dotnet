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

namespace boilerplate_desktop_dotnet.Views.Roles
{
    public partial class RoleListForm : Form
    {
        private BindingSource _bs = new BindingSource();

        public RoleListForm()
        {
            InitializeComponent();
            // if designer empty, add simple list
            if (Controls["dgvRoles"] == null)
            {
                var dgv = new DataGridView { Name = "dgvRoles", Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true };
                Controls.Add(dgv);
            }
            ((DataGridView)Controls["dgvRoles"]).DataSource = _bs;
        }

        private void RoleListForm_Load(object sender, EventArgs e)
        {
            LoadRoles();
        }

        private void LoadRoles()
        {
            _bs.DataSource = RoleController.GetAllRoles();
        }

        private Role GetSelectedRole()
        {
            var dgv = (DataGridView)Controls["dgvRoles"];
            if (dgv.SelectedRows.Count == 0) return null;
            return dgv.SelectedRows[0].DataBoundItem as Role;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var f = new RoleForm();
            if (f.ShowDialog() == DialogResult.OK) LoadRoles();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var r = GetSelectedRole();
            if (r == null) { MessageBox.Show("Select role first."); return; }
            var f = new RoleForm(r.Id);
            if (f.ShowDialog() == DialogResult.OK) LoadRoles();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var r = GetSelectedRole();
            if (r == null) { MessageBox.Show("Select role first."); return; }
            if (MessageBox.Show($"Delete role '{r.Name}'?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (RoleController.DeleteRole(r.Id)) LoadRoles();
                else MessageBox.Show("Delete failed.");
            }
        }
    }
}
