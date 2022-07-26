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

namespace Wallpapering
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool invertClock = false;
        private bool twelveHr = false;

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
            lblClock.Content = DateTime.Now.ToString((twelveHr) ? @"hh:mm:ss tt" : @"HH:mm:ss", CultureInfo.CurrentCulture);
            lblDate.Content = DateTime.Now.ToString(@"dddd, d MMMM yyyy", CultureInfo.CurrentCulture);
        }

        private void LoadConfig()
        {

        }

        private void SaveConfig()
        {

        }

        private void Main_Loaded(object sender, RoutedEventArgs e)
        {
            ResizeWindow();
            UpdateClock();

            DispatcherTimer clockTimer = new DispatcherTimer(DispatcherPriority.Render);
            clockTimer.Tick += delegate (object? sender, EventArgs e)
            {
                UpdateClock();
            };
            clockTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            clockTimer.Start();

            lblClock.Foreground = (invertClock) ? Brushes.White : Brushes.Black;
            lblDate.Foreground = (invertClock) ? Brushes.White : Brushes.Black;
        }
    }
}
