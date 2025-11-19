namespace boilerplate_desktop_dotnet.Views.Roles
{
    partial class UserRoleListForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvUserRoles = new System.Windows.Forms.DataGridView();
            this.cboUsers = new System.Windows.Forms.ComboBox();
            this.cboRoles = new System.Windows.Forms.ComboBox();
            this.btnAssign = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUserRoles)).BeginInit();
            this.SuspendLayout();
            // 
            // cboUsers
            //
            this.cboUsers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboUsers.Location = new System.Drawing.Point(12, 12);
            this.cboUsers.Width = 250;
            // 
            // cboRoles
            //
            this.cboRoles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRoles.Location = new System.Drawing.Point(280, 12);
            this.cboRoles.Width = 200;
            // 
            // btnAssign
            //
            this.btnAssign.Location = new System.Drawing.Point(500, 10);
            this.btnAssign.Text = "Assign";
            this.btnAssign.Click += new System.EventHandler(this.btnAssign_Click);
            // 
            // btnRemove
            //
            this.btnRemove.Location = new System.Drawing.Point(590, 10);
            this.btnRemove.Text = "Remove";
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnClose
            //
            this.btnClose.Location = new System.Drawing.Point(680, 10);
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // dgvUserRoles
            // 
            this.dgvUserRoles.Location = new System.Drawing.Point(12, 50);
            this.dgvUserRoles.Size = new System.Drawing.Size(760, 380);
            this.dgvUserRoles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvUserRoles.ReadOnly = true;
            this.dgvUserRoles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // UserRoleListForm
            // 
            this.ClientSize = new System.Drawing.Size(784, 450);
            this.Controls.Add(this.dgvUserRoles);
            this.Controls.Add(this.cboUsers);
            this.Controls.Add(this.cboRoles);
            this.Controls.Add(this.btnAssign);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnClose);
            this.Text = "User Roles";
            ((System.ComponentModel.ISupportInitialize)(this.dgvUserRoles)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.DataGridView dgvUserRoles;
        private System.Windows.Forms.ComboBox cboUsers;
        private System.Windows.Forms.ComboBox cboRoles;
        private System.Windows.Forms.Button btnAssign;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnClose;
    }
}