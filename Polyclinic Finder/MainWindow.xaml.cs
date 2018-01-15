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

            Chrome.Address = "https://www.google.co.in/maps";
            List<Polyclinic> polyclinics = GetData();

            polyclinicList.ItemsSource = polyclinics;
        }

        public List<Polyclinic> GetData()
        {
            List<Polyclinic> policData = new List<Polyclinic>();
            string data;

            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;

                var json = client.DownloadString(@"http://data.egov.kz/api/v2/emhanalar/v3?pretty");

                data = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(json));
            }

            policData = JsonConvert.DeserializeObject<List<Polyclinic>>(data);

            return policData;
        }

        private void SearchButton_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (searchText.Text == "Ваш адресс...")
            {
                searchText.Text = "";
                searchText.Foreground = Brushes.Black;
            }
        }

        private void SearchButton_LostFocus(object sender, RoutedEventArgs e)
        {
            if (searchText.Text == "")
            {
                searchText.Text = "Ваш адресс...";
                searchText.Foreground = Brushes.Gray;
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (searchText.Text == "Ваш адресс...")
                MessageBox.Show("Введите адрес!");
            else
            {
                Chrome.Address = "https://www.google.co.in/maps?q=" + searchText.Text;
            }
        }

        private void PolyclinicList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Chrome.Address = "https://www.google.co.in/maps?q=" + polyclinicList.SelectedItem.ToString();
        }
    }
}
