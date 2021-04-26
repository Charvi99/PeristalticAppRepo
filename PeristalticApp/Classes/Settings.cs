﻿using System;
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
        }
        public Settings(string name, int value, string unit, bool selecte, bool changed)
        {
            Name = name;
            Value = value;
            Unit = unit;
            Selected = selected;
            Changed = changed;
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
