using Contract;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchRename
{
    public class Method : ICloneable
    {
        public PropertyChangedEventHandler PropertyChanged;
        public string Name { get; set; } = ""; //Name of rule
        public int Order { get; set; } = 0;
        public bool IsChecked { get; set; } = false; //true if method is checked
        public IRule rule { get; set; } = null; //Logic of rule

        public string ColorButton { get; set; } = "LightBlue"; //backgroud color of button
        public string Icon { get; set; } = "Plus"; 
        public string IconColor { get; set; } = "Green";

        public object Clone()
        {
            return MemberwiseClone();
        }
        public void changeStatus(bool newStatus)
        {
            IsChecked = newStatus;
            if (IsChecked)
            {
                ColorButton = "LightGrey";
                Icon = "Minus";
                IconColor = "Red";
            }
            else
            {
                ColorButton = "LightBlue";
                Icon = "Plus";
                IconColor = "Green";
            }
        }
    }
}
