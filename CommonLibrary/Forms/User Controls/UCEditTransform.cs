using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CommonLibrary
{
    public partial class UCEditTransform : System.Windows.Forms.UserControl
    {
        #region Properties

        public string TransformName
        {
            get { return groupBox1.Text; }
            set { groupBox1.Text = value; }
        }

        bool _uniformSupport;
        public bool UniformSupport
        {
            get { return _uniformSupport; }
            set
            {
                _uniformSupport = value;
                if (_uniformSupport)
                    ShowCheckBox();
                else
                    HideCheckBox();
            }
        }

        #endregion

        #region Event

        public delegate void SubmitTransformHandler(object sender, TransfromArgs e);
        public event SubmitTransformHandler SubmitTransform;

        #endregion

        #region Construction

        public UCEditTransform()
        {
            InitializeComponent();
            UniformSupport = false;
        }

        #endregion

        #region Event-Handler

        private void UniformCheck_Change(Object sender, EventArgs e)
        {
            textBox2.ReadOnly = checkBox1.Checked;
            textBox3.ReadOnly = checkBox1.Checked;

            if (checkBox1.Checked)
                UniformTextBox();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (_uniformSupport && checkBox1.Checked)
                UniformTextBox();
        }

        private void textBox_LostFocus(object sender, System.EventArgs e)
        {
            SubmitText();
        }

        #endregion

        #region Key-Receiver

        public void ReceiveKeyboard(string key)
        {
            if (key == null)
                return;

            if (!ReceiveKeyboard(textBox1, key))
            {
                if (!ReceiveKeyboard(textBox2, key))
                    ReceiveKeyboard(textBox3, key);

            }
        }

        private bool ReceiveKeyboard(System.Windows.Forms.TextBox textbox, string key)
        {
            if (!textbox.Focused || textbox.ReadOnly)
                return false;

            if (key == "Enter")
            {
                UnfocusTextBox(textbox);
                SubmitText();
                return true;
            }

            StringBuilder builder = new StringBuilder(textbox.Text);
            builder.Append(key);
            textbox.Text = builder.ToString();
            textbox.Select(textbox.Text.Length, 0);

            return true;
        }

        #endregion

        #region Helper Methods

        private void UniformTextBox()
        {
            textBox2.Text = textBox1.Text;
            textBox3.Text = textBox1.Text;
        }

        private void UnfocusTextBox(System.Windows.Forms.TextBox textbox)
        {
            switch (textbox.Name)
            {
                case "textBox1":
                    textBox2.Focus();
                    break;
                case "textBox2":
                    textBox3.Focus();
                    break;
                case "textBox3":
                    textBox1.Focus();
                    break;
            }
        }

        private void SubmitText()
        {
            if (SubmitTransform != null)
                SubmitTransform(this, new TransfromArgs(textBox1.Text, textBox2.Text, textBox3.Text));
        }

        #endregion

        #region Public Methods

        public void SetText(string text1, string text2, string text3)
        {
            textBox1.Text = text1;
            textBox2.Text = text2;
            textBox3.Text = text3;
        }
        
        #endregion
    }

    #region Event Args Class

    public class TransfromArgs : EventArgs
    {
        public Vector3 Transform;

        public TransfromArgs(string x, string y, string z)
        {
            float xFloat, yFloat, zFloat;

            if (!float.TryParse(x, out xFloat))
                xFloat = 0;
            if (!float.TryParse(y, out yFloat))
                yFloat = 0;
            if (!float.TryParse(z, out zFloat))
                zFloat = 0;

            Transform = new Vector3(xFloat, yFloat, zFloat);
        }
    }

    #endregion
}
