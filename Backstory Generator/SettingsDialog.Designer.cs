namespace Backstory_Generator
{
    partial class SettingsDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxRimWorldPath = new System.Windows.Forms.TextBox();
            this.buttonSetRimWorldPath = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.listBoxIncludedModPaths = new System.Windows.Forms.ListBox();
            this.textBoxIncludedModPaths = new System.Windows.Forms.TextBox();
            this.buttonGetModPath = new System.Windows.Forms.Button();
            this.buttonAddModPath = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.buttonDel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(123, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "RimWorld Directory Path";
            // 
            // textBoxRimWorldPath
            // 
            this.textBoxRimWorldPath.Location = new System.Drawing.Point(16, 30);
            this.textBoxRimWorldPath.Name = "textBoxRimWorldPath";
            this.textBoxRimWorldPath.Size = new System.Drawing.Size(354, 20);
            this.textBoxRimWorldPath.TabIndex = 1;
            this.textBoxRimWorldPath.TextChanged += new System.EventHandler(this.textBoxRimWorldPath_TextChanged);
            // 
            // buttonSetRimWorldPath
            // 
            this.buttonSetRimWorldPath.Location = new System.Drawing.Point(376, 28);
            this.buttonSetRimWorldPath.Name = "buttonSetRimWorldPath";
            this.buttonSetRimWorldPath.Size = new System.Drawing.Size(34, 23);
            this.buttonSetRimWorldPath.TabIndex = 2;
            this.buttonSetRimWorldPath.Text = "...";
            this.buttonSetRimWorldPath.UseVisualStyleBackColor = true;
            this.buttonSetRimWorldPath.Click += new System.EventHandler(this.buttonSetRimWorldPath_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Included Mod Paths";
            // 
            // listBoxIncludedModPaths
            // 
            this.listBoxIncludedModPaths.FormattingEnabled = true;
            this.listBoxIncludedModPaths.Location = new System.Drawing.Point(13, 91);
            this.listBoxIncludedModPaths.Name = "listBoxIncludedModPaths";
            this.listBoxIncludedModPaths.Size = new System.Drawing.Size(357, 82);
            this.listBoxIncludedModPaths.TabIndex = 4;
            // 
            // textBoxIncludedModPaths
            // 
            this.textBoxIncludedModPaths.Location = new System.Drawing.Point(13, 179);
            this.textBoxIncludedModPaths.Name = "textBoxIncludedModPaths";
            this.textBoxIncludedModPaths.Size = new System.Drawing.Size(321, 20);
            this.textBoxIncludedModPaths.TabIndex = 5;
            // 
            // buttonGetModPath
            // 
            this.buttonGetModPath.Location = new System.Drawing.Point(340, 177);
            this.buttonGetModPath.Name = "buttonGetModPath";
            this.buttonGetModPath.Size = new System.Drawing.Size(29, 23);
            this.buttonGetModPath.TabIndex = 6;
            this.buttonGetModPath.Text = "...";
            this.buttonGetModPath.UseVisualStyleBackColor = true;
            this.buttonGetModPath.Click += new System.EventHandler(this.buttonGetModPath_Click);
            // 
            // buttonAddModPath
            // 
            this.buttonAddModPath.Location = new System.Drawing.Point(375, 109);
            this.buttonAddModPath.Name = "buttonAddModPath";
            this.buttonAddModPath.Size = new System.Drawing.Size(35, 23);
            this.buttonAddModPath.TabIndex = 7;
            this.buttonAddModPath.Text = "Add";
            this.buttonAddModPath.UseVisualStyleBackColor = true;
            this.buttonAddModPath.Click += new System.EventHandler(this.buttonAddModPath_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(126, 210);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 8;
            this.button4.Text = "Save";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(220, 210);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 9;
            this.button5.Text = "Cancel";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // buttonDel
            // 
            this.buttonDel.Location = new System.Drawing.Point(375, 138);
            this.buttonDel.Name = "buttonDel";
            this.buttonDel.Size = new System.Drawing.Size(35, 23);
            this.buttonDel.TabIndex = 10;
            this.buttonDel.Text = "Del";
            this.buttonDel.UseVisualStyleBackColor = true;
            this.buttonDel.Click += new System.EventHandler(this.buttonDel_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(422, 245);
            this.Controls.Add(this.buttonDel);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.buttonAddModPath);
            this.Controls.Add(this.buttonGetModPath);
            this.Controls.Add(this.textBoxIncludedModPaths);
            this.Controls.Add(this.listBoxIncludedModPaths);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonSetRimWorldPath);
            this.Controls.Add(this.textBoxRimWorldPath);
            this.Controls.Add(this.label1);
            this.Name = "Form2";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxRimWorldPath;
        private System.Windows.Forms.Button buttonSetRimWorldPath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox listBoxIncludedModPaths;
        private System.Windows.Forms.TextBox textBoxIncludedModPaths;
        private System.Windows.Forms.Button buttonGetModPath;
        private System.Windows.Forms.Button buttonAddModPath;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button buttonDel;
    }
}