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
using System.Text.RegularExpressions;
using System.Windows.Controls.Primitives;


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
            if(settings.Count == 0)
            {
                settings.Add(new Classes.Settings("Mode", 10, "-", false, false));
                settings.Add(new Classes.Settings("Dose", 157, "ml", false, false));
                settings.Add(new Classes.Settings("Direction", 8, "-", false, false));
                settings.Add(new Classes.Settings("Speed", 50, "%", false, false));
                settings.Add(new Classes.Settings("Interval", 57, "s", false, false));
                settings.Add(new Classes.Settings("Ramp", 57, "s", false, false));
                settings.Add(new Classes.Settings("Tube Lenght", 30, "mm", false, false));
                settings.Add(new Classes.Settings("Tube Diameter", 3, "mm", false, false));
            }

            SettingsEWlementOverview.ItemsSource = settings;

            MQTTPage.MQTT_Publish("peristaltic/data", "penis");

        }

        private void Export_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string export = JsonConvert.SerializeObject(settings,Formatting.Indented);
            Task.Run(async () => { await MQTTPage.MQTT_Publish("peristaltic/settings", export); });
            
            for (int i = 0; i < settings.Count; i++)
                settings[i].Changed = false;
            SettingsEWlementOverview.ItemsSource = null;
            SettingsEWlementOverview.ItemsSource = settings;

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

        /*
         * private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string theSender = ((TextBox)sender).Tag.ToString();

            for (int i = 0; i < settings.Count; i++)
            {
                if (settings[i].Name == theSender)
                {
                    settings[i].Changed = true;
                    break;
                }
            }
        }*/

        private void BackBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.navigateToPage("MonitorPage");

        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string theSender = ((TextBox)sender).Tag.ToString();

            if (theSender == "Mode")
            {
                Regex regex = new Regex("([^ASMIasmi]{1}$)");
                e.Handled = regex.IsMatch(e.Text);
            }
            /*
             else
            {
                if (theSenderValue.Length < 1)
                {
                    Regex regex = new Regex("([^0-1]{1,2})+$");
                    e.Handled = regex.IsMatch(e.Text);
                }
                else
                {
                    Regex regex = new Regex("([^0-1]{0})+$");
                    e.Handled = regex.IsMatch(e.Text);
                }

            }*/

        }

        private void DecreseBorderRepeat_Click(object sender, RoutedEventArgs e)
        {
            string theSender = ((RepeatButton)sender).Tag.ToString();
            for (int i = 0; i < settings.Count; i++)
            {
                if (settings[i].Name == theSender)
                {
                    settings[i].decreseValue();
                    settings[i].Selected = true;
                }
                else settings[i].Selected = false;
            }
        }

        private void IncreseBorderRepeat_Click(object sender, RoutedEventArgs e)
        {
            string theSender = ((RepeatButton)sender).Tag.ToString();
            for (int i = 0; i < settings.Count; i++)
            {
                if (settings[i].Name == theSender)
                {
                    settings[i].increseValue();
                    settings[i].Selected = true;
                }
                else settings[i].Selected = false;
            }

        }
    }
}
