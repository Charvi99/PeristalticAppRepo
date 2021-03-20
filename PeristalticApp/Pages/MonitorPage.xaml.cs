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

namespace PeristalticApp
{
    /// <summary>
    /// Interaction logic for MonitorPage.xaml
    /// </summary>
    
    public partial class MonitorPage : Page
    {

        static public List<Classes.Settings> DataFromPump { get; set; }
        public MonitorPage()
        {
            InitializeComponent();
            
        }

        private void Settings_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
