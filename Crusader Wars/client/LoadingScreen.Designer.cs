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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.GIF)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // Label_Message
            // 
            this.Label_Message.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Label_Message.AutoSize = true;
            this.Label_Message.BackColor = System.Drawing.Color.Transparent;
            this.Label_Message.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Label_Message.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Label_Message.Font = new System.Drawing.Font("Paradox King Script", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label_Message.ForeColor = System.Drawing.Color.White;
            this.Label_Message.Location = new System.Drawing.Point(924, 716);
            this.Label_Message.Name = "Label_Message";
            this.Label_Message.Size = new System.Drawing.Size(158, 44);
            this.Label_Message.TabIndex = 0;
            this.Label_Message.Text = "Message";
            // 
            // GIF
            // 
            this.GIF.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GIF.BackColor = System.Drawing.Color.Transparent;
            this.GIF.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.GIF.Image = global::Crusader_Wars.Properties.Resources.rotating_logo1;
            this.GIF.Location = new System.Drawing.Point(868, 716);
            this.GIF.Name = "GIF";
            this.GIF.Size = new System.Drawing.Size(50, 45);
            this.GIF.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.GIF.TabIndex = 1;
            this.GIF.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Crusader_Wars.Properties.Resources.loadingscreen1;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1919, 1000);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // LoadingScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.BackgroundImage = global::Crusader_Wars.Properties.Resources.loadingscreen1;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1920, 1000);
            this.ControlBox = false;
            this.Controls.Add(this.GIF);
            this.Controls.Add(this.Label_Message);
            this.Controls.Add(this.pictureBox1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LoadingScreen";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Crusader Wars";
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.GIF)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Label_Message;
        private System.Windows.Forms.PictureBox GIF;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}