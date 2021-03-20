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
using Newtonsoft.Json;


namespace PeristalticApp
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>

    public partial class SettingsPage : Page
    {
        List<Classes.Settings> settings = new List<Classes.Settings>();

        public SettingsPage()
        {
            InitializeComponent();
            settings.Add(new Classes.Settings("Mode", 10, "-", false));
            settings.Add(new Classes.Settings("Dose", 157, "ml", false));
            settings.Add(new Classes.Settings("Direction", 8, "-", false));
            settings.Add(new Classes.Settings("Speed", 50, "%", false));
            settings.Add(new Classes.Settings("Interval", 57, "s", false));
            settings.Add(new Classes.Settings("Ramp", 57, "s", false));
            SettingsEWlementOverview.ItemsSource = settings;
        }

        private void Export_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string export = JsonConvert.SerializeObject(settings,Formatting.Indented);
            Task.Run(async () => { await MQTTPage.MQTT_Publish("peristaltic/settings", export); });
           

        }

        private void SettingsElement_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string theSender = ((Border)sender).Tag.ToString();
            for (int i = 0; i < settings.Count; i++)
            {
                if (settings[i].Name == theSender)  settings[i].Selected = true;
                else settings[i].Selected = false; 
            }

        }
    }
}
