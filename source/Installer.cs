using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Rewst.RemoteAgent.Calvindd2f.Helpers;
using Rewst.RemoteAgent.Calvindd2f.Properties;

namespace Rewst.RemoteAgent.Calvindd2f
{
    public partial class Installer : Form
    {
        public Installer()
        {
            this.InitializeComponent();
            Icon icon = Icon.FromHandle(Resources.favicon.GetHicon());
            base.Icon = icon;
            icon.Dispose();
        }

        private void UpdateButtonStates()
        {
            if (AgentInstallController.CheckIfInstalled())
            {
                this.installButton.Enabled = false;
                this.uninstallButton.Enabled = true;
                return;
            }
            this.installButton.Enabled = true;
            this.uninstallButton.Enabled = false;
        }

        private void DisableButton(bool disableUninstall)
        {
            if (disableUninstall)
            {
                this.uninstallButton.Enabled = false;
                return;
            }
            this.installButton.Enabled = false;
        }

        private void Installer_Load(object sender, EventArgs e)
        {
            this.UpdateButtonStates();
        }

        private void installButton_Click(object sender, EventArgs e)
        {
            try
            {
                LogUtil.LogInfo("Execute install for agent service", "");
                string tenantName = PE32Binary.GetTenantName();
                AgentInstallController.Install(true, default(int?), tenantName, false);
                MessageBox.Show("Installation Complete!");
            }
            catch (AgentInstallException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex2)
            {
                MessageBox.Show(ex2.ToString(), "Unknown error occured during installation.");
            }
            finally
            {
                this.UpdateButtonStates();
            }
        }

        private void uninstallButton_Click(object sender, EventArgs e)
        {
            try
            {
                LogUtil.LogInfo("Execute uninstall for agent service", "");
                this.DisableButton(true);
                if (AgentInstallController.Uninstall(true, true))
                {
                    MessageBox.Show("Uninstall Complete!");
                }
                else
                {
                    MessageBox.Show("Uninstall did not Complete!");
                }
            }
            catch (AgentInstallException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex2)
            {
                MessageBox.Show(ex2.ToString(), "Unknown error occured during uninstall.");
            }
            finally
            {
                this.UpdateButtonStates();
            }
        }
    }
}
