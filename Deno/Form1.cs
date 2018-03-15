using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Deno
{
    public partial class Form1 : Form
    {
        class RegistryProperties
        {
            private static readonly RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            private static readonly string appName = Application.ProductName.ToString();

            private bool _runOnStartup = !string.IsNullOrEmpty((string)rk.GetValue(appName));

            public Boolean RunOnStartup
            {
                get => _runOnStartup;
                set
                {
                    _runOnStartup = value;
                    if (value) rk.SetValue(appName, Application.ExecutablePath.ToString());
                    else rk.DeleteValue(appName, false);
                }
            }
        }

        private RegistryProperties registryProperties = new RegistryProperties();

        public Form1()
        {
            InitializeComponent();

            // window dragging
            this.bottomBar.MouseDown += (object s, MouseEventArgs e) =>
            {
                if (e.Button == MouseButtons.Left)
                    GuiHelper.StartDragging(this);
            };

            // bottom bar visibility
            this.textBox1.KeyUp += (s, e) => bottomBar.Visible = false;
            this.Activated += (s, e) => bottomBar.Visible = true;
            //this.textBox1.Click += (s, e) => bottomBar.Visible = true;
            this.Deactivate += (s, e) => bottomBar.Visible = false;

            // text, position and window size saving
            this.Deactivate += (s, e) => SaveProperties();
            this.FormClosing += (s, e) => SaveProperties();

            // notification area icon actions
            this.notifyIcon1.Click += (s, e) => this.Activate();

            // menu actions
            this.closeToolStripMenuItem.Click += (s, e) => this.Close();
            
            // menu->run-on-startup checkbox 
            runOnStartupCB.Checked = registryProperties.RunOnStartup;
            this.runOnStartupCB.CheckedChanged += (s, e) => registryProperties.RunOnStartup = runOnStartupCB.Checked;

            // state loading
            RestoreProperties();
            this.textBox1.Select(int.MaxValue, 0);  // unselect text and move cursor to the end
        }

        private void RestoreProperties()
        {
            var a = new Properties.Settings();
            this.Size = a.WindowSize;
            this.SetDesktopLocation(a.WindowLocation.X, a.WindowLocation.Y);
            textBox1.Text = a.Text;
        }

        private void SaveProperties() =>
            new Properties.Settings
            {
                WindowSize = this.Size,
                WindowLocation = this.PointToScreen(new System.Drawing.Point(0, 0)),
                Text = textBox1.Text
            }.Save();
    }
}
