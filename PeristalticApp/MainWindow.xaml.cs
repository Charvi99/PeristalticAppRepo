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

namespace PeristalticApp
{
    public static class Global
    {
        public static IMqttClient mqttClient;
        public static MQTTnet.Client.IMqttClientOptions options = null;
    }

    public partial class MainWindow : Window
    {
        private bool navigationVisible = false;
        private MQTTPage newMQTT = new MQTTPage();
        //Frame MainWindowFrame = new Frame();
        public MainWindow()
        {
            InitializeComponent();
            //subMainGrid.Children.Add(MainWindowFrame);
            //MainWindowFrame.SetValue(Grid.RowProperty, 1);

            //Dispatcher.BeginInvoke(new Frame());
            //Task.Run(async () => { await newMQTT.ConnectMqttServerAsync(); await newMQTT.MQTT_Subscribe(); await MQTTPage.MQTT_Publish("peristaltic/data", "penis"); });
            //MQTTPage.ConnectMqttServerAsync();
            //MQTTPage.ConnectMqttServerAsync();
            //newMQTT.ConnectMqttServerAsync();

            //MQTTPage.MQTT_Subscribe();
            //MQTTPage.MQTT_Publish("peristaltic/data", "OH HELLO THERE");
            navigateToPage("SettingsPage");



        }
        private void navigateToPage(string goTo)
        {
            foreach (Window window in System.Windows.Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    (window as MainWindow).MainWindowFrame.Navigate(new Uri(string.Format("{0}{1}{2}", "Pages/", goTo, ".xaml"), UriKind.RelativeOrAbsolute));
                }
            }
        }

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

        private void NavigationIcon_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void NavigationIcon_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void MonitorBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            navigateToPage("MonitorPage");
        }

        private void SettingsBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            navigateToPage("SettingsPage");
        }

        private void MQTTBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            navigateToPage("MQTTPage");
        }
    }
}
