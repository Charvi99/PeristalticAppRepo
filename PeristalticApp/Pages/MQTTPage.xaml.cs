using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Implementations;
using MQTTnet.ManagedClient;
using MQTTnet.Protocol;
using Newtonsoft.Json;

namespace PeristalticApp
{
    /// <summary>
    /// Interaction logic for MQTTPage.xaml
    /// </summary>

    public partial class MQTTPage : Page
    {
        
        private static bool isReconnect = true;
        
        public string topic = "";

        public static List<Classes.Topic> Topics { get; set; }
        static MQTTPage staticMqtt;

        public MQTTPage()
        {
            InitializeComponent();
            //ElementHost.EnableModelessKeyboardInterop(MQTTPage);
             Global.mqttClient = null;
            staticMqtt = this;

            Topics = new List<Classes.Topic>();
            Topics.Add(new Classes.Topic("peristaltic/data"));
            Topics.Add(new Classes.Topic("peristaltic/settings"));

            TopicOverview.ItemsSource = Topics;
            TopicComboBox.ItemsSource = null;
            TopicComboBox.ItemsSource = Topics;
            
        }

        private async void BtnPublish_Click(object sender, RoutedEventArgs e) => await MQTT_Publish(Topics[TopicComboBox.SelectedIndex].Name,txtSendMessage.Text);

        private async void Subscribe_Click(object sender, RoutedEventArgs e) => await MQTT_Subscribe();

        static public async Task MQTT_Publish(string topic1, string content)
        {
            if (string.IsNullOrEmpty(topic1))
            {
                MessageBox.Show("Topic is empty");
                return;
            }

            ///qos=0，-> jednou se odešle
            ///QoS 1: -> určite přijde alespon jednou
            ///QoS 2: -> nejvyssi uroven, zprava prijde určite pouze jednou

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic1)
                .WithPayload(content)
                .WithAtMostOnceQoS()
                .WithRetainFlag(true)
                .Build();

            await Global.mqttClient.PublishAsync(message);
        }

        public static async Task MQTT_Subscribe()
        {
            for (int i = 0; i < Topics.Count; i++)
            {
                if (string.IsNullOrEmpty(Topics[i].Name))
                {
                    MessageBox.Show("Subscribe is Empty！");
                    return;
                }

                if (!Global.mqttClient.IsConnected)
                {
                    //MessageBox.Show("MQTT Client isn't connected！");
                    return;
                }

                // Subscribe to a topic
                await Global.mqttClient.SubscribeAsync(new TopicFilterBuilder()
                    .WithTopic(Topics[i].Name)
                    .WithAtMostOnceQoS()
                    .Build()
                    );


                staticMqtt.Dispatcher.Invoke((new Action(() =>
                {
                    staticMqtt.txtReceiveMessage.AppendText($"Client is subsribeing: [{Topics[i].Name}]topic{Environment.NewLine}");
                })));
            }
        }

        public static async Task ConnectMqttServerAsync()
        {
            // Create a new MQTT client.

            if (Global.mqttClient == null)
            {
                var factory = new MqttFactory();
                Global.mqttClient = factory.CreateMqttClient();

                Global.mqttClient.ApplicationMessageReceived += MqttClient_ApplicationMessageReceived;
                Global.mqttClient.Connected += MqttClient_Connected;
                Global.mqttClient.Disconnected += MqttClient_Disconnected;
            }

            try
            {
                CreteClient();
                await Global.mqttClient.ConnectAsync(Global.options);

            }
            catch (Exception ex)
            {
                staticMqtt.Dispatcher.Invoke((new Action(() =>
                {
                    staticMqtt.txtReceiveMessage.AppendText("Failed to connect to MQTT server！" + Environment.NewLine + ex.Message + Environment.NewLine);
                    //MessageBox.Show("Failed to connect to MQTT server");

                })));
            }

        }
        private static void CreteClient()
        {

            //getting inside actual thread
            if (!staticMqtt.Dispatcher.CheckAccess())
            {
                // We're not in the UI thread, ask the dispatcher to call this same method in the UI thread, then exit
                staticMqtt.Dispatcher.BeginInvoke(new Action(CreteClient));
                return;
            }
            //Create TCP based options using the builder.
            var id = staticMqtt.txtClientId.Text;
            string ip = staticMqtt.txtIp.Text.ToString();
            int port = Convert.ToInt32(staticMqtt.txtPort.Text.ToString());
            string user = staticMqtt.txtUsername.Text.ToString();
            string pass = staticMqtt.txtPsw.Text.ToString();
            Global.options = new MqttClientOptionsBuilder()
                .WithClientId(id)
                .WithTcpServer(ip, port)
                .WithCredentials(user, pass)
                .WithCleanSession()
                .Build();
            return;
        }

        private static void MqttClient_Connected(object sender, EventArgs e)
        {
            staticMqtt.Dispatcher.Invoke((new Action(() =>
            {
                staticMqtt.txtReceiveMessage.Clear();
                staticMqtt.txtReceiveMessage.AppendText("Connected to MQTT server！" + Environment.NewLine);
            })));
        }

        private static void MqttClient_Disconnected(object sender, EventArgs e)
        {
            staticMqtt.Dispatcher.Invoke((new Action(() =>
            {
                staticMqtt.txtReceiveMessage.Clear();
                DateTime curTime = new DateTime();
                curTime = DateTime.UtcNow;
                staticMqtt.txtReceiveMessage.AppendText($">> [{curTime.ToLongTimeString()}]");
                staticMqtt.txtReceiveMessage.AppendText("MQTT disconnected！" + Environment.NewLine);
            })));

            //Reconnecting
            if (isReconnect)
            {
                staticMqtt.Dispatcher.Invoke((new Action(() =>
                {
                    staticMqtt.txtReceiveMessage.AppendText("Trying to reconnect.." + Environment.NewLine);
                })));

                CreteClient();
                staticMqtt.Dispatcher.Invoke((new Action(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    try
                    {
                        await Global.mqttClient.ConnectAsync(Global.options);
                    }
                    catch
                    {
                        staticMqtt.txtReceiveMessage.AppendText("### RECONNECTING FAILED ###" + Environment.NewLine);
                    }
                })));
            }
            else
            {
                staticMqtt.Dispatcher.Invoke((new Action(() =>
                {
                    staticMqtt.txtReceiveMessage.AppendText("Offline！" + Environment.NewLine);
                })));
            }
        }
        private static void MqttClient_ApplicationMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            staticMqtt.Dispatcher.Invoke((new Action(() =>
            {
                staticMqtt.txtReceiveMessage.AppendText($">> {"### RECEIVED APPLICATION MESSAGE ###"}{Environment.NewLine}");
            })));
            staticMqtt.Dispatcher.Invoke((new Action(() =>
            {
                staticMqtt.txtReceiveMessage.AppendText($">> Topic = {e.ApplicationMessage.Topic}{Environment.NewLine}");
            })));
            staticMqtt.Dispatcher.Invoke((new Action(() =>
            {
                staticMqtt.txtReceiveMessage.AppendText($">> Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}{Environment.NewLine}");
                //MessageBox.Show(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
            })));
            staticMqtt.Dispatcher.Invoke((new Action(() =>
            {
                staticMqtt.txtReceiveMessage.AppendText($">> QoS = {e.ApplicationMessage.QualityOfServiceLevel}{Environment.NewLine}");
            })));
            staticMqtt.Dispatcher.Invoke((new Action(() =>
            {
                staticMqtt.txtReceiveMessage.AppendText($">> Retain = {e.ApplicationMessage.Retain}{Environment.NewLine}");
            })));

            if (e.ApplicationMessage.Topic == "peristaltic/settings")
            {
                /*try { MonitorPage.DataFromPump = JsonConvert.DeserializeObject<Classes.JSONSettings>((e.ApplicationMessage.Payload).ToString()); }

                catch (Exception) {}*/
                string IncomingJson = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                if (IncomingJson[0] == '[')
                    MonitorPage.DataFromPump = JsonConvert.DeserializeObject<List<Classes.Settings>>(IncomingJson);
            }

        }

        private void btnLogIn_Click(object sender, EventArgs e)
        {
            isReconnect = true;
            Task.Run(async () => { await ConnectMqttServerAsync(); await MQTT_Subscribe(); });
            //Task.Run(async () => { await ConnectMqttServerAsync(); await MQTT_Subscribe(); });
            //Task.Run(async () => { await MQTT_Subscribe(); });
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            isReconnect = false;
            try
            {
                Task.Run(async () => { await Global.mqttClient.DisconnectAsync(); });

            }
            catch (Exception)
            {
                //MessageBox.Show("nejsi pripojen");
                throw;
            }
        }

        private void TopicComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            topic = Topics[TopicComboBox.SelectedIndex].Name;
        }
    }

}