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
using System.Windows.Threading;
using System.Globalization;
using Newtonsoft.Json;
using System.IO;

namespace Wallpapering
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool invertClock = false;
        private bool twelveHr = false;
        private string background = "background.jpg";
        private int buttonSize = 100;
        private List<ConfigButton> buttons = new List<ConfigButton>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ResizeWindow()
        {
            this.Width = SystemParameters.WorkArea.Width;
            this.Height = SystemParameters.WorkArea.Height;
            this.Left = SystemParameters.WorkArea.Left;
            this.Top = SystemParameters.WorkArea.Top;
        }

        private void UpdateClock()
        {
            lblClock.Foreground = (invertClock) ? Brushes.Black : Brushes.White;
            lblDate.Foreground = (invertClock) ? Brushes.Black : Brushes.White;
            lblClock.Content = DateTime.Now.ToString((twelveHr) ? @"hh:mm:ss tt" : @"HH:mm:ss", CultureInfo.CurrentCulture);
            lblDate.Content = DateTime.Now.ToString(@"dddd, d MMMM yyyy", CultureInfo.CurrentCulture);
        }

        private void LoadConfig()
        {
            if (File.Exists("config.json")) {
                Config conf;

                using (StreamReader sr = new StreamReader("config.json"))
                {
                    conf = JsonConvert.DeserializeObject<Config>(sr.ReadToEnd());
                }

                twelveHr = conf.twelveHr;
                invertClock = conf.invertClock;
                background = conf.background;
                buttonSize = conf.buttonSize;
                buttons = conf.buttons;

                meBackground.Source = new Uri($"{Directory.GetCurrentDirectory()}\\{System.IO.Path.GetFileName(background)}", UriKind.Absolute);
                UpdateButtons();
            } else
            {
                SaveConfig();
                UpdateButtons();
                meBackground.Source = new Uri($"{Directory.GetCurrentDirectory()}\\{System.IO.Path.GetFileName(background)}", UriKind.Absolute);
            }
        }

        private void SaveConfig()
        {
            Config conf = new Config()
            {
                twelveHr = this.twelveHr,
                invertClock = this.invertClock,
                background = this.background,
                buttonSize = this.buttonSize,
                buttons = this.buttons
            };

            using (StreamWriter sw = new StreamWriter("config.json"))
            {
                sw.WriteLine(JsonConvert.SerializeObject(conf, Formatting.Indented));
            }
        }

        private void UpdateButtons()
        {
            wrpButtons.Children.Clear();

            foreach (ConfigButton configButton in buttons)
            {
                Button button = new Button();
                button.Content = "";
                button.Click += delegate (object sender, RoutedEventArgs e)
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                    {
                        Arguments = configButton.uri,
                        FileName = "explorer.exe"
                    });
                };
                button.Width = buttonSize;
                button.Height = buttonSize;
                button.Margin = new Thickness(10, 0, 10, 0);
                button.BorderThickness = new Thickness(0);
                button.Background = Brushes.Transparent;
                button.Content = new Image()
                {
                    Source = new BitmapImage(new Uri($"{Directory.GetCurrentDirectory()}\\icons\\{configButton.icon}", UriKind.Absolute)),
                    Stretch = Stretch.UniformToFill,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                wrpButtons.Children.Add(button);
            }
        }

        private void UpdateWallpaper(string filename)
        {
            File.Copy(filename, $"{Directory.GetCurrentDirectory()}\\{System.IO.Path.GetFileName(filename)}");
            meBackground.Source = new Uri($"{Directory.GetCurrentDirectory()}\\{System.IO.Path.GetFileName(filename)}", UriKind.Absolute);
        }

        private void Main_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();

            ResizeWindow();
            UpdateClock();

            meBackground.Stretch = Stretch.UniformToFill;

            DispatcherTimer clockTimer = new DispatcherTimer(DispatcherPriority.Render);
            clockTimer.Tick += delegate (object? sender, EventArgs e)
            {
                UpdateClock();
            };
            clockTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            clockTimer.Start();
        }

        private void Main_Closed(object sender, EventArgs e)
        {
            SaveConfig();
        }
    }

    struct Config
    {
        public bool invertClock;
        public bool twelveHr;
        public string background;
        public int buttonSize;
        public List<ConfigButton> buttons;
    }

    struct ConfigButton
    {
        public string uri;
        public string icon;
    }
}
