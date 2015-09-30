using Bing.Maps.Search;
using NavteqPoiSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poin_nonPhone
{
    /// <summary>
    /// this is the Search class that implement IREST for a place search
    /// i.e. find location without LAT and LONG
    /// </summary>
    public class PlaceSearch : IREST
    {
        private Response poiResponse;
        public NavteqPoiSchema.Response _poiResponse
        {
            set{ poiResponse = value; }
            get { return poiResponse; }
        }

        private Search searches;
        public Search _searches
        {
            set { searches = value; }
            get { return searches; }
        }

        private MyLocation location;
        public MyLocation _location
        {
            get { return location; }
            set { location = value; }
        }


        public PlaceSearch(MyLocation loc, Search search)
        {
            _searches = search;
            _location = loc;
        }
        /// <summary>
        /// fromats url
        /// </summary>
        /// <param name="url">uri object </param>
        /// <returns></returns>
        public string formatURL(string url)
        {
            if (_location._useFilter)
            {
                return string.Format("{0}?spatialFilter=nearby({1:N5},{2:N5},{3})&$filter=EntityTypeID%20Eq%20{4}&$format=json&key={5}", url, _location._latitude, _location._longitude, _location._radius, _location._filter, _location._key);
            }
            else
            {
                return string.Format("{0}?spatialFilter=nearby({1:N5},{2:N5},{3})&$format=json&key={4}", url, _location._latitude, _location._longitude, _location._radius, _location._key);
            }
        }

        /// <summary>
        /// performs search with a search key
        /// </summary>
        /// <returns></returns>
        public async Task performSearch()
        {
            try
            {
                //clear map
                _location._mapLayer.Children.Clear();

                if(_location._searcher.Contains("Search Key") || string.IsNullOrWhiteSpace(_location._searcher))
                {
                    throw new FormatException("Search Key is empty or unchanged");
                }

                //find location and get the latitude and longitude
                await getLatLon();

                string mainSearchURL = formatURL(_searches.checkContinent(_location));

                //get the response
                _poiResponse = await _searches.performHttp<Response>(new Uri(mainSearchURL));

                if(_searches.checkNullPOI(_poiResponse))
                {
                    _searches.moveMap(_poiResponse, _location);
                }
                else
                {
                    throw new Exception("No POI's Available.");
                }
            }
            catch(Exception e)
            {
                throw new Exception("Error getting the points of interest.", e);
            }
        }

        /// <summary>
        /// gets the location only by using a place i.e. Reno as a search key
        /// </summary>
        /// <returns></returns>
        public async Task getLatLon()
        {
            LocationDataResponse responseLoc;
            try
            {
                //get the GEocode request
                GeocodeRequestOptions options = new GeocodeRequestOptions(_location._searcher);

                //get the location from the place
                responseLoc = await _location._map.SearchManager.GeocodeAsync(options);
           
                //as long as it not null set the lat amd long of the location
                if (_searches.checkNullLoc(responseLoc))
                {
                    _location._latitude = responseLoc.LocationData[0].Location.Latitude;
                    _location._longitude = responseLoc.LocationData[0].Location.Longitude;
                }
            }
            catch (Exception e)
            {
              throw new Exception("problem getting Location from key search", e);
            }

        }

    }
}
