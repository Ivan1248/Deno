using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Deno
{
    class DenoTextBox : TextBox
    {
        private int _tabWidth;

        public DenoTextBox() : base() => TabWidth = 1;

        public int TabWidth
        {
            get => _tabWidth;
            set => SetTabWidth(value);
        }

        private void SetTabWidth(int tabWidth)
        {
            const int EM_SETTABSTOPS = 0x00CB;
            SendMessage(this.Handle, EM_SETTABSTOPS, 1, new int[] { tabWidth * 4 });
            _tabWidth = tabWidth;
        }

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr h, int msg, int wParam, int[] lParam);
    }
}
