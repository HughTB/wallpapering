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
using System.Windows.Shapes;

namespace Wallpapering
{
    /// <summary>
    /// Interaction logic for TextDialog.xaml
    /// </summary>
    public partial class TextDialog : Window
    {
        public string Response = "";

        public TextDialog(string title, string question, string placeholder)
        {
            InitializeComponent();

            Title = title;
            lblQuestion.Content = question;
            //txtResponse.Text = placeholder;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Response = txtResponse.Text;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
