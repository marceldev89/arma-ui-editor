using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arma_ui_editor
{
    class UIClass
    {
        public string Name;
        public Dictionary<string, string> Properties;
        public List<UIControl> ForegroundControls;
        public List<UIControl> BackgroundControls;

        public UIClass(string name)
        {
            this.Name = name;
            this.Properties = new Dictionary<string, string>();
            this.ForegroundControls = new List<UIControl>();
            this.BackgroundControls = new List<UIControl>();
        }

        public void AddProperty(string key, string value)
        {
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
    }
}
