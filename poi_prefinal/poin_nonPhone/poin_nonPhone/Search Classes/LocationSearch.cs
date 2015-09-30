using NavteqPoiSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poin_nonPhone
{
    /// <summary>
    /// Search class for location searches only (LAT and LONG already known)
    /// </summary>
    class LocationSearch:IREST
    {
        private Response poiResponse;
        public NavteqPoiSchema.Response _poiResponse
        {
            get
            {
                return poiResponse;
            }
            set
            {
                poiResponse = value;
            }
        }

        private Search searches;
        public Search _searches
        {
            get
            {
                return searches;
            }
            set
            {
                searches = value;
            }
        }
        private MyLocation location;
        public MyLocation _location
        {
            get
            {
                return location;
            }
            set
            {
                location = value;
            }
        }

        public LocationSearch(MyLocation loc, Search search)
        {
            _searches = search;
            _location = loc;
        }

        /// <summary>
        /// formats url
        /// </summary>
        /// <param name="url"></param>
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
        /// this will perform the search just by using the location obj LAt and LOng
        /// </summary>
        /// <returns></returns>
        public async Task performSearch()
        {
            try
            {
                //clear map
                _location._mapLayer.Children.Clear();

                //format URL
                string mainSearchURL = formatURL(_searches.checkContinent(_location));
                //get POIS
                _poiResponse = await _searches.performHttp<Response>(new Uri(mainSearchURL));

                if (_searches.checkNullPOI(_poiResponse))
                {
                    //move map if not null
                    _searches.moveMap(_poiResponse, _location);
                }
                else
                {
                    if(_poiResponse.ResultSet.Results.Length == 0)
                    {
                        throw new Exception("No POI's Available");
                    }
                }
            }
            catch(Exception e)
            {
                throw new Exception("Error getting points of interest.", e);
            }
        }
    }
}
