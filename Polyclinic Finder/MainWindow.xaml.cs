﻿using System;
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

            string requestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false", Uri.EscapeDataString(polyclinicList.Items[0].ToString()));

            WebRequest request = WebRequest.Create(requestUri);
            WebResponse response = request.GetResponse();
            XDocument xdoc = XDocument.Load(response.GetResponseStream());

            XElement result = xdoc.Element("GeocodeResponse").Element("result");
            XElement locationElement = result.Element("geometry").Element("location");
            XElement lat = locationElement.Element("lat");
            XElement lng = locationElement.Element("lng");

            double latitude = double.Parse(lat.Value.Replace('.', ','));
            double longitude = double.Parse(lng.Value.Replace('.', ','));
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
                //Chrome.Address = "https://www.google.co.in/maps?q=" + searchText.Text;
            }
        }

        private void PolyclinicList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Chrome.Address = "https://www.google.co.in/maps?q=" + polyclinicList.SelectedItem.ToString();
            float lat = 0;
            float lng = 0;

            GeoCoderStatusCode status;
            PointLatLng? point = GMapProviders.GoogleMap.GetPoint(polyclinicList.SelectedItem.ToString(), out status);
            if (status == GeoCoderStatusCode.G_GEO_SUCCESS && point != null)
            {
                lat = (float)point.Value.Lat;
                lng = (float)point.Value.Lng;
            }

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

            mapView.MapProvider = GMap.NET.MapProviders.OpenStreetMapProvider.Instance;
            mapView.MinZoom = 4;
            mapView.MaxZoom = 17;

            mapView.Zoom = 11;

            mapView.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;

            mapView.CanDragMap = true;

            mapView.DragButton = MouseButton.Left;

            mapView.ShowCenter = false;

            mapView.Position = new GMap.NET.PointLatLng(51.1801, 71.44598);
        }
    }
}
