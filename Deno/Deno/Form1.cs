using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;

namespace Mideno
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            SetTabWidth(textBox1, 3);
            RestoreLastPositionAndSize();
            this.FormClosing += (s,e) => SavePositionAndSize();

            runOnStartupCB.Checked = !string.IsNullOrEmpty((string)rk.GetValue("Mideno"));
        }

        private void RestoreLastPositionAndSize()
        {
            var a = new Deno.Properties.Settings();
            this.Size = a.WindowSize;
            this.SetDesktopLocation(a.WindowLocation.X, a.WindowLocation.Y);
            textBox1.Text = a.Text;
            textBox1.Select(int.MaxValue, 0);
        }

        private void SavePositionAndSize()
        {
            var a = new Deno.Properties.Settings
            {
                WindowSize = this.Size,
                WindowLocation = this.PointToScreen(new System.Drawing.Point(0, 0)),
                Text = textBox1.Text
            };
            a.Save();
        }

        private void Form1_Activated(object sender, EventArgs e) => statusStrip1.Visible = true;

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            statusStrip1.Visible = false;
            SavePositionAndSize();
        }

        private void textBox2_KeyUp(object sender, KeyEventArgs e) => statusStrip1.Visible = false;

        // set tab stops to a width of 4
        private const int EM_SETTABSTOPS = 0x00CB;

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr h, int msg, int wParam, int[] lParam);

        public static void SetTabWidth(TextBox textbox, int tabWidth) =>
            SendMessage(textbox.Handle, EM_SETTABSTOPS, 1, new[] { tabWidth * 4 });

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void statusStrip1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e) => this.Close();

        readonly RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        private void runOnStartupCB_CheckedChanged(object sender, EventArgs e)
        {
            if (runOnStartupCB.Checked)
                rk.SetValue("Mideno", Application.ExecutablePath.ToString());
            else
                rk.DeleteValue("Mideno", false);
        }

        private void notifyIcon1_Click(object sender, EventArgs e) => this.Activate();
    }
}
