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
using System.IO;
using Newtonsoft.Json;
using System.Net;

namespace Polyclinic_Finder
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            List<Polyclinic> polyclinics = new List<Polyclinic>();

            string data;

            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;

                var json = client.DownloadString(@"http://data.egov.kz/api/v2/emhanalar/v3?pretty");

                data = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(json));

                //MessageBox.Show(convertedJson);
            }

            Polyclinic[] a = JsonConvert.DeserializeObject<Polyclinic[]>(data);
        }

        private void searchButton_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (searchButton.Text == "Ваш адресс...")
            {
                searchButton.Text = "";
                searchButton.Foreground = Brushes.Black;
            }
        }

        private void searchButton_LostFocus(object sender, RoutedEventArgs e)
        {
            if (searchButton.Text == "")
            {
                searchButton.Text = "Ваш адресс...";
                searchButton.Foreground = Brushes.Gray;
            }
        }
    }
}
