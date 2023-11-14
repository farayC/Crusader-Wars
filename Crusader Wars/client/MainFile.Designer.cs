namespace Crusader_Wars
{
    partial class HomePage
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HomePage));
            this.ExecuteButton = new System.Windows.Forms.Button();
            this.btt_debug = new System.Windows.Forms.Button();
            this.infoLabel = new System.Windows.Forms.Label();
            this.SettingsBtn = new System.Windows.Forms.Button();
            this.Info_Status = new System.Windows.Forms.PictureBox();
            this.InformationToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.labelVersion = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnDiscord = new System.Windows.Forms.Button();
            this.btnSteam = new System.Windows.Forms.Button();
            this.btnPatch = new System.Windows.Forms.Button();
            this.btnWebsite = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.Info_Status)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ExecuteButton
            // 
            resources.ApplyResources(this.ExecuteButton, "ExecuteButton");
            this.ExecuteButton.BackColor = System.Drawing.Color.Transparent;
            this.ExecuteButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ExecuteButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.ExecuteButton.FlatAppearance.BorderSize = 0;
            this.ExecuteButton.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ExecuteButton.Image = global::Crusader_Wars.Properties.Resources.Declare_war_interaction;
            this.ExecuteButton.Name = "ExecuteButton";
            this.ExecuteButton.UseVisualStyleBackColor = false;
            this.ExecuteButton.Click += new System.EventHandler(this.ExecuteButton_Click);
            this.ExecuteButton.MouseHover += new System.EventHandler(this.ExecuteButton_MouseHover);
            // 
            // btt_debug
            // 
            resources.ApplyResources(this.btt_debug, "btt_debug");
            this.btt_debug.BackColor = System.Drawing.Color.Blue;
            this.btt_debug.Name = "btt_debug";
            this.btt_debug.UseVisualStyleBackColor = false;
            this.btt_debug.Click += new System.EventHandler(this.btt_debug_Click);
            // 
            // infoLabel
            // 
            resources.ApplyResources(this.infoLabel, "infoLabel");
            this.infoLabel.BackColor = System.Drawing.Color.Transparent;
            this.infoLabel.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.infoLabel.Name = "infoLabel";
            // 
            // SettingsBtn
            // 
            resources.ApplyResources(this.SettingsBtn, "SettingsBtn");
            this.SettingsBtn.BackColor = System.Drawing.Color.Transparent;
            this.SettingsBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SettingsBtn.FlatAppearance.BorderSize = 0;
            this.SettingsBtn.Image = global::Crusader_Wars.Properties.Resources.imageedit_2_3450978699;
            this.SettingsBtn.Name = "SettingsBtn";
            this.SettingsBtn.UseVisualStyleBackColor = false;
            this.SettingsBtn.Click += new System.EventHandler(this.SettingsBtn_Click);
            // 
            // Info_Status
            // 
            resources.ApplyResources(this.Info_Status, "Info_Status");
            this.Info_Status.BackColor = System.Drawing.Color.Transparent;
            this.Info_Status.BackgroundImage = global::Crusader_Wars.Properties.Resources.info_smaller;
            this.Info_Status.Name = "Info_Status";
            this.Info_Status.TabStop = false;
            this.Info_Status.MouseHover += new System.EventHandler(this.Info_Status_MouseHover);
            // 
            // labelVersion
            // 
            resources.ApplyResources(this.labelVersion, "labelVersion");
            this.labelVersion.BackColor = System.Drawing.Color.Transparent;
            this.labelVersion.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.labelVersion.Name = "labelVersion";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Image = global::Crusader_Wars.Properties.Resources.Sem_nome;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.flowLayoutPanel1.Controls.Add(this.ExecuteButton);
            this.flowLayoutPanel1.Controls.Add(this.infoLabel);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // btnDiscord
            // 
            resources.ApplyResources(this.btnDiscord, "btnDiscord");
            this.btnDiscord.BackColor = System.Drawing.Color.Transparent;
            this.btnDiscord.BackgroundImage = global::Crusader_Wars.Properties.Resources.discord2;
            this.btnDiscord.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDiscord.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnDiscord.FlatAppearance.BorderSize = 0;
            this.btnDiscord.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnDiscord.Name = "btnDiscord";
            this.btnDiscord.UseVisualStyleBackColor = false;
            this.btnDiscord.Click += new System.EventHandler(this.btnDiscord_Click);
            this.btnDiscord.MouseHover += new System.EventHandler(this.btnDiscord_MouseHover);
            // 
            // btnSteam
            // 
            resources.ApplyResources(this.btnSteam, "btnSteam");
            this.btnSteam.BackColor = System.Drawing.Color.Transparent;
            this.btnSteam.BackgroundImage = global::Crusader_Wars.Properties.Resources.steam;
            this.btnSteam.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSteam.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnSteam.FlatAppearance.BorderSize = 0;
            this.btnSteam.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnSteam.Name = "btnSteam";
            this.btnSteam.UseVisualStyleBackColor = false;
            this.btnSteam.Click += new System.EventHandler(this.btnSteam_Click);
            this.btnSteam.MouseHover += new System.EventHandler(this.btnSteam_MouseHover);
            // 
            // btnPatch
            // 
            resources.ApplyResources(this.btnPatch, "btnPatch");
            this.btnPatch.BackColor = System.Drawing.Color.Transparent;
            this.btnPatch.BackgroundImage = global::Crusader_Wars.Properties.Resources.patch;
            this.btnPatch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPatch.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnPatch.FlatAppearance.BorderSize = 0;
            this.btnPatch.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnPatch.Name = "btnPatch";
            this.btnPatch.UseVisualStyleBackColor = false;
            this.btnPatch.Click += new System.EventHandler(this.btnPatch_Click);
            this.btnPatch.MouseHover += new System.EventHandler(this.btnPatch_MouseHover);
            // 
            // btnWebsite
            // 
            resources.ApplyResources(this.btnWebsite, "btnWebsite");
            this.btnWebsite.BackColor = System.Drawing.Color.Transparent;
            this.btnWebsite.BackgroundImage = global::Crusader_Wars.Properties.Resources.website1;
            this.btnWebsite.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnWebsite.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnWebsite.FlatAppearance.BorderSize = 0;
            this.btnWebsite.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnWebsite.Name = "btnWebsite";
            this.btnWebsite.UseVisualStyleBackColor = false;
            this.btnWebsite.Click += new System.EventHandler(this.btnWebsite_Click);
            this.btnWebsite.MouseHover += new System.EventHandler(this.btnWebsite_MouseHover);
            // 
            // HomePage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.IndianRed;
            this.BackgroundImage = global::Crusader_Wars.Properties.Resources.wider_original;
            this.Controls.Add(this.btnWebsite);
            this.Controls.Add(this.btnPatch);
            this.Controls.Add(this.btnSteam);
            this.Controls.Add(this.btnDiscord);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.Info_Status);
            this.Controls.Add(this.SettingsBtn);
            this.Controls.Add(this.btt_debug);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "HomePage";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HomePage_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.HomePage_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.Info_Status)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ExecuteButton;
        private System.Windows.Forms.Button btt_debug;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.Button SettingsBtn;
        private System.Windows.Forms.PictureBox Info_Status;
        private System.Windows.Forms.ToolTip InformationToolTip;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnDiscord;
        private System.Windows.Forms.Button btnSteam;
        private System.Windows.Forms.Button btnPatch;
        private System.Windows.Forms.Button btnWebsite;
    }
}

