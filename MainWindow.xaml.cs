using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Globalization;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Forms = System.Windows.Forms;
using System.Reflection;

namespace Wallpapering
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        private const int GWL_EX_STYLE = -20;
        private const int WS_EX_APPWINDOW = 0x00040000, WS_EX_TOOLWINDOW = 0x00000080;

        private bool invertClock = false;
        private bool twelveHr = false;
        private string background = "background.jpg";
        private int buttonSize = 64;
        private List<ConfigButton> buttons = new List<ConfigButton>();
        private Forms.NotifyIcon notifyIcon = new Forms.NotifyIcon() { Visible = false, Text = "Wallpapering - Running in background..." };

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
                wrpButtons.Height = conf.buttonSize;
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

        public void CreateButton(string uri, string icon)
        {
            Directory.CreateDirectory($"{Directory.GetCurrentDirectory()}\\icons");
            File.Copy(icon, $"{Directory.GetCurrentDirectory()}\\icons\\{System.IO.Path.GetFileName(icon)}", true);
            icon = System.IO.Path.GetFileName(icon);

            buttons.Add(new ConfigButton()
            {
                uri = uri,
                icon = icon
            });

            UpdateButtons();
        }

        public void LocalButton(object sender, EventArgs e)
        {
            string uri = "";
            string icon = "";

            OpenFileDialog ofd = new OpenFileDialog()
            {
                InitialDirectory = "C:\\",
                Filter = "All Files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
                Title = "Wallpapering - Select an application or file"
            };

            if (ofd.ShowDialog() == true)
            {
                uri = ofd.FileName;
            }

            ofd = new OpenFileDialog()
            {
                InitialDirectory = "C:\\",
                Filter = "Image files (*.png;*.jpg;*.jpeg;*.gif;*.ico)|*.png;*.jpg;*.jpeg;*.gif;*.ico",
                FilterIndex = 1,
                RestoreDirectory = true,
                Title = "Wallpapering - Select an icon"
            };

            if (ofd.ShowDialog() == true)
            {
                icon = ofd.FileName;
            }

            CreateButton(uri, icon);
            SaveConfig();
        }

        public void WebButton(object sender, EventArgs e)
        {
            string url = "";
            string icon = "";

            TextDialog txt = new TextDialog("Wallpapering", "Enter a URL:", "https://google.com/");

            if (txt.ShowDialog() == true)
            {
                OpenFileDialog ofd = new OpenFileDialog()
                {
                    InitialDirectory = "C:\\",
                    Filter = "Image files (*.png;*.jpg;*.jpeg;*.gif;*.ico)|*.png;*.jpg;*.jpeg;*.gif;*.ico",
                    FilterIndex = 1,
                    RestoreDirectory = true,
                    Title = "Wallpapering - Select an icon"
                };

                if (ofd.ShowDialog() == true)
                {
                    icon = ofd.FileName;
                }

                url = txt.Response;
                url = (url.Contains("https://") || url.Contains("http://")) ? url : $"http://{url}";

                CreateButton(url, icon);
                SaveConfig();
            }
        }
        
        public void SteamButton(object sender, EventArgs e)
        {
            string url = "";
            string icon = "";

            TextDialog txt = new TextDialog("Wallpapering", "Enter a Steam AppID:", "730");

            if (txt.ShowDialog() == true)
            {
                OpenFileDialog ofd = new OpenFileDialog()
                {
                    InitialDirectory = "C:\\",
                    Filter = "Image files (*.png;*.jpg;*.jpeg;*.gif;*.ico)|*.png;*.jpg;*.jpeg;*.gif;*.ico",
                    FilterIndex = 1,
                    RestoreDirectory = true,
                    Title = "Wallpapering - Select an icon"
                };

                if (ofd.ShowDialog() == true)
                {
                    icon = ofd.FileName;
                }

                url = $"steam://rungameid/{txt.Response}";

                CreateButton(url, icon);
                SaveConfig();
            }
        }

        private void UpdateWallpaper(string filename)
        {
            background = System.IO.Path.GetFileName(filename);

            File.Copy(filename, $"{Directory.GetCurrentDirectory()}\\{background}", true);
            meBackground.Source = new Uri($"{Directory.GetCurrentDirectory()}\\{background}", UriKind.Absolute);

            SaveConfig();
        }

        private void Main_Loaded(object sender, RoutedEventArgs e)
        {
            var helper = new WindowInteropHelper(this).Handle;
            SetWindowLong(helper, GWL_EX_STYLE, (GetWindowLong(helper, GWL_EX_STYLE) | WS_EX_TOOLWINDOW) & ~WS_EX_APPWINDOW);

            LoadConfig();

            notifyIcon.Click += delegate (object? sender, EventArgs e)
            {
                ShowWindow();
            };
            notifyIcon.Icon = new System.Drawing.Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream("Wallpapering.icon.ico")); 

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
            notifyIcon.Dispose();
        }

        private void Main_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                notifyIcon.Visible = true;
            } else if (this.WindowState == WindowState.Normal)
            {
                notifyIcon.Visible = false;
            }
        }

        private void Close(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void HideWindow(object sender, EventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void ShowWindow()
        {
            this.WindowState = WindowState.Normal;
        }

        private void WallpaperPicker(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                InitialDirectory = "%USERPROFILE%\\Pictures",
                Filter = "All Files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
                Title = "Wallpapering - Select a new wallpaper"
            };

            if (ofd.ShowDialog() == true)
            {
                UpdateWallpaper(ofd.FileName);
            }
        }

        private void InvertClock(object sender, EventArgs e)
        {
            invertClock = !invertClock;
            mnuInvert.IsChecked = invertClock;
        }

        private void TwelveHour(object sender, EventArgs e)
        {
            twelveHr = !twelveHr;
            mnuTwelve.IsChecked = twelveHr;
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
