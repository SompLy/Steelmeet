using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteelMeet
{
    internal class Fullscreen
    {
        public void ToggleFullscreen( bool _isFullscreen, Form _form)
        {
            if ( _isFullscreen )
            {
                _form.FormBorderStyle = FormBorderStyle.Fixed3D;
                _form.WindowState = FormWindowState.Normal;
            }
            else
            {
                _form.FormBorderStyle = FormBorderStyle.None;
                _form.WindowState = FormWindowState.Maximized;
            }
        }
    }
}
