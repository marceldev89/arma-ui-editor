using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using TinyPG;

namespace arma_ui_editor
{
    public partial class Form1 : Form
    {
        UIClass uiClass;
        UIControl selectedControl;
        Control focused = null;

        int gridSize;

        int x1;
        int y1;

        Rectangle absoluteRect;
        Rectangle safezoneRect;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            drawPanel.Width = 1440;
            drawPanel.Height = 900;
            this.Width = 1280;
            this.Height = 800;

            absoluteRect = new Rectangle(300, 135, 840, 630);
            safezoneRect = new Rectangle(0, 0, 1440, 900);

            int w = SystemInformation.PrimaryMonitorSize.Width;
            int h = SystemInformation.PrimaryMonitorSize.Height;

            this.Left = (w - this.Width) / 2;
            this.Top = (h - this.Height) / 2;

            selectedControl = null;
            splitContainer1.Panel1.AutoScrollPosition = new Point((drawPanel.Width - splitContainer1.Panel1.Width) / 2, (drawPanel.Height - splitContainer1.Panel1.Height) / 2);
            gridSize = 20;

            uiClass = new UIClass("New");
        }

        private void splitContainer_MouseDown(object sender, MouseEventArgs e)
        {
            // Get the focused control before the splitter is focused
            focused = getFocused(this.Controls);
        }

        private Control getFocused(Control.ControlCollection controls)
        {
            foreach (Control c in controls)
            {
                if (c.Focused)
                {
                    // Return the focused control
                    return c;
                }
                else if (c.ContainsFocus)
                {
                    // If the focus is contained inside a control's children
                    // return the child
                    return getFocused(c.Controls);
                }
            }
            // No control on the form has focus
            return null;
        }

        private void ReadConfigFile(string filename)
        {
            Scanner scanner = new Scanner();
            Parser parser = new Parser(scanner);
            string input = System.IO.File.ReadAllText(filename);
            ParseTree tree = parser.Parse(input);

            ParseUIConfig(tree);

            drawPanel.Invalidate();

            treeView1.Nodes.Add(uiClass.Name);

            if (uiClass.ForegroundControls.Count > 0)
            {
                treeView1.Nodes[0].Nodes.Add("Controls");

                foreach (UIControl c in uiClass.ForegroundControls)
                {
                    tNode = treeView1.Nodes[0].Nodes[0].Nodes.Add(c.Name);
                    if (c.ForegroundControls.Count > 0)
                    {
                        foreach (UIControl c2 in c.ForegroundControls)
                        {
                            tNode.Nodes.Add(c2.Name);
                        }
                    }
                }
            }

            if (uiClass.BackgroundControls.Count > 0)
            {
                treeView1.Nodes[0].Nodes.Add("ControlsBackground");

                foreach (UIControl c in uiClass.BackgroundControls)
                {
                    tNode = treeView1.Nodes[0].Nodes[1].Nodes.Add(c.Name);

                    if (c.BackgroundControls.Count > 0)
                    {
                        foreach (UIControl c2 in c.BackgroundControls)
                        {
                            tNode.Nodes.Add(c2.Name);
                        }
                    }
                }
            }

            CheckAll(treeView1.Nodes);

            treeView1.ExpandAll();
        }

        private void CheckAll(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                node.Checked = true;
                CheckAllChildren(node);
            }
        }

        private void CheckAllChildren(TreeNode rootNode)
        {
            foreach (TreeNode node in rootNode.Nodes)
            {
                node.Checked = true;
                CheckAllChildren(node);
            }
        }

        private void SelectNode(TreeNode rootNode, UIControl control)
        {
            foreach (TreeNode node in rootNode.Nodes)
            {
                if (node.Text == control.Name)
                {
                    treeView1.SelectedNode = node;
                }
                else
                {
                    SelectNode(node, control);
                }
            }
        }

        TreeNode tNode = null;
        UIControl lastControl = null;

        private void ParseUIConfig(ParseNode node)
        {
            foreach (ParseNode n in node.Nodes)
            {
                string classPattern = @"class\s+(?<class>[a-zA-Z_]+[a-zA-Z0-9_]*)\s*:*\s*(?<parent>[a-zA-Z_]+[a-zA-Z0-9_]*)*.*?$";
                string properyPattern = @"((?:\w|[\[\]])+)\s*=\s*(.*);";
                Match match;

                // Class
                if (n.Token.Type == TokenType.CLASS && n.Parent.Token.Type == TokenType.Class)
                {
                    Match m = Regex.Match(n.Token.Text, classPattern);

                    string className = m.Groups["class"].Value;
                    string classParent = m.Groups["parent"].Value;

                    uiClass = new UIClass(className);
                }

                // Class -> PROPERTY
                if (n.Token.Type == TokenType.PROPERTY && n.Parent.Token.Type == TokenType.Class)
                {
                    match = Regex.Match(n.Token.Text, properyPattern);
                    uiClass.AddProperty(match.Groups[1].Value, match.Groups[2].Value);
                }

                // Class -> fControls -> Control
                if (n.Token.Type == TokenType.CLASS && n.Parent.Token.Type == TokenType.Control && n.Parent.Parent.Token.Type == TokenType.fControls && n.Parent.Parent.Parent.Token.Type == TokenType.Class)
                {
                    Match m = Regex.Match(n.Token.Text, classPattern);

                    string className = m.Groups["class"].Value;
                    string classParent = m.Groups["parent"].Value;

                    UIControl c = new UIControl(className, classParent);
                    lastControl = c;
                    uiClass.AddForegroundControl(c);
                }

                // Class -> bControls -> Control
                if (n.Token.Type == TokenType.CLASS && n.Parent.Token.Type == TokenType.Control && n.Parent.Parent.Token.Type == TokenType.bControls && n.Parent.Parent.Parent.Token.Type == TokenType.Class)
                {
                    Match m = Regex.Match(n.Token.Text, classPattern);

                    string className = m.Groups["class"].Value;
                    string classParent = m.Groups["parent"].Value;

                    UIControl c = new UIControl(className, classParent);
                    lastControl = c;
                    uiClass.AddBackgroundControl(c);
                }

                // Class -> fControls -> Control -> PROPERTY
                if (n.Token.Type == TokenType.PROPERTY && n.Parent.Token.Type == TokenType.Control && n.Parent.Parent.Token.Type == TokenType.fControls && n.Parent.Parent.Parent.Token.Type == TokenType.Class)
                {
                    match = Regex.Match(n.Token.Text, properyPattern);
                    lastControl.AddProperty(match.Groups[1].Value, match.Groups[2].Value, absoluteRect, safezoneRect);
                }

                // Class -> bControls -> Control -> PROPERTY
                if (n.Token.Type == TokenType.PROPERTY && n.Parent.Token.Type == TokenType.Control && n.Parent.Parent.Token.Type == TokenType.bControls && n.Parent.Parent.Parent.Token.Type == TokenType.Class)
                {
                    match = Regex.Match(n.Token.Text, properyPattern);
                    lastControl.AddProperty(match.Groups[1].Value, match.Groups[2].Value, absoluteRect, safezoneRect);
                }

                // Class -> fControls -> Control -> fControls -> Control
                if (n.Token.Type == TokenType.CLASS && n.Parent.Token.Type == TokenType.Control && n.Parent.Parent.Token.Type == TokenType.fControls && n.Parent.Parent.Parent.Token.Type != TokenType.Class)
                {
                    Match m = Regex.Match(n.Token.Text, classPattern);

                    string className = m.Groups["class"].Value;
                    string classParent = m.Groups["parent"].Value;

                    UIControl c = new UIControl(className, classParent);
                    c.Parent = lastControl;
                    lastControl.AddForegroundControl(c);
                }

                // Class -> bControls -> Control -> bControls -> Control
                if (n.Token.Type == TokenType.CLASS && n.Parent.Token.Type == TokenType.Control && n.Parent.Parent.Token.Type == TokenType.bControls && n.Parent.Parent.Parent.Token.Type != TokenType.Class)
                {
                    Match m = Regex.Match(n.Token.Text, classPattern);

                    string className = m.Groups["class"].Value;
                    string classParent = m.Groups["parent"].Value;

                    UIControl c = new UIControl(className, classParent);
                    c.Parent = lastControl;
                    lastControl.AddBackgroundControl(c);
                }

                // Class -> bControls -> Control -> bControls -> Control -> PROPERTY
                if (n.Token.Type == TokenType.PROPERTY && n.Parent.Token.Type == TokenType.Control && n.Parent.Parent.Token.Type == TokenType.fControls && n.Parent.Parent.Parent.Token.Type != TokenType.Class)
                {
                    match = Regex.Match(n.Token.Text, properyPattern);
                    lastControl.ForegroundControls[lastControl.ForegroundControls.Count - 1].AddProperty(match.Groups[1].Value, match.Groups[2].Value, absoluteRect, safezoneRect);
                }

                // Class -> bControls -> Control -> bControls -> Control -> PROPERTY
                if (n.Token.Type == TokenType.PROPERTY && n.Parent.Token.Type == TokenType.Control && n.Parent.Parent.Token.Type == TokenType.bControls && n.Parent.Parent.Parent.Token.Type != TokenType.Class)
                {
                    match = Regex.Match(n.Token.Text, properyPattern);
                    lastControl.BackgroundControls[lastControl.BackgroundControls.Count - 1].AddProperty(match.Groups[1].Value, match.Groups[2].Value, absoluteRect, safezoneRect);
                }

                ParseUIConfig(n);
            }
        }

        private void drawControls(List<UIControl> controls, Graphics g)
        {
            foreach (UIControl control in controls)
            {
                SolidBrush brush;

                if (control.Show)
                {
                    brush = new System.Drawing.SolidBrush(control.Color);

                    if (control != selectedControl)
                    {
                        g.FillRectangle(brush, control.Rectangle);
                    }
                    else
                    {
                        g.FillRectangle(new SolidBrush(Color.LightSlateGray), control.Rectangle);
                    }

                    g.DrawRectangle(new Pen(Color.Black, 2), new Rectangle(control.X, control.Y, control.Width, control.Height));

                    if (control.Name != null)
                    {
                        //g.DrawString(control.Name, new Font("Arial", 4), new SolidBrush(Color.Black), control.X + 2, control.Y + 2);
                    }

                    brush.Dispose();

                    if (control.ForegroundControls.Count > 0)
                    {
                        drawControls(control.ForegroundControls, g);
                    }

                    if (control.BackgroundControls.Count > 0)
                    {
                        drawControls(control.BackgroundControls, g);
                    }
                }
            }
        }

        private void drawGrid(Graphics g, int gridSize)
        {
            if (gridButton.Checked)
            {
                for (int i = 0; i <= drawPanel.Width / gridSize; i++)
                {
                    g.DrawLine(new Pen(Color.FromArgb(65, Color.Black)), i * gridSize, 0, i * gridSize, drawPanel.Height);
                }

                for (int i = 0; i <= drawPanel.Height / gridSize; i++)
                {
                    g.DrawLine(new Pen(Color.FromArgb(65, Color.Black)), 0, i * gridSize, drawPanel.Width, i * gridSize);
                }

                g.DrawLine(new Pen(Color.FromArgb(130, Color.DarkRed), 1), drawPanel.Width / 2, 0, drawPanel.Width / 2, drawPanel.Height);
                g.DrawLine(new Pen(Color.FromArgb(130, Color.DarkRed), 1), 0, drawPanel.Height / 2, drawPanel.Width, drawPanel.Height / 2);
                g.DrawRectangle(new Pen(Color.FromArgb(130, Color.DarkRed), 1), 300, 135, absoluteRect.Width, absoluteRect.Height);
            }
        }

        private void FindControl(UIControl control, Point point)
        {
            if (control.Rectangle.Contains(point))
            {
                if (control.Show)
                {
                    selectedControl = control;
                }
            }

            foreach (UIControl child in control.BackgroundControls)
            {
                if (child.Rectangle.Contains(point))
                {
                    if (child.Show)
                    {
                        selectedControl = child;
                    }
                }

                FindControl(child, point);
            }

            foreach (UIControl child in control.ForegroundControls)
            {
                if (child.Rectangle.Contains(point))
                {
                    if (child.Show)
                    {
                        selectedControl = child;
                    }
                }

                FindControl(child, point);
            }
        }

        private void SelectControl(UIControl uiControl)
        {
            selectedControl = uiControl;

            //foreach (TreeNode node in treeView1.Nodes[0].Nodes)
            //{
            //    if (node.Text == uiControl.Name)
            //    {
            //        treeView1.SelectedNode = node;
            //    }
            //}

            foreach (TreeNode node in treeView1.Nodes)
            {
                SelectNode(node, uiControl);
            }

            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Add("Properties", "Properties");
            dataGridView1.Columns.Add("Values", "");

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            dataGridView1.Rows.Add("Parent class", uiControl.Inherit);
            dataGridView1.Rows.Add(1);
            dataGridView1.Rows[1].ReadOnly = true;

            foreach (KeyValuePair<string,string> kvp in uiControl.Properties)
            {
                dataGridView1.Rows.Add(kvp.Key, kvp.Value);
            }

            drawPanel.Invalidate();
        }

        private void DeselectControl(UIControl uiControl)
        {
            if (uiControl == null)
            {
                selectedControl = null;
                //treeView1.SelectedNode = null;
                dataGridView1.Columns.Clear();
                dataGridView1.Rows.Clear();
            }
            else
            {
                selectedControl = null;
                treeView1.SelectedNode = null;
                dataGridView1.Columns.Clear();
                dataGridView1.Rows.Clear();
            }            

            drawPanel.Invalidate();
        }

        private void UpdateControl(UIControl uiControl, Rectangle rectangle)
        {
            uiControl.Rectangle = rectangle;            
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            StringBuilder output = new StringBuilder();

            output.AppendFormat("{0} {{\r\n", uiClass.Name);

            if (uiClass.Properties.Count > 0)
            {
                foreach (KeyValuePair<string, string> kvp in uiClass.Properties)
                {
                    output.AppendFormat("\t{0} = {1};\r\n", kvp.Key, kvp.Value);
                }
            }

            if (uiClass.ForegroundControls.Count > 0)
            {
                output.AppendFormat("\tclass Controls {{\r\n");

                foreach (UIControl c in uiClass.ForegroundControls)
                {
                    output.AppendFormat("\t\t{0} {{\r\n", c.Name);

                    foreach (KeyValuePair<string, string> kvp in c.Properties)
                    {
                        output.AppendFormat("\t\t\t{0} = {1};\r\n", kvp.Key, kvp.Value);
                    }

                    if (c.ForegroundControls.Count > 0)
                    {
                        output.AppendFormat("\t\t\tclass Controls {{\r\n");

                        foreach (UIControl c2 in c.ForegroundControls)
                        {
                            output.AppendFormat("\t\t\t\t{0} {{\r\n", c2.Name);

                            foreach (KeyValuePair<string, string> kvp in c2.Properties)
                            {
                                output.AppendFormat("\t\t\t\t\t{0} = {1};\r\n", kvp.Key, kvp.Value);
                            }

                            output.AppendFormat("\t\t\t\t}};\r\n");
                        }

                        output.AppendFormat("\t\t\t}};\r\n");
                    }

                    output.AppendFormat("\t\t}};\r\n");
                }

                output.AppendFormat("\t}};\r\n");
            }

            if (uiClass.BackgroundControls.Count > 0)
            {
                foreach (UIControl c in uiClass.BackgroundControls)
                {
                    if (c.BackgroundControls.Count > 0)
                    {
                        foreach (UIControl c2 in c.BackgroundControls)
                        {

                        }
                    }
                }
            }

            output.AppendFormat("}};\r\n");
            Clipboard.SetText(output.ToString());
        }

        private void drawPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics formGraphics;
            formGraphics = drawPanel.CreateGraphics();

            drawControls(uiClass.BackgroundControls, formGraphics);
            drawControls(uiClass.ForegroundControls, formGraphics);
            drawGrid(formGraphics, gridSize);

            formGraphics.Dispose();
            treeView1.Focus(); // remove focus from splitter
        }

        private void drawPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (moveButton.Checked)
                {
                    if (uiClass != null)
                    {
                        foreach (UIControl control in uiClass.ForegroundControls)
                        {
                            Rectangle controlRect = control.Rectangle;

                            if (controlRect.Contains(e.Location))
                            {
                                if (selectedControl != control)
                                {
                                    SelectControl(control);
                                }
                            }
                            else
                            {
                                if (selectedControl == control)
                                {
                                    DeselectControl(control);
                                }
                            }
                        }

                        x1 = e.Location.X;
                        y1 = e.Location.Y;
                    }
                }
            }
        }

        private void drawPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (selectButton.Checked)
                {
                    if (uiClass != null)
                    {
                        foreach (UIControl control in uiClass.BackgroundControls)
                        {
                            FindControl(control, e.Location);
                        }

                        foreach (UIControl control in uiClass.ForegroundControls)
                        {
                            FindControl(control, e.Location);
                        }

                        if (selectedControl != null)
                        {
                            SelectControl(selectedControl);
                        }
                    }
                }
            }
        }

        private void drawPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (moveButton.Checked)
                {
                    Point pos = SnapToGrid(e.Location.X, e.Location.Y);

                    int tmpX = pos.X;
                    int tmpY = pos.Y;

                    pos = SnapToGrid(selectedControl.X + e.Location.X - x1, selectedControl.Y + e.Location.Y - y1);

                    selectedControl.UpdatePosition(new Rectangle(pos.X, pos.Y, selectedControl.Width, selectedControl.Height));

                    x1 = tmpX;
                    y1 = tmpY;

                    drawPanel.Invalidate();
                }
            }
        }

        private Point SnapToGrid(int x, int y)
        {
            int xNew = x;
            int yNew = y;

            if (snapButton.Checked)
            {
                if (x % gridSize < gridSize / 4)
                {
                    xNew = x - x % gridSize;
                }
                else if (x % gridSize > gridSize - (gridSize / 4))
                {
                    xNew = x + (gridSize - x % gridSize);
                }

                if (y % gridSize < gridSize / 4)
                {
                    yNew = y - y % gridSize;
                }
                else if (y % gridSize > gridSize - (gridSize / 4))
                {
                    yNew = y + (gridSize - y % gridSize);
                }
            }

            return new Point(xNew, yNew);
        }

        private void containerPanel_Scroll(object sender, ScrollEventArgs e)
        {
            drawPanel.Invalidate();
        }

        private void gridButton_CheckedChanged(object sender, EventArgs e)
        {
            drawPanel.Invalidate();
        }

        private void splitContainer_MouseUp(object sender, MouseEventArgs e)
        {
            // If a previous control had focus
            if (focused != null)
            {
                // Return focus and clear the temp variable for 
                // garbage collection
                focused.Focus();
                focused = null;
            }
        }

        private void splitContainer1_Paint(object sender, PaintEventArgs e)
        {
            focused = getFocused(this.Controls);
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            drawPanel.Invalidate();
        }

        private void drawButton_CheckedChanged(object sender, EventArgs e)
        {
            if (drawButton.Checked)
            {
                selectButton.Checked = false;
                moveButton.Checked = false;
            }
        }

        private void selectButton_CheckedChanged(object sender, EventArgs e)
        {
            if (selectButton.Checked)
            {
                drawButton.Checked = false;
                moveButton.Checked = false;
            }
        }

        private void moveButton_CheckedChanged(object sender, EventArgs e)
        {
            if (moveButton.Checked)
            {
                selectButton.Checked = false;
                drawButton.Checked = false;
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (uiClass != null)
            {
                if (uiClass.Name == e.Node.Text)
                {
                    DeselectControl(null);
                }
                else
                {
                    foreach (UIControl c in uiClass.ForegroundControls)
                    {
                        if (c.Name == e.Node.Text)
                        {
                            SelectControl(c);
                        }

                        if (c.ForegroundControls.Count > 0)
                        {
                            foreach (UIControl c2 in c.ForegroundControls)
                            {
                                if (c2.Name == e.Node.Text)
                                {
                                    SelectControl(c2);
                                }
                            }
                        }                        
                    }

                    foreach (UIControl c in uiClass.BackgroundControls)
                    {
                        if (c.Name == e.Node.Text)
                        {
                            SelectControl(c);
                        }

                        if (c.BackgroundControls.Count > 0)
                        {
                            foreach (UIControl c2 in c.BackgroundControls)
                            {
                                if (c2.Name == e.Node.Text)
                                {
                                    SelectControl(c2);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void configToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Config files (*.cpp)|*.cpp|All files (*.*)|*.*";
            openFileDialog1.ShowDialog();
            openFileDialog1.Filter = "";
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            ReadConfigFile(openFileDialog1.FileName);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            int w = splitContainer1.Panel1.Width;
            int h = splitContainer1.Panel1.Height;

            if (drawPanel.Width < w)
            {
                drawPanel.Left = (w - drawPanel.Width) / 2;
            }
            else
            {
                drawPanel.Left = 0;
            }

            if (drawPanel.Height < h)
            {
                drawPanel.Top = (h - drawPanel.Height) / 2;
            }
            else
            {
                drawPanel.Top = 0;
            }
        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (uiClass != null)
            {
                if (uiClass.Name == e.Node.Text)
                {
                    //DeselectControl(null);
                }
                else
                {
                    foreach (UIControl c in uiClass.ForegroundControls)
                    {
                        if (c.Name == e.Node.Text)
                        {
                            //SelectControl(c);
                            if (e.Node.Checked)
                            {
                                c.Show = true;
                            }
                            else
                            {
                                c.Show = false;
                            }

                            drawPanel.Invalidate();
                        }
                        else
                        {
                            if (c.ForegroundControls.Count > 0)
                            {
                                foreach (UIControl c2 in c.ForegroundControls)
                                {
                                    if (c2.Name == e.Node.Text)
                                    {
                                        if (e.Node.Checked)
                                        {
                                            c2.Show = true;
                                        }
                                        else
                                        {
                                            c2.Show = false;
                                        }

                                        drawPanel.Invalidate();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
