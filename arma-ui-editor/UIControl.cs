using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NCalc;

namespace arma_ui_editor
{
    class UIControl
    {
        public Rectangle Rectangle;
        public Color Color;
        public string Name;
        public string Inherit;
        public Dictionary<string, string> Properties;
        public List<UIControl> ForegroundControls;
        public List<UIControl> BackgroundControls;
        public UIControl Parent;

        public bool Show;

        public int X
        {
            get { return this.Rectangle.X; }
            set { this.Rectangle.X = value; }
        }

        public int Y
        {
            get { return this.Rectangle.Y; }
            set { this.Rectangle.Y = value; }
        }

        public int Width
        {
            get { return this.Rectangle.Width; }
            set { this.Rectangle.Width = value; }
        }

        public int Height
        {
            get { return this.Rectangle.Height; }
            set { this.Rectangle.Height = value; }
        }

        public UIControl() { }

        public UIControl(string name, string parent)
        {
            this.Name = name;
            this.Inherit = parent;
            this.Rectangle = new Rectangle();
            this.Properties = new Dictionary<string,string>();
            this.ForegroundControls = new List<UIControl>();
            this.BackgroundControls = new List<UIControl>();
            this.Color = Color.DarkSlateGray;
            this.Show = true;
        }

        private int RealVirtuality2Pixels(string key, string value, Rectangle absolute, Rectangle safezone)
        {
            int pixels = 0;
            bool isSafezone = (value.ToLower().Contains("safezone")) ? true : false;
            bool hasParent = (this.Parent != null) ? true : false;
            double eval = Convert.ToDouble(new Expression((isSafezone) ? Regex.Match(value, @"\d+[.]*\d*").Value : value).Evaluate());

            switch (key)
            {
                case "x":
                    pixels = (int)(eval * ((isSafezone) ? safezone.Width : absolute.Width)) + ((hasParent) ? this.Parent.X : ((isSafezone) ? safezone.X : absolute.X));
                    break;
                case "y":
                    pixels = (int)(eval * ((isSafezone) ? safezone.Height : absolute.Height)) + ((hasParent) ? this.Parent.Y : ((isSafezone) ? safezone.Y : absolute.Y));
                    break;
                case "w":
                    pixels = (int)(eval * ((isSafezone) ? safezone.Width : absolute.Width));
                    break;
                case "h":
                    pixels = (int)(eval * ((isSafezone) ? safezone.Height : absolute.Height));
                    break;
            }

            return pixels;
        }

        public void AddProperty(string key, string value, Rectangle absolute, Rectangle safezone)
        {
            if (key.ToLower() == "x" || key.ToLower() == "y" || key.ToLower() == "w" || key.ToLower() == "h")
            {
                int pixels = RealVirtuality2Pixels(key, value, absolute, safezone);

                switch (key)
                {
                    case "x":
                        this.X = pixels;
                        value = pixels.ToString();
                        break;
                    case "y":
                        this.Y = pixels;
                        value = pixels.ToString();
                        break;
                    case "w":
                        this.Width = pixels;
                        value = pixels.ToString();
                        break;
                    case "h":
                        this.Height = pixels;
                        value = pixels.ToString();
                        break;
                }
            }

            this.Properties.Add(key, value);
        }

        public void AddForegroundControl(UIControl uiControl)
        {
            this.ForegroundControls.Add(uiControl);
        }

        public void AddBackgroundControl(UIControl uiControl)
        {
            this.BackgroundControls.Add(uiControl);
        }

        public void UpdatePosition(Rectangle rectangle)
        {
            this.Rectangle = rectangle;

            UpdateProperty("x", rectangle.X.ToString());
            UpdateProperty("y", rectangle.Y.ToString());
            UpdateProperty("w", rectangle.Width.ToString());
            UpdateProperty("h", rectangle.Height.ToString());
        }

        public void UpdateProperty(string key, string value)
        {
            if (this.Properties.ContainsKey(key))
            {
                this.Properties[key] = value;
            }
        }
    }
}
