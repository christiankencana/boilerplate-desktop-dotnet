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
    public partial class RoleForm : Form
    {
        private int? _roleId;

        public RoleForm(int? roleId = null)
        {
            InitializeComponent();
            _roleId = roleId;
            if (_roleId.HasValue) LoadRole(_roleId.Value);
            else BuildBasicUi();
        }

        private void BuildBasicUi()
        {
            if (Controls["txtName"] != null) return;
            var lblName = new Label { Text = "Name", Top = 10, Left = 10 };
            var txtName = new TextBox { Name = "txtName", Top = 30, Left = 10, Width = 300 };
            var lblDesc = new Label { Text = "Description", Top = 65, Left = 10 };
            var txtDesc = new TextBox { Name = "txtDesc", Top = 85, Left = 10, Width = 300, Height = 80, Multiline = true };
            var btnSave = new Button { Text = "Save", Top = 180, Left = 10 };
            btnSave.Click += BtnSave_Click;
            Controls.Add(lblName); Controls.Add(txtName);
            Controls.Add(lblDesc); Controls.Add(txtDesc);
            Controls.Add(btnSave);
        }

        private void LoadRole(int id)
        {
            var r = RoleController.GetRoleById(id);
            if (r == null) return;
            BuildBasicUi();
            Controls["txtName"].Text = r.Name;
            Controls["txtDesc"].Text = r.Description;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var name = Controls["txtName"].Text.Trim();
            var desc = Controls["txtDesc"].Text.Trim();

            if (string.IsNullOrEmpty(name)) { MessageBox.Show("Name required."); return; }

            if (_roleId.HasValue)
            {
                var r = new Role { Id = _roleId.Value, Name = name, Description = desc };
                if (!RoleController.UpdateRole(r)) { MessageBox.Show("Update failed."); return; }
            }
            else
            {
                var r = new Role { Name = name, Description = desc };
                if (!RoleController.CreateRole(r)) { MessageBox.Show("Create failed."); return; }
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
