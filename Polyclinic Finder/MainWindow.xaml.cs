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
using System.Xml.Linq;
using GMap.NET.WindowsPresentation;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using GMap.NET.WindowsForms.Markers;
using System.Globalization;
using Yandex;
using Newtonsoft.Json;

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

            //Chrome.Address = "https://www.google.co.in/maps";
            List<Polyclinic> polyclinics = GetData();

            polyclinicList.ItemsSource = polyclinics;

            //string requestUri = string.Format("https://geocode-maps.yandex.ru/1.x/?format=json&geocode=" + "Астана, проспект Республика, дом 50");

            double lat, lng;

            lat = GetLocation("Астана, проспект Республика, дом 50").Item1;
            lng = GetLocation("Астана, проспект Республика, дом 50").Item2;

            GMapMarker marker = new GMapMarker(new PointLatLng(lat, lng));
            marker.Shape = new Ellipse
            {
                Width = 10,
                Height = 10,
                Stroke = Brushes.Black,
                StrokeThickness = 3
            };

            mapView.Markers.Add(marker);
        }

        public Tuple<double, double> GetLocation(string address)
        {
            string data;

            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;

                var json = client.DownloadString("https://geocode-maps.yandex.ru/1.x/?format=json&geocode=" + address);

                data = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(json));
            }

            JsonTextReader reader = new JsonTextReader(new StringReader(data));

            string latLng = "";

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.PropertyName && (string)reader.Value == "pos")
                {
                    latLng = reader.ReadAsString();
                }
            }

            double lat = 0, lng = 0;
            string[] coordinates = latLng.Split(' ');

            lat = double.Parse(coordinates[1].Replace('.', ','));
            lng = double.Parse(coordinates[0].Replace('.', ','));

            return Tuple.Create(lat, lng);
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
                //Chrome.Address = "https://www.google.co.in/maps?q=" + searchText.Text;\

                double lat = 0;
                double lng = 0;

                lat = GetLocation(searchText.Text).Item1;
                lng = GetLocation(searchText.Text).Item2;

                mapView.Position = new GMap.NET.PointLatLng(lat, lng);
                mapView.Zoom = 24;

                GMapMarker marker = new GMapMarker(new PointLatLng(lat, lng));
                //marker.Shape = new UIElement(Properties.Resources.maps_pin);
                //    new Ellipse
                //{
                //    Fill=Brushes.Green,
                //    Width = 20,
                //    Height = 20,
                //    Stroke = Brushes.Red,
                //    StrokeThickness = 3
                //};

                mapView.Markers.Add(marker);

                PointLatLng startPoint = new PointLatLng(lat, lng);
                PointLatLng endPoint = new PointLatLng(
                    GetLocation("проспект Республики 50, Астана").Item1,
                    GetLocation("проспект Республики 50, Астана").Item2);

                MapRoute route = GoogleMapProvider.Instance.GetRoute(startPoint, endPoint, false, true, 15);

                GMapRoute r = new GMapRoute(route.Points);

                mapView.Markers.Add(r);
            }
        }

        private void PolyclinicList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Chrome.Address = "https://www.google.co.in/maps?q=" + polyclinicList.SelectedItem.ToString();
            double lat = 0;
            double lng = 0;

            lat = GetLocation(polyclinicList.SelectedItem.ToString()).Item1;
            lng = GetLocation(polyclinicList.SelectedItem.ToString()).Item2;

            GMapMarker marker = new GMapMarker(new PointLatLng(lat, lng));
            marker.Shape = new Ellipse
            {
                Width = 10,
                Height = 10,
                Stroke = Brushes.Black,
                StrokeThickness = 3
            };

            mapView.Markers.Add(marker);
        }

        private void MapView_Loaded(object sender, RoutedEventArgs e)
        {
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache;

            mapView.MapProvider = GMap.NET.MapProviders.YandexMapProvider.Instance;
            mapView.MinZoom = 4;
            mapView.MaxZoom = 17;

            mapView.Zoom = 11;

            mapView.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;

            mapView.CanDragMap = true;

            mapView.DragButton = MouseButton.Left;

            mapView.ShowCenter = true;

            mapView.Position = new GMap.NET.PointLatLng(51.1801, 71.44598);
        }
    }
}
