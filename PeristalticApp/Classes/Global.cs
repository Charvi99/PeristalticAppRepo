using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Implementations;
using MQTTnet.ManagedClient;
using MQTTnet.Protocol;
using System.ComponentModel;
using System.Windows.Media;

namespace PeristalticApp.Classes
{
    public sealed class Global : INotifyPropertyChanged
    {
        public static IMqttClient mqttClient;
        public static MQTTnet.Client.IMqttClientOptions options = null;
        private static readonly Global instance = new Global();
        //public Classes.GraphData 
        private Global() { }
        public static Global Instance
        {
            get
            {
                return instance;
            }
        }
        private bool running { get; set; }
        public bool Running
        {
            get { return running; }

            set
            {
                if (value != running)
                {
                    running = value;
                    NotifyPropertyChanged("Running");
                }
            }
        }

        private string infoPanelText { get; set; }
        public string InfoPanelText
        {
            get { return infoPanelText; }
            set
            {
                if (infoPanelText != value)
                {
                    infoPanelText = value;
                    NotifyPropertyChanged("InfoPanelText");
                }
            }
        }


        private string _MQTTConnectedIndicator;
        public string MQTTConnected
        {
            get { return this._MQTTConnectedIndicator; }

            set
            {
                if (value != this._MQTTConnectedIndicator)
                {
                    this._MQTTConnectedIndicator = value;
                    NotifyPropertyChanged("MyProp");
                }
            }
        }
        private string _MQTTConnectedIndicatorTxt;
        public string MQTTConnectedIndicatorTxt
        {
            get { return this._MQTTConnectedIndicatorTxt; }

            set
            {
                if (value != this._MQTTConnectedIndicatorTxt)
                {
                    this._MQTTConnectedIndicatorTxt = value;
                    NotifyPropertyChanged("MQTTConnectedIndicatorTxt");
                }
            }
        }
        private Brush _MQTTConnectedIndicatorIndicator;
        public Brush MQTTConnectedIndicatorIndicator
        {
            get { return this._MQTTConnectedIndicatorIndicator; }

            set
            {
                if (value != this._MQTTConnectedIndicatorIndicator)
                {
                    this._MQTTConnectedIndicatorIndicator = value;
                    NotifyPropertyChanged("MQTTConnectedIndicatorIndicator");
                }
            }
        }

        private string _PumpRunningIndicatorTxt;
        public string PumpRunningIndicatorTxt
        {
            get { return this._PumpRunningIndicatorTxt; }

            set
            {
                if (value != this._PumpRunningIndicatorTxt)
                {
                    this._PumpRunningIndicatorTxt = value;
                    NotifyPropertyChanged("PumpRunningIndicatorTxt");
                }
            }
        }
        private Brush _PumpRunningIndicator;
        public Brush PumpRunningIndicator
        {
            get { return this._PumpRunningIndicator; }

            set
            {
                if (value != this._PumpRunningIndicator)
                {
                    this._PumpRunningIndicator = value;
                    NotifyPropertyChanged("PumpRunningIndicator");
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String info)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }

    
    }

}
