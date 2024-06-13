namespace Crusader_Wars
{
    partial class LoadingScreen
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
            this.Label_Message = new System.Windows.Forms.Label();
            this.GIF = new System.Windows.Forms.PictureBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.MajorLayout = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.GIF)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.MajorLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // Label_Message
            // 
            this.Label_Message.AutoSize = true;
            this.Label_Message.BackColor = System.Drawing.Color.Transparent;
            this.Label_Message.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label_Message.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Label_Message.Font = new System.Drawing.Font("Paradox King Script", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label_Message.ForeColor = System.Drawing.Color.White;
            this.Label_Message.Location = new System.Drawing.Point(3, 0);
            this.Label_Message.Name = "Label_Message";
            this.Label_Message.Size = new System.Drawing.Size(156, 42);
            this.Label_Message.TabIndex = 0;
            this.Label_Message.Text = "Message";
            this.Label_Message.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // GIF
            // 
            this.GIF.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.GIF.BackColor = System.Drawing.Color.Transparent;
            this.GIF.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.GIF.Image = global::Crusader_Wars.Properties.Resources.rotating_loading;
            this.GIF.Location = new System.Drawing.Point(1796, 3);
            this.GIF.Name = "GIF";
            this.GIF.Size = new System.Drawing.Size(121, 94);
            this.GIF.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.GIF.TabIndex = 1;
            this.GIF.TabStop = false;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.BackgroundImage = global::Crusader_Wars.Properties.Resources.progress_orange;
            this.flowLayoutPanel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.flowLayoutPanel1.Controls.Add(this.Label_Message);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(1628, 29);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(162, 42);
            this.flowLayoutPanel1.TabIndex = 3;
            this.flowLayoutPanel1.WrapContents = false;
            // 
            // MajorLayout
            // 
            this.MajorLayout.AutoSize = true;
            this.MajorLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MajorLayout.BackColor = System.Drawing.Color.Transparent;
            this.MajorLayout.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.MajorLayout.Controls.Add(this.GIF);
            this.MajorLayout.Controls.Add(this.flowLayoutPanel1);
            this.MajorLayout.Dock = System.Windows.Forms.DockStyle.Top;
            this.MajorLayout.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.MajorLayout.Location = new System.Drawing.Point(0, 0);
            this.MajorLayout.Name = "MajorLayout";
            this.MajorLayout.Size = new System.Drawing.Size(1920, 100);
            this.MajorLayout.TabIndex = 4;
            this.MajorLayout.WrapContents = false;
            // 
            // LoadingScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.BackgroundImage = global::Crusader_Wars.Properties.Resources.warfare_loadingscreen_smallerlogo;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1920, 1000);
            this.ControlBox = false;
            this.Controls.Add(this.MajorLayout);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LoadingScreen";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Crusader Wars";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Shown += new System.EventHandler(this.LoadingScreen_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.GIF)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.MajorLayout.ResumeLayout(false);
            this.MajorLayout.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Label_Message;
        private System.Windows.Forms.PictureBox GIF;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel MajorLayout;
    }
}