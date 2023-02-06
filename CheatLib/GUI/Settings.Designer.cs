using System.ComponentModel;

namespace CheatLib
{
    partial class Settings
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.scrollableControl1 = new System.Windows.Forms.ScrollableControl();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.enable_esp = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.max_zoom = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.auto_treassure = new System.Windows.Forms.CheckBox();
            this.auto_pickup = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.scrollableControl1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // scrollableControl1
            // 
            this.scrollableControl1.Controls.Add(this.groupBox3);
            this.scrollableControl1.Controls.Add(this.groupBox2);
            this.scrollableControl1.Controls.Add(this.groupBox1);
            this.scrollableControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scrollableControl1.Location = new System.Drawing.Point(0, 0);
            this.scrollableControl1.Name = "scrollableControl1";
            this.scrollableControl1.Size = new System.Drawing.Size(352, 314);
            this.scrollableControl1.TabIndex = 0;
            this.scrollableControl1.Text = "scrollableControl1";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.enable_esp);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(0, 141);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(352, 173);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "ESP";
            // 
            // enable_esp
            // 
            this.enable_esp.Cursor = System.Windows.Forms.Cursors.Help;
            this.enable_esp.Dock = System.Windows.Forms.DockStyle.Top;
            this.enable_esp.Location = new System.Drawing.Point(3, 16);
            this.enable_esp.Name = "enable_esp";
            this.enable_esp.Size = new System.Drawing.Size(346, 24);
            this.enable_esp.TabIndex = 0;
            this.enable_esp.Text = "checkBox1";
            this.enable_esp.UseVisualStyleBackColor = true;
            this.enable_esp.CheckedChanged += new System.EventHandler(this.enable_esp_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tableLayoutPanel1);
            this.groupBox2.Controls.Add(this.max_zoom);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 67);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(352, 74);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Camera zoom";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.trackBar1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 33);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(346, 35);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // trackBar1
            // 
            this.trackBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackBar1.LargeChange = 150;
            this.trackBar1.Location = new System.Drawing.Point(3, 3);
            this.trackBar1.Maximum = 5000;
            this.trackBar1.Minimum = 1000;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(167, 29);
            this.trackBar1.SmallChange = 50;
            this.trackBar1.TabIndex = 0;
            this.trackBar1.TickFrequency = 100;
            this.trackBar1.Value = 1000;
            this.trackBar1.ValueChanged += new System.EventHandler(this.setMaxZoomValue);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.Location = new System.Drawing.Point(176, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(167, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "label1";
            // 
            // max_zoom
            // 
            this.max_zoom.Cursor = System.Windows.Forms.Cursors.Help;
            this.max_zoom.Dock = System.Windows.Forms.DockStyle.Top;
            this.max_zoom.Location = new System.Drawing.Point(3, 16);
            this.max_zoom.Name = "max_zoom";
            this.max_zoom.Size = new System.Drawing.Size(346, 17);
            this.max_zoom.TabIndex = 0;
            this.max_zoom.Text = "checkBox1";
            this.max_zoom.UseVisualStyleBackColor = true;
            this.max_zoom.CheckedChanged += new System.EventHandler(this.max_zoom_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.auto_treassure);
            this.groupBox1.Controls.Add(this.auto_pickup);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(352, 67);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Auto loot";
            // 
            // auto_treassure
            // 
            this.auto_treassure.Cursor = System.Windows.Forms.Cursors.Help;
            this.auto_treassure.Dock = System.Windows.Forms.DockStyle.Top;
            this.auto_treassure.Location = new System.Drawing.Point(3, 40);
            this.auto_treassure.Name = "auto_treassure";
            this.auto_treassure.Size = new System.Drawing.Size(346, 21);
            this.auto_treassure.TabIndex = 3;
            this.auto_treassure.Text = "auto treassure";
            this.auto_treassure.UseVisualStyleBackColor = true;
            this.auto_treassure.CheckedChanged += new System.EventHandler(this.auto_treassure_CheckedChanged);
            // 
            // auto_pickup
            // 
            this.auto_pickup.Cursor = System.Windows.Forms.Cursors.Help;
            this.auto_pickup.Dock = System.Windows.Forms.DockStyle.Top;
            this.auto_pickup.Location = new System.Drawing.Point(3, 16);
            this.auto_pickup.Name = "auto_pickup";
            this.auto_pickup.Size = new System.Drawing.Size(346, 24);
            this.auto_pickup.TabIndex = 0;
            this.auto_pickup.Text = "auto pickup";
            this.auto_pickup.UseVisualStyleBackColor = true;
            this.auto_pickup.CheckedChanged += new System.EventHandler(this.auto_pickup_CheckedChanged);
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.scrollableControl1);
            this.Name = "Settings";
            this.Size = new System.Drawing.Size(352, 314);
            this.scrollableControl1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.CheckBox auto_treassure;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox enable_esp;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox max_zoom;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox auto_pickup;
        private System.Windows.Forms.ToolTip toolTip1;

        private System.Windows.Forms.ScrollableControl scrollableControl1;

        #endregion
    }
}