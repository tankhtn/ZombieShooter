using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary
{
    public partial class UCBuildingButton : System.Windows.Forms.UserControl
    {
        string _image;

        public delegate void BuildClickHandler(object sender, System.EventArgs e);
        public event BuildClickHandler BuildClicked;

        public UCBuildingButton(string image)
        {
            _image = image;
            InitializeComponent();
        }

        private void build_Click(object sender, System.EventArgs e)
        {
            if (BuildClicked != null)
                BuildClicked(this, new EventArgs());
        }
    }
}
