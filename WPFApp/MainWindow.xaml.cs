using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace WPFApp
{
    public class SingleMeasure
    {
        public float Measure { get; set; }
    }
    public partial class MainWindow : Window
    {
        private const int CaliperFormatLength = 7;
        private const float DefaultErrorMeasure = -1000.0F;
        private const int DelayTime = 500;
        private SingleMeasure CaliperMeasure { get; set; } = new SingleMeasure();
        private ControllerClient ControllerClient { get; set; } = new ControllerClient();
        private const string LocalIp = "127.0.0.1";
        private const int LocalPort = 5114;
        private const string NetworkIp = "192.168.137.1";
        private const int NetworkPort = 80;
        private string CurrentIp { get; set; }
        private int CurrentPort { get; set; }
        private bool Showed { get; set; } = false;
        private const int TimeInterval= 5;
        public MainWindow()
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(TimeInterval)
            };
            timer.Tick += CheckNetwork;
            timer.Start();
        }
        public void CheckNetwork(object sender, EventArgs e)
        {
            if (ControllerClient.DetectNetwork(LocalIp))
            {
                BorderStatus.BorderBrush = Brushes.LawnGreen;
                TextBoxIp.Text = LocalIp + ":" + LocalPort;
                CurrentIp = LocalIp;
                CurrentPort = LocalPort;
                return;
            }
            else if (ControllerClient.DetectNetwork(NetworkIp))
            {
                BorderStatus.BorderBrush = Brushes.LawnGreen;
                TextBoxIp.Text = NetworkIp + ":" + NetworkPort;
                CurrentIp = NetworkIp;
                CurrentPort = NetworkPort;
                return;
            }
            else if (!Showed)
            {
                MessageBox.Show("ASP Controller is unreachable, make sure the ASP server is running", "Server error", MessageBoxButton.OK, MessageBoxImage.Error);
                BorderStatus.BorderBrush = Brushes.Red;
                TextBoxIp.Text = "";
                CurrentIp = "";
                Showed = true;
            }
            else
            {
                BorderStatus.BorderBrush = Brushes.Red;
                TextBoxIp.Text = "";
                CurrentIp = "";
            }
        }
        async private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                string lastStr = textBox.Text.Replace('.', ',');
                if (lastStr.Length < CaliperFormatLength)
                {
                    return;
                }

                if (float.TryParse(lastStr, out float parsedStr))
                {
                    if (parsedStr <= 0f)
                    {
                        //\u211D is R (Real number symbol)
                        await Task.Run(() =>
                        {
                            MessageBox.Show("Invalid value, measure has to be in range \u211D+", "Measure error", MessageBoxButton.OK, MessageBoxImage.Error);
                        });
                            textBox.Text = "";
                        return;
                    }
                    ListBoxItem listBoxItem = new ListBoxItem
                    {
                        Content = DateTime.Now + " || " + parsedStr.ToString().Replace(',', '.')
                    };
                    CaliperMeasure.Measure = parsedStr;
                    
                    if (CurrentIp != "")
                    {
                        ListBoxOldMeasures.Items.Insert(0, listBoxItem);
                        ControllerClient.PutMeasure(CaliperMeasure, CurrentIp, CurrentPort);
                    }
                    else
                    {
                        await Task.Run(() =>
                        {
                            MessageBox.Show("No connection, can't send measure", "Connection error", MessageBoxButton.OK, MessageBoxImage.Error);
                        });
                    }
                    await Task.Delay(DelayTime);
                    textBox.Text = "";
                }
                else
                {
                    CaliperMeasure.Measure = DefaultErrorMeasure; //default error measure (out of range)
                    MessageBox.Show("Invalid value, measure has to be format +xxx.xx", "Measure error", MessageBoxButton.OK, MessageBoxImage.Error);
                    textBox.Text = "";
                }
            }
        }

        private void MenuItemDocumentation_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            const string Path = "\\CaliperDocumentation.pdf";
            string currentDir = Directory.GetCurrentDirectory();
            Uri pdf = new Uri(currentDir + Path, UriKind.RelativeOrAbsolute);
            try
            {
                process.StartInfo.FileName = pdf.LocalPath;
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load the documentation, make sure that " + pdf.ToString() + " exists\nDetails: " + ex.Message, "Documentation error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("WPFApp, Caliper interface\n2022, Quentin Fornerod", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
    public class ControllerClient
    {
        public bool DetectNetwork(string hostUri)
        {
            //doesn't specify the port but it takes way less time than a request
            Ping ping = new Ping();
            try
            {
                PingReply pingReply = ping.Send(hostUri);
                return pingReply.Status == IPStatus.Success;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to ping the address\nDetails: " 
                    + ex.Message, "Ping error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return false;
        }
        public async void PutMeasure(SingleMeasure CaliperMeasure, string CurrentIp, int CurrentPort)
        {
            string url = "http://" + CurrentIp + ":" + CurrentPort.ToString() + "/api/AR/PutMeasure";
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(CaliperMeasure);
            HttpWebRequest httpRequest = WebRequest.CreateHttp(url);
            httpRequest.Method = "PUT";
            httpRequest.ContentType = "application/json";
            try
            {
                using (var writer = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    writer.Write(jsonString);
                }
                using (var httpResponse = await httpRequest.GetResponseAsync())
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var Status = (HttpWebResponse)httpResponse;
                        if (Status.StatusCode != HttpStatusCode.OK)
                        {
                            throw new WebException();
                        }
                        var result = streamReader.ReadToEnd();
                        if (result == null)
                        {
                            throw new ArgumentNullException();
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                await Task.Run(() =>
                {
                    MessageBox.Show("Can't reach server\nDetails: " + ex.Message, "Server error", MessageBoxButton.OK, MessageBoxImage.Error);

                });
            }
            catch (ArgumentNullException ex)
            {
                MessageBox.Show("Bad request\nDetails: " + ex.Message, "Server error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can't reach server, launch the server or contact the administrator\nDetails: " + ex.Message, "Server error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
