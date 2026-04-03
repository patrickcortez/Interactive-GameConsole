namespace Console
{
    partial class Editior
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
            this.rtb_edit = new System.Windows.Forms.RichTextBox();
            this.mn_editor = new System.Windows.Forms.MenuStrip();
            this.menuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutConsoleExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.themesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.darkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.mn_editor.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtb_edit
            // 
            this.rtb_edit.Location = new System.Drawing.Point(12, 49);
            this.rtb_edit.Name = "rtb_edit";
            this.rtb_edit.Size = new System.Drawing.Size(763, 446);
            this.rtb_edit.TabIndex = 0;
            this.rtb_edit.Text = "";
            // 
            // mn_editor
            // 
            this.mn_editor.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
            this.mn_editor.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mn_editor.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.themesToolStripMenuItem});
            this.mn_editor.Location = new System.Drawing.Point(0, 0);
            this.mn_editor.Name = "mn_editor";
            this.mn_editor.Size = new System.Drawing.Size(787, 28);
            this.mn_editor.TabIndex = 1;
            this.mn_editor.Text = "menuStrip1";
            // 
            // menuToolStripMenuItem
            // 
            this.menuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.menuToolStripMenuItem.Name = "menuToolStripMenuItem";
            this.menuToolStripMenuItem.Size = new System.Drawing.Size(52, 24);
            this.menuToolStripMenuItem.Text = "Files";
            this.menuToolStripMenuItem.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.saveAsToolStripMenuItem.Text = "Save As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutConsoleExplorerToolStripMenuItem});
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(64, 24);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // aboutConsoleExplorerToolStripMenuItem
            // 
            this.aboutConsoleExplorerToolStripMenuItem.Name = "aboutConsoleExplorerToolStripMenuItem";
            this.aboutConsoleExplorerToolStripMenuItem.Size = new System.Drawing.Size(249, 26);
            this.aboutConsoleExplorerToolStripMenuItem.Text = "About Console Explorer";
            this.aboutConsoleExplorerToolStripMenuItem.Click += new System.EventHandler(this.aboutConsoleExplorerToolStripMenuItem_Click);
            // 
            // themesToolStripMenuItem
            // 
            this.themesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lightToolStripMenuItem,
            this.darkToolStripMenuItem});
            this.themesToolStripMenuItem.Name = "themesToolStripMenuItem";
            this.themesToolStripMenuItem.Size = new System.Drawing.Size(74, 24);
            this.themesToolStripMenuItem.Text = "Themes";
            // 
            // lightToolStripMenuItem
            // 
            this.lightToolStripMenuItem.Name = "lightToolStripMenuItem";
            this.lightToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.lightToolStripMenuItem.Text = "Light";
            this.lightToolStripMenuItem.Click += new System.EventHandler(this.lightToolStripMenuItem_Click);
            // 
            // darkToolStripMenuItem
            // 
            this.darkToolStripMenuItem.Name = "darkToolStripMenuItem";
            this.darkToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.darkToolStripMenuItem.Text = "Dark";
            this.darkToolStripMenuItem.Click += new System.EventHandler(this.darkToolStripMenuItem_Click);
            // 
            // Editior
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(787, 507);
            this.ControlBox = false;
            this.Controls.Add(this.rtb_edit);
            this.Controls.Add(this.mn_editor);
            this.Name = "Editior";
            this.Text = "Text Editior";
            this.mn_editor.ResumeLayout(false);
            this.mn_editor.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtb_edit;
        private System.Windows.Forms.MenuStrip mn_editor;
        private System.Windows.Forms.ToolStripMenuItem menuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutConsoleExplorerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem themesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lightToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem darkToolStripMenuItem;
        private System.Windows.Forms.ColorDialog colorDialog1;
    }
}