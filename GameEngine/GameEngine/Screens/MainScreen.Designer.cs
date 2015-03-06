using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CommonLibrary;
using System.Runtime.InteropServices;

namespace GameEngine
{
    partial class MainScreen
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern uint MessageBox(IntPtr hWnd, String text, String caption, uint type);

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem buildToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem terrainToolStripMenuItem;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelRight;
        private System.Windows.Forms.Label label2;
        private UCEditTransform transform3;
        private UCEditTransform transform2;
        private UCEditTransform transform1;

        private System.Windows.Forms.Form form;

        private GameWindow Window;
        private int _widthWindow;
        private int _heightWindow;

        List<string> _imagesName = new List<string>();
        List<string> _buildingsBtnName = new List<string>();

        private void BuildForm()
        {
            form = System.Windows.Forms.Form.FromHandle(Window.Handle) as System.Windows.Forms.Form;
            if (form == null)
                return;

            form.FormClosing += FormClosing;

            _widthWindow = _mainGame.GraphicsDevice.Viewport.Width;
            _heightWindow = _mainGame.GraphicsDevice.Viewport.Height;

            InitializeImageName();

            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buildToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.terrainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flowLayoutPanelRight = new System.Windows.Forms.FlowLayoutPanel();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.transform3 = new UCEditTransform();
            this.transform2 = new UCEditTransform();
            this.transform1 = new UCEditTransform();
            this.menuStrip1.SuspendLayout();
            this.flowLayoutPanelRight.SuspendLayout();
            this.panelLeft.SuspendLayout();
            form.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.buildToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(284, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // buildToolStripMenuItem
            // 
            this.buildToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.terrainToolStripMenuItem});
            this.buildToolStripMenuItem.Name = "buildToolStripMenuItem";
            this.buildToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.buildToolStripMenuItem.Text = "Build";
            // 
            // terrainToolStripMenuItem
            // 
            this.terrainToolStripMenuItem.Name = "terrainToolStripMenuItem";
            this.terrainToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.terrainToolStripMenuItem.Text = "Terrain";
            this.terrainToolStripMenuItem.Click += new System.EventHandler(this.terrainToolStripMenuItem_Click);
            // 
            // flowLayoutPanelRight
            // 
            int y_flowLayoutPanelRight = 27;
            int width_flowLayoutPanelRight = 198;
            int x_flowLayoutPanelRight = _widthWindow - width_flowLayoutPanelRight;
            int height_flowLayoutPanelRight = _heightWindow - y_flowLayoutPanelRight;

            this.flowLayoutPanelRight.AutoScroll = true;
            this.flowLayoutPanelRight.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            int i = 0;
            foreach (string image in _imagesName)
            {
                this.flowLayoutPanelRight.Controls.Add(CreateUserControl(_buildingsBtnName[i++], image));
            }
            this.flowLayoutPanelRight.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelRight.Location = new System.Drawing.Point(
                x_flowLayoutPanelRight, y_flowLayoutPanelRight);
            this.flowLayoutPanelRight.Name = "flowLayoutPanelRight";
            this.flowLayoutPanelRight.Size = new System.Drawing.Size(
                width_flowLayoutPanelRight, height_flowLayoutPanelRight);
            this.flowLayoutPanelRight.TabIndex = 1;
            this.flowLayoutPanelRight.WrapContents = false;
            // 
            // flowLayoutPanelLeft
            // 
            int x_flowLayoutPanelLeft = 0;
            int y_flowLayoutPanelLeft = 27;
            int width_flowLayoutPanelLeft = 236;
            int height_flowLayoutPanelLeft = _heightWindow - y_flowLayoutPanelRight;

            this.panelLeft.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelLeft.Controls.Add(this.transform3);
            this.panelLeft.Controls.Add(this.transform2);
            this.panelLeft.Controls.Add(this.transform1);
            this.panelLeft.Controls.Add(this.label2);
            this.panelLeft.Location =
                new System.Drawing.Point(x_flowLayoutPanelLeft, y_flowLayoutPanelLeft);
            this.panelLeft.Name = "panel1";
            this.panelLeft.Size =
                new System.Drawing.Size(width_flowLayoutPanelLeft, height_flowLayoutPanelLeft);
            this.panelLeft.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, 
                System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(20, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 19);
            this.label2.TabIndex = 0;
            this.label2.Text = "Transform";
            // 
            // transform3
            // 
            this.transform3.Location = new System.Drawing.Point(10, 310);
            this.transform3.Name = "transform3";
            this.transform3.Size = new System.Drawing.Size(164, 128);
            this.transform3.TabIndex = 3;
            this.transform3.TransformName = "Scale";
            this.transform3.UniformSupport = true;
            this.transform3.SubmitTransform += SubmitTransform;
            // 
            // transform2
            // 
            this.transform2.Location = new System.Drawing.Point(10, 172);
            this.transform2.Name = "transform2";
            this.transform2.Size = new System.Drawing.Size(164, 128);
            this.transform2.TabIndex = 2;
            this.transform2.TransformName = "Rotation";
            this.transform2.SubmitTransform += SubmitTransform;
            // 
            // transform1
            // 
            this.transform1.Location = new System.Drawing.Point(10, 38);
            this.transform1.Name = "transform1";
            this.transform1.Size = new System.Drawing.Size(164, 128);
            this.transform1.TabIndex = 1;
            this.transform1.TransformName = "Position";
            this.transform1.SubmitTransform += SubmitTransform;
            // 
            // Form1
            // 
            form.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            form.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            form.Controls.Add(this.panelLeft);
            form.Controls.Add(this.flowLayoutPanelRight);
            form.Controls.Add(this.menuStrip1);
            form.Name = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.flowLayoutPanelRight.ResumeLayout(false);
            this.panelLeft.ResumeLayout(false);
            this.panelLeft.PerformLayout();
            form.ResumeLayout(false);
            form.PerformLayout();
        }

        private void InitializeImageName()
        {
            _imagesName.Add("Thumbnail_House.png");
            _imagesName.Add("Thumbnail_Tree.png");
            _imagesName.Add("Thumbnail_Bridge.png");
            _imagesName.Add("Thumbnail_Dude.png");

            _buildingsBtnName.Add("House_1");
            _buildingsBtnName.Add("Tree_1");
            _buildingsBtnName.Add("OldBridge");
            _buildingsBtnName.Add("Dude");
        }
        
        private UCBuildingButton CreateUserControl(string name, string image)
        {
            UCBuildingButton userCtr = new UCBuildingButton(image);

            userCtr.Location = new System.Drawing.Point(3, 3);
            userCtr.Name = name;
            userCtr.Size = new System.Drawing.Size(136, 159);
            userCtr.TabIndex = 0;
            userCtr.BuildClicked += BuildClicked;

            return userCtr;
        }
    }
}
