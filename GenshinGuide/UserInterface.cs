using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace GenshinGuide
{
    public static class UserInterface
    {
        private static TextBox _textBox;
        private static PictureBox _pictureBox;

        public static void Init(PictureBox pictureBox,TextBox textBox)
        {
            _textBox = textBox;
            _pictureBox = pictureBox; 
        }

        public static void SetImage(Bitmap bm)
        {
            MethodInvoker imageAction = delegate { 
                _pictureBox.Image = bm;
                _pictureBox.Refresh();
            };
            _pictureBox.Invoke(imageAction);
            //_pictureBox.Image = bm;
            //_pictureBox.Refresh();
        }

        public static void AddText(string s)
        {
            MethodInvoker textAction = delegate
            {
                _textBox.Text += s;
                _textBox.Refresh();
            };
            _textBox.Invoke(textAction);

            //_textBox.Text += s;
            //_textBox.Refresh();
        }

        public static void Reset()
        {

            MethodInvoker imageAction = delegate { _pictureBox.Image = null; };
            MethodInvoker textAction = delegate { _textBox.Text = ""; };
            _pictureBox.Invoke(imageAction);
            _textBox.Invoke(textAction);

            //_pictureBox.Image = null;
            //_textBox.Text = "";
        }

    }
}
