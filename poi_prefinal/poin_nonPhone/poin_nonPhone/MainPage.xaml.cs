using Bing.Maps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace poin_nonPhone
{
    /// <summary>
    /// Main Page
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public VM viewmodel;
        public bool byAddr;
        public bool isChecked = false;
        private Geolocator gg = null;
        public double _changedRadius = 10;
        public string key;
        public Map _map;
        public MapLayer _mapLayer;
        public bool _searchWithFilter = false;
        public string _filter;

        /// <summary>
        /// Initial page, intializes map and objects
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();

            Map.Loaded += (s, e) =>
            {
                try
                {
                    init();
                }
                catch (Exception ex)
                {

                }
            };
        }

        /// <summary>
        /// Initializes values and points on map
        /// </summary>
        public async void init()
        {
            key = await Map.GetSessionIdAsync();
            _mapLayer = new MapLayer();
            _map = Map;
            _map.Children.Add(_mapLayer);
        }

        /// <summary>
        /// if they hitt search button event, will search for what the user wants
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Tapped_Search(object sender, TappedRoutedEventArgs e)
        {
            //find out whether the fileter box is checked
            try
            {
                _searchWithFilter = (bool)FilterCheck.IsChecked;

                //get the location from a search input and set values in the location object
                MyLocation location = new MyLocation(_map, _mapLayer, this, _searchWithFilter, key);
                location._searcher = SearchInput.Text;
                location._radius = _changedRadius;

                //if searching with filter, find the filter value
                if (_searchWithFilter)
                {
                    //pick filter
                    location.findFilter(_filter);
                }

                //instantiate VM and call search by filter
                viewmodel = new VM(this);
                viewmodel.search(true, location);
            }
            catch(Exception ex)
            {
                //display exception
                var messageDialog = new MessageDialog(ex.Message);
                messageDialog.ShowAsync();
            }
        }

        /// <summary>
        /// will search based off of longitude and latitude
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Tapped_Loc(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                _searchWithFilter = (bool)FilterCheck.IsChecked;
                MyLocation location = new MyLocation(_map, _mapLayer, this, _searchWithFilter, key);

                //set values using the given lat AND  long
                location._radius = _changedRadius;
                location._latitude = Convert.ToDouble(Latitude.Text);
                location._longitude = Convert.ToDouble(Longitude.Text);

                if (_searchWithFilter)
                {
                    //pick filter
                    location.findFilter(_filter);
                }

                //search just by loc
                viewmodel = new VM(this);
                viewmodel.search(false, location);
            }
            catch(Exception ex)
            {
                //display exception
                var messageDialog = new MessageDialog(ex.Message);
                messageDialog.ShowAsync();
            }
        }


        /// <summary>
        /// event for searching poi based on your own location from device
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Tapped_MyLoc(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                //check if there isnt a Gelocator object, if there isnt instantiate the object
                if (gg == null)
                {
                    gg = new Geolocator();
                }

                //get position
                Geoposition position = await gg.GetGeopositionAsync();

                //fill out forms
                Latitude.Text = String.Format("LAT: {0}", position.Coordinate.Point.Position.Latitude.ToString());
                Longitude.Text = String.Format("LONG: {0}", position.Coordinate.Point.Position.Longitude.ToString());
                SearchInput.Text = String.Format("Source: {0} , Country: {1}", position.Coordinate.PositionSource.ToString(), position.CivicAddress.Country);

                _searchWithFilter = (bool)FilterCheck.IsChecked;

                MyLocation location = new MyLocation(_map, _mapLayer, this, _searchWithFilter, key);

                //set Location OBJ LAt and Long
                location._radius = _changedRadius;
                location._latitude = position.Coordinate.Point.Position.Latitude;
                location._longitude = position.Coordinate.Point.Position.Longitude;

                if (_searchWithFilter)
                {
                    //pick filter
                    location.findFilter(_filter);
                }

                //start search just by loc only
                viewmodel = new VM(this);
                viewmodel.search(false, location);
            }
            catch(Exception ex)
            {
                var messageDialog = new MessageDialog(ex.Message);
                messageDialog.ShowAsync();
            }
        }

        /// <summary>
        /// if the user changes radius, set the new value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Tapped_Radius(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                _changedRadius = Convert.ToDouble(Radius.Text);
            }
            catch (Exception ex)
            {


            }

        }

        /// <summary>
        /// if user changes the filter obj, change to new value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterChooser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _filter = (string)FilterChooser.SelectedItem;
        }

        /// <summary>
        /// if user wants to save to file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextFile_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if(viewmodel != null)
            {
                viewmodel.SaveFile();
            }
        }

        /// <summary>
        /// makes sure that when user clicks into box the current text dissapears.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= TextBox_GotFocus;
        }


    }
}
