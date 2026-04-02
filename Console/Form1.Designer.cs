namespace Console
{
    partial class Form1
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
            this.tb_input1 = new System.Windows.Forms.TextBox();
            this.rtb_output = new System.Windows.Forms.RichTextBox();
            this.btn_submit = new System.Windows.Forms.Button();
            this.lb_path = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tb_input1
            // 
            this.tb_input1.Location = new System.Drawing.Point(140, 416);
            this.tb_input1.Name = "tb_input1";
            this.tb_input1.Size = new System.Drawing.Size(648, 22);
            this.tb_input1.TabIndex = 0;
            // 
            // rtb_output
            // 
            this.rtb_output.BackColor = System.Drawing.Color.White;
            this.rtb_output.Enabled = false;
            this.rtb_output.ForeColor = System.Drawing.Color.Black;
            this.rtb_output.Location = new System.Drawing.Point(12, 12);
            this.rtb_output.Name = "rtb_output";
            this.rtb_output.Size = new System.Drawing.Size(776, 398);
            this.rtb_output.TabIndex = 1;
            this.rtb_output.Text = "";
            // 
            // btn_submit
            // 
            this.btn_submit.Location = new System.Drawing.Point(12, 416);
            this.btn_submit.Name = "btn_submit";
            this.btn_submit.Size = new System.Drawing.Size(122, 23);
            this.btn_submit.TabIndex = 2;
            this.btn_submit.Text = "Enter";
            this.btn_submit.UseVisualStyleBackColor = true;
            this.btn_submit.Click += new System.EventHandler(this.btn_submit_Click);
            // 
            // lb_path
            // 
            this.lb_path.AutoSize = true;
            this.lb_path.Location = new System.Drawing.Point(137, 447);
            this.lb_path.Name = "lb_path";
            this.lb_path.Size = new System.Drawing.Size(44, 16);
            this.lb_path.TabIndex = 3;
            this.lb_path.Text = "label1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 472);
            this.Controls.Add(this.lb_path);
            this.Controls.Add(this.btn_submit);
            this.Controls.Add(this.rtb_output);
            this.Controls.Add(this.tb_input1);
            this.Name = "Form1";
            this.Text = "GameConsole";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tb_input1;
        private System.Windows.Forms.RichTextBox rtb_output;
        private System.Windows.Forms.Button btn_submit;
        private System.Windows.Forms.Label lb_path;
    }
}

