using System.ComponentModel;

namespace CheatLib.GUI
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.materialTabControl1 = new MaterialSkin.Controls.MaterialTabControl();
            this.ESP = new System.Windows.Forms.TabPage();
            this.materialCheckedListBox1 = new MaterialSkin.Controls.MaterialCheckedListBox();
            this.materialCheckbox1 = new MaterialSkin.Controls.MaterialCheckbox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.materialCheckbox2 = new MaterialSkin.Controls.MaterialCheckbox();
            this.materialTabControl1.SuspendLayout();
            this.ESP.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // materialTabControl1
            // 
            this.materialTabControl1.Controls.Add(this.ESP);
            this.materialTabControl1.Controls.Add(this.tabPage2);
            this.materialTabControl1.Depth = 0;
            this.materialTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.materialTabControl1.Location = new System.Drawing.Point(3, 64);
            this.materialTabControl1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialTabControl1.Multiline = true;
            this.materialTabControl1.Name = "materialTabControl1";
            this.materialTabControl1.SelectedIndex = 0;
            this.materialTabControl1.Size = new System.Drawing.Size(794, 383);
            this.materialTabControl1.TabIndex = 0;
            // 
            // ESP
            // 
            this.ESP.Controls.Add(this.materialCheckedListBox1);
            this.ESP.Controls.Add(this.materialCheckbox1);
            this.ESP.Location = new System.Drawing.Point(4, 22);
            this.ESP.Name = "ESP";
            this.ESP.Padding = new System.Windows.Forms.Padding(3);
            this.ESP.Size = new System.Drawing.Size(786, 357);
            this.ESP.TabIndex = 0;
            this.ESP.Text = "tabPage1";
            this.ESP.UseVisualStyleBackColor = true;
            // 
            // materialCheckedListBox1
            // 
            this.materialCheckedListBox1.AutoScroll = true;
            this.materialCheckedListBox1.BackColor = System.Drawing.Color.Transparent;
            this.materialCheckedListBox1.Depth = 0;
            this.materialCheckedListBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.materialCheckedListBox1.Location = new System.Drawing.Point(3, 38);
            this.materialCheckedListBox1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialCheckedListBox1.Name = "materialCheckedListBox1";
            this.materialCheckedListBox1.Size = new System.Drawing.Size(780, 316);
            this.materialCheckedListBox1.Striped = false;
            this.materialCheckedListBox1.StripeDarkColor = System.Drawing.Color.Empty;
            this.materialCheckedListBox1.TabIndex = 1;
            // 
            // materialCheckbox1
            // 
            this.materialCheckbox1.Depth = 0;
            this.materialCheckbox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.materialCheckbox1.Location = new System.Drawing.Point(3, 3);
            this.materialCheckbox1.Margin = new System.Windows.Forms.Padding(0);
            this.materialCheckbox1.MouseLocation = new System.Drawing.Point(-1, -1);
            this.materialCheckbox1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialCheckbox1.Name = "materialCheckbox1";
            this.materialCheckbox1.ReadOnly = false;
            this.materialCheckbox1.Ripple = true;
            this.materialCheckbox1.Size = new System.Drawing.Size(780, 35);
            this.materialCheckbox1.TabIndex = 0;
            this.materialCheckbox1.Text = "Enable ESP";
            this.materialCheckbox1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.materialCheckbox2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(786, 357);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Player";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // materialCheckbox2
            // 
            this.materialCheckbox2.Depth = 0;
            this.materialCheckbox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.materialCheckbox2.Location = new System.Drawing.Point(3, 3);
            this.materialCheckbox2.Margin = new System.Windows.Forms.Padding(0);
            this.materialCheckbox2.MouseLocation = new System.Drawing.Point(-1, -1);
            this.materialCheckbox2.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialCheckbox2.Name = "materialCheckbox2";
            this.materialCheckbox2.ReadOnly = false;
            this.materialCheckbox2.Ripple = true;
            this.materialCheckbox2.Size = new System.Drawing.Size(780, 43);
            this.materialCheckbox2.TabIndex = 0;
            this.materialCheckbox2.Text = "Enable auto F";
            this.materialCheckbox2.UseVisualStyleBackColor = true;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.materialTabControl1);
            this.Name = "Main";
            this.Text = "Main";
            this.materialTabControl1.ResumeLayout(false);
            this.ESP.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private MaterialSkin.Controls.MaterialCheckbox materialCheckbox2;

        private MaterialSkin.Controls.MaterialCheckbox materialCheckbox1;
        private MaterialSkin.Controls.MaterialCheckedListBox materialCheckedListBox1;

        private MaterialSkin.Controls.MaterialTabControl materialTabControl1;
        private System.Windows.Forms.TabPage ESP;
        private System.Windows.Forms.TabPage tabPage2;

        #endregion
    }
}