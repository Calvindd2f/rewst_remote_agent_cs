namespace Rewst.RemoteAgent.Calvindd2f
{
	public partial class Installer :  System.Windows.Forms.Form
	{
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.installButton = new  System.Windows.Forms.Button();
			this.uninstallButton = new  System.Windows.Forms.Button();
			base.SuspendLayout();
			this.installButton.Font = new  System.Drawing.Font("Microsoft Sans Serif", 14f, 0, 3, 0);
			this.installButton.Location = new  System.Drawing.Point(0x12, 0xF);
			this.installButton.Name = "installButton";
			this.installButton.Size = new  System.Drawing.Size(0xA9, 0x41);
			this.installButton.TabIndex = 0;
			this.installButton.Text = "Install";
			this.installButton.UseVisualStyleBackColor = true;
			this.installButton.Click += new  System.EventHandler(this.installButton_Click);
			this.uninstallButton.Font = new  System.Drawing.Font("Microsoft Sans Serif", 14f, 0, 3, 0);
			this.uninstallButton.Location = new  System.Drawing.Point(0x12, 0x5A);
			this.uninstallButton.Name = "uninstallButton";
			this.uninstallButton.Size = new  System.Drawing.Size(0xA9, 0x41);
			this.uninstallButton.TabIndex = 1;
			this.uninstallButton.Text = "Uninstall";
			this.uninstallButton.UseVisualStyleBackColor = true;
			this.uninstallButton.Click += new  System.EventHandler(this.uninstallButton_Click);
			base.AutoScaleDimensions = new  System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = 1;
			base.ClientSize = new  System.Drawing.Size(0xCA, 0xA8);
			base.Controls.Add(this.uninstallButton);
			base.Controls.Add(this.installButton);
			base.MaximizeBox = false;
			this.MaximumSize = new  System.Drawing.Size(0xDA, 0xCF);
			this.MinimumSize = new  System.Drawing.Size(0xDA, 0xCF);
			base.Name = "Installer";
			this.Text = "Agent";
			base.Load += new  System.EventHandler(this.Installer_Load);
			base.ResumeLayout(false);
		}

		private  System.ComponentModel.IContainer components;

		private  System.Windows.Forms.Button installButton;

		private  System.Windows.Forms.Button uninstallButton;
	}
}


