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
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Media.Animation;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Implementations;
using MQTTnet.ManagedClient;
using MQTTnet.Protocol;
using System.ComponentModel;

namespace PeristalticApp
{

    public partial class MainWindow : Window
    {
        private bool navigationVisible = false;
        private MQTTPage newMQTT = new MQTTPage();
        public static bool connectFromMainWindow { get; set; }

        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();



        //Frame MainWindowFrame = new Frame();
        public MainWindow()
        {
            connectFromMainWindow = true;
            WindowStyle = WindowStyle.SingleBorderWindow;
            InitializeComponent();

            Classes.Global.Instance.MQTTConnectedIndicatorTxt = "ahoj";
            Classes.Global.Instance.PumpRunningIndicator = new SolidColorBrush(Colors.Red); 
            Classes.Global.Instance.PumpRunningIndicatorTxt = "Pump STOP ";
            

            navigateToPage("MQTTPage");

            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }
        static public void navigateToPage(string goTo)
        {
            foreach (Window window in System.Windows.Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    (window as MainWindow).MainWindowFrame.Navigate(new Uri(string.Format("{0}{1}{2}", "Pages/", goTo, ".xaml"), UriKind.RelativeOrAbsolute));
                }
            }
        }
        // HANDLER PRO VÝSUVNÉ MENU NAVIGACE
        private void NavigationIcon_MouseDown(object sender, EventArgs e)
        {
            navigationAnimation(!navigationVisible);
            navigationVisible = !navigationVisible;
        }
        private void navigationAnimation(bool show)
        {
            if (show == true)
            {
                Vector offset = VisualTreeHelper.GetOffset(NavigationPanel);
                var left = offset.X;
                TranslateTransform trans = new TranslateTransform();

                NavigationPanel.RenderTransform = trans;
                DoubleAnimation showAnim = new DoubleAnimation(0, -left, TimeSpan.FromSeconds(0.5));
                DoubleAnimation shadowAnim = new DoubleAnimation(1, 0.8, TimeSpan.FromSeconds(0.5), FillBehavior.Stop);
                trans.BeginAnimation(TranslateTransform.XProperty, showAnim);
                shadowAnim.Completed += (s, a) => MainWindowFrame.Opacity = 0.8;
                MainWindowFrame.BeginAnimation(UIElement.OpacityProperty, shadowAnim);
                MainWindowFrame.IsEnabled = false;

            }
            else
            {
                Vector offset = VisualTreeHelper.GetOffset(NavigationPanel);
                var left = offset.X;
                TranslateTransform trans = new TranslateTransform();
                NavigationPanel.RenderTransform = trans;
                DoubleAnimation hideAnim = new DoubleAnimation(-left, 0, TimeSpan.FromSeconds(0.5));
                DoubleAnimation shadowAnim = new DoubleAnimation(0.8, 1, TimeSpan.FromSeconds(0.5), FillBehavior.Stop);
                trans.BeginAnimation(TranslateTransform.XProperty, hideAnim);
                shadowAnim.Completed += (s, a) => MainWindowFrame.Opacity = 1;
                MainWindowFrame.BeginAnimation(UIElement.OpacityProperty, shadowAnim);
                MainWindowFrame.IsEnabled = true;

            }
        }
        // NAVIGACE DO MONITORPAGE
        private void MonitorBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            navigateToPage("MonitorPage");
        }

        // NAVIGACE DO SETTINGS
        private void SettingsBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            navigateToPage("SettingsPage");
        }

        // NAVIGACE DO MQTT
        private void MQTTBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            navigateToPage("MQTTPage");
        }


        private void Start_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MQTTPage.MQTT_Publish("peristaltic/run", "START");
            Classes.Global.Instance.Running = true;
        }

        private void Stop_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MQTTPage.MQTT_Publish("peristaltic/run", "STOP");
            Classes.Global.Instance.Running = false;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try { DragMove(); }
            catch { }
        }
        private void Minimalize_MouseDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void Maximaze_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (WindowState != WindowState.Maximized)
                WindowState = WindowState.Maximized;

            else
                WindowState = WindowState.Normal;

        }
        private void Exit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            /* MessageBoxResult result = MessageBoxResult.No;
             if (UI_App_WPF.Pages.PokusPage.Saved == false || UI_App_WPF.Pages.ProgramPage.Saved == false || UI_App_WPF.Pages.UploadPage.Saved == false)
             {
                 result = System.Windows.MessageBox.Show("Ukončit bez uložení?", "", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                 if (result == MessageBoxResult.Yes)
                     System.Windows.Application.Current.Shutdown();
             }
             else*/
            System.Windows.Application.Current.Shutdown();

        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            // code goes here
        }

        private void Git_ahref(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Charvi99/PeristalticAppRepo");
        }
    }
}
