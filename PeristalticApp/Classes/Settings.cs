using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;

namespace PeristalticApp.Classes
{
    public class Settings : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public int Value { get; set; }
        public string stringValue { get; set; }
        public string StringValue
        {
            get { return stringValue; }
            set
            {
                if (stringValue != value)
                {
                    stringValue = value;
                    OnPropertyChanged("StringValue");
                }
            }
        }
        public string Unit { get; set; }
        private bool selected { get; set; }
        public bool Selected
        {
            get { return selected; }
            set
            {
                if (selected != value)
                {
                    selected = value;
                    OnPropertyChanged("Selected");
                }
            }
        }
        private bool changed { get; set; }
        public bool Changed
        {
            get { return changed; }
            set
            {
                if (changed != value)
                {
                    changed = value;
                    OnPropertyChanged("Changed");
                }
            }
        }


        public Settings()
        {
            Name = "";
            Value = 0;
            Unit = "";
            Selected = false;
            StringValue = "";
        }
        public Settings(string name, int value, string unit, bool selecte, bool changed)
        {
            Name = name;
            Value = value;
            Unit = unit;
            Selected = selected;
            Changed = changed;
            intValtoStringVal();

        }

        public void increseValue()
        {
            Value++;
            Changed = true;
            intValtoStringVal();
        }
        public void decreseValue()
        {
            Value--;
            Changed = true;
            intValtoStringVal();
        }

        private void intValtoStringVal()
        {
            if (Name == "Mode")
            {
                if (Value == 0)
                    StringValue = "MANUAL";
                else if (Value == 1)
                    StringValue = "SEMIMANUAL";
                else if (Value == 2)
                    StringValue = "AUTOMAT";
                else if (Value == 3)
                    StringValue = "INTERVAL";
                else if (Value > 3)
                { Value = 0; intValtoStringVal(); }
                else if (Value < 0)
                { Value = 3; intValtoStringVal(); }
            }
            else if (Name == "Direction")
            {
                if (Value == 0)
                    StringValue = "CW";
                else if (Value == 1)
                    StringValue = "ACW";
                else if (Value > 1)
                { Value = 0; intValtoStringVal(); }
                else
                { Value = 1; intValtoStringVal(); }

            }
            else if (Name == "Speed")
            {
                if (Value > 10)
                    Value = 10;
                else if (Value < 1)
                    Value = 1;

                StringValue = (Value*10).ToString();
            }
            else
                StringValue = Value.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
