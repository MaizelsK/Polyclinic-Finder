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

            Map.Navigate("https://www.google.co.in/maps");
            List<Polyclinic> polyclinics;

            string data;

            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;

                var json = client.DownloadString(@"http://data.egov.kz/api/v2/emhanalar/v3?pretty");

                data = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(json));

                //MessageBox.Show(convertedJson);
            }

            polyclinics = JsonConvert.DeserializeObject<List<Polyclinic>>(data);
        }

        private void searchButton_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (searchText.Text == "Ваш адресс...")
            {
                searchText.Text = "";
                searchText.Foreground = Brushes.Black;
            }
        }

        private void searchButton_LostFocus(object sender, RoutedEventArgs e)
        {
            if (searchText.Text == "")
            {
                searchText.Text = "Ваш адресс...";
                searchText.Foreground = Brushes.Gray;
            }
        }

        private void SeachButton_Click(object sender, RoutedEventArgs e)
        {
            if (searchText.Text == "Ваш адресс...")
                MessageBox.Show("Введите адрес!");
        }
    }
}
