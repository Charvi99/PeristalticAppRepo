using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PeristalticApp;
using Newtonsoft.Json;
using FontAwesome.WPF;
using System.ComponentModel;
using LiveCharts;
using LiveCharts.Configurations;
using System.Threading;
//using System.Threading.Tasks;

namespace PeristalticApp
{
    /// <summary>
    /// Interaction logic for MonitorPage.xaml
    /// </summary>
    
    public partial class MonitorPage : Page, INotifyPropertyChanged
    {
        static public List<Classes.Settings> SettingsFromPump { get; set; }
        static public List<Classes.JSONData> DataFromPump { get; set; }
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        private double _axisMax;
        private double _axisMin;
        private double _trend;

        public int longMessageInterval { get; set; }
        public string warningMessage { get; set; }
        public int flowCounter { get; set; }




        public MonitorPage()
        {
            InitializeComponent();
            infoPanel.Text = "tramtada  \n daaaaaaa";
            Classes.Global.Instance.Running = false;
            //running = true;
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            dispatcherTimer.Start();

            longMessageInterval = 0;
            warningMessage = "";

            flowCounter = 0;

            DataFromPump = new List<Classes.JSONData>();

            Classes.JSONData zeroFirst = new Classes.JSONData();
            zeroFirst.Name = "Current";
            zeroFirst.Value = 0;
            zeroFirst.Unit = "mA";
            DataFromPump.Add(zeroFirst);

            var mapper = Mappers.Xy<Classes.GraphData>()
               .X(model => model.DateTime.Ticks)   //use DateTime.Ticks as X
               .Y(model => model.Value);           //use the value property as Y

            //lets save the mapper globally.
            Charting.For<Classes.GraphData>(mapper);

            //the values property will store our values array
            ChartValues = new ChartValues<Classes.GraphData>();

            //lets set how to display the X Labels
            DateTimeFormatter = value => new DateTime((long)value).ToString("mm:ss");

            //AxisStep forces the distance between each separator in the X axis
            AxisStep = TimeSpan.FromSeconds(1).Ticks;
            //AxisUnit forces lets the axis know that we are plotting seconds
            //this is not always necessary, but it can prevent wrong labeling
            AxisUnit = TimeSpan.TicksPerSecond;

            SetAxisLimits(DateTime.Now);

            //The next code simulates data changes every 300 ms

            IsReading = false;
            Task.Factory.StartNew(Read);

            DataContext = this;
        }

        public bool _youSpinMyHeadRightNowRightNowLikeArecordBabyRightNowRightNow { get; set; }

        public bool youSpinMyHeadRightNowRightNowLikeArecordBabyRightNowRightNow
        {
            get { return _youSpinMyHeadRightNowRightNowLikeArecordBabyRightNowRightNow; }
            set
            {
                _youSpinMyHeadRightNowRightNowLikeArecordBabyRightNowRightNow = value;
                OnPropertyChanged("youSpinMyHeadRightNowRightNowLikeArecordBabyRightNowRightNow");
            }
        }

        public ChartValues<Classes.GraphData> ChartValues { get; set; }
        public Func<double, string> DateTimeFormatter { get; set; }
        public double AxisStep { get; set; }
        public double AxisUnit { get; set; }
        public double AxisMax
        {
            get { return _axisMax; }
            set
            {
                _axisMax = value;
                OnPropertyChanged("AxisMax");
            }
        }
        public double AxisMin
        {
            get { return _axisMin; }
            set
            {
                _axisMin = value;
                OnPropertyChanged("AxisMin");
            }
        }
        public bool IsReading { get; set; }

        private void Read()
        {
            var r = new Random();

            while (IsReading)
            {
                Thread.Sleep(150);
                var now = DateTime.Now;

                _trend += r.Next(-8, 10);

                try
                {
                    ChartValues.Add(new Classes.GraphData
                    {
                        DateTime = now,
                        Value = DataFromPump[0].Value
                    });

                }
                    catch (Exception){throw;}

                SetAxisLimits(now);

                //lets only use the last 150 values
                if (ChartValues.Count > 150) ChartValues.RemoveAt(0);
            }
        }
        private void SetAxisLimits(DateTime now)
        {
            AxisMax = now.Ticks + TimeSpan.FromSeconds(1).Ticks; // lets force the axis to be 1 second ahead
            AxisMin = now.Ticks - TimeSpan.FromSeconds(10).Ticks; // and 8 seconds behind
        }



        private void Settings_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MainWindow.navigateToPage("SettingsPage");
        }

        private void RotationBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Classes.Global.Instance.Running == false)
            {
                if (RotationIcon.FlipOrientation == FlipOrientation.Normal)
                    RotationIcon.FlipOrientation = FlipOrientation.Horizontal;
                else if (RotationIcon.FlipOrientation == FlipOrientation.Horizontal)
                    RotationIcon.FlipOrientation = FlipOrientation.Normal;
                else
                    RotationIcon.FlipOrientation = FlipOrientation.Normal;

                if (SettingsFromPump[2].Value == 0) SettingsFromPump[2].Value = 1;
                else SettingsFromPump[2].Value = 0;
                string export = JsonConvert.SerializeObject(SettingsFromPump, Formatting.Indented);
                Task.Run(async () => { await MQTTPage.MQTT_Publish("peristaltic/settings", export); });
            }
            else
            {
                warningMessage = "Direction of pump \ncant be changed \nwhile pump is running";
            }

        }
        public void RotationIconEnable()
        {
            //RotationIcon.Spin = Classes.Global.Instance.Running;
            youSpinMyHeadRightNowRightNowLikeArecordBabyRightNowRightNow = Classes.Global.Instance.Running;
        }
        public void fromSettingsToInfopanelText()
        {
            if (SettingsFromPump != null)
            { 
                if(warningMessage == "")
                {
                    txtFlowValue.Text = flowCounter.ToString();
                    switch (SettingsFromPump[0].Value)
                    {
                        default:    //MANUAL
                            txtMode.Text = "MANUAL";
                            infoPanel.Text =
                                    SettingsFromPump[3].Name + ":\t" + SettingsFromPump[3].Value + " [" + SettingsFromPump[3].Unit + "]\n" +
                                    SettingsFromPump[5].Name + ":\t" + SettingsFromPump[5].Value + " [" + SettingsFromPump[5].Unit + "]";
                            // SettingsFromPump[9].Value + ":\r" + SettingsFromPump[9].Value + "\r" + SettingsFromPump[9].Unit + "\n";
                            break;
                        case 1:     //SEMIMANUAL
                            txtMode.Text = "SEMIMANUAL";
                            infoPanel.Text =
                                    SettingsFromPump[3].Name + ":\t" + SettingsFromPump[3].Value + " [" + SettingsFromPump[3].Unit + "]\n" +
                                    SettingsFromPump[5].Name + ":\t" + SettingsFromPump[5].Value + " [" + SettingsFromPump[5].Unit + "]";
                            //SettingsFromPump[9].Value + ":\r" + SettingsFromPump[9].Value + "\r" + SettingsFromPump[9].Unit + "\n";
                            break;
                        case 2:     //AUTOMAT
                            txtMode.Text = "AUTOMAT";
                            infoPanel.Text =
                                    SettingsFromPump[3].Name + ":\t" + SettingsFromPump[3].Value + " [" + SettingsFromPump[3].Unit + "]\n" +
                                    SettingsFromPump[1].Name + ":\t" + SettingsFromPump[1].Value + " [" + SettingsFromPump[1].Unit + "]\n" +
                                    SettingsFromPump[5].Name + ":\t" + SettingsFromPump[5].Value + " [" + SettingsFromPump[5].Unit + "]";
                            // SettingsFromPump[9].Value + ":\r" + SettingsFromPump[9].Value + "\r" + SettingsFromPump[9].Unit + "\n";
                            break;
                        case 3:     //INTERVAL
                            txtMode.Text = "INTERVAL";
                            infoPanel.Text =
                                    SettingsFromPump[3].Name + ":\t" + SettingsFromPump[3].Value + " [" + SettingsFromPump[3].Unit + "]\n" +
                                    SettingsFromPump[1].Name + ":\t" + SettingsFromPump[1].Value + " [" + SettingsFromPump[1].Unit + "]\n" +
                                    SettingsFromPump[4].Name + ":\t" + SettingsFromPump[4].Value + " [" + SettingsFromPump[4].Unit + "]\n" +
                                    SettingsFromPump[5].Name + ":\t" + SettingsFromPump[5].Value + " [" + SettingsFromPump[5].Unit + "]";
                            // SettingsFromPump[9].Value + ":\r" + SettingsFromPump[9].Value + "\r" + SettingsFromPump[9].Unit + "\n";
                            break;

                    }
                }
                else if(longMessageInterval < 6)
                {
                    longMessageInterval++;
                    infoPanel.Text = warningMessage;
                    var bc = new BrushConverter();
                    infoPanel.Background = (Brush)bc.ConvertFrom("#F8E7AE");
                    infoPanel.Foreground = (Brush)bc.ConvertFrom("#1f7396");
                    infoPanelBorder.Background = (Brush)bc.ConvertFrom("#F8E7AE");
                }
                else
                {
                    longMessageInterval = 0;
                    warningMessage = "";
                    var bc = new BrushConverter();
                    infoPanel.Background = (Brush)bc.ConvertFrom("#1f7396");
                    infoPanel.Foreground = (Brush)bc.ConvertFrom("#ffffff");
                    infoPanelBorder.Background = (Brush)bc.ConvertFrom("#1f7396");

                }
            }
        }
        public void graphUpdate()
        {
            IsReading = Classes.Global.Instance.Running;
            if (IsReading) Task.Factory.StartNew(Read);

        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            RotationIconEnable();
            fromSettingsToInfopanelText();
            if(Classes.Global.Instance.Running != IsReading)
                graphUpdate();
        }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private void ModeRight_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(SettingsFromPump != null)
            {
                if (Classes.Global.Instance.Running == false)
                {
                        SettingsFromPump[0].Value++;
                    if (SettingsFromPump[0].Value > 3) SettingsFromPump[0].Value = 0;
                    string export = JsonConvert.SerializeObject(SettingsFromPump, Formatting.Indented);
                    Task.Run(async () => { await MQTTPage.MQTT_Publish("peristaltic/settings", export); });
                }
                else
                {
                    warningMessage = "Mode of pump \ncant be changed \nwhile pump is running";
                }
            }
        }

        private void ModeLeft_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (SettingsFromPump != null)
            {
                if(Classes.Global.Instance.Running == false)
                {
                    SettingsFromPump[0].Value--;
                    if (SettingsFromPump[0].Value < 0) SettingsFromPump[0].Value = 3;
                    string export = JsonConvert.SerializeObject(SettingsFromPump, Formatting.Indented);
                    Task.Run(async () => { await MQTTPage.MQTT_Publish("peristaltic/settings", export); });
                }
                else
                {
                    warningMessage = "Mode of pump \ncant be changed \nwhile pump is running";
                }
            }

        }

        private void FlowBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Classes.Global.Instance.Running == false)
            {
                flowCounter = 0;
            }
            else
            {
                warningMessage = "Flow counter \ncant be nullify \nwhile pump is running";
            }
        }
    }
}
