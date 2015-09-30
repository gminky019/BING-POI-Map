using Bing.Maps;
using Bing.Maps.Search;
using NavteqPoiSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace poin_nonPhone
{
    public class Search
    {
        /// <summary>
        /// This is the HTTP GET Method
        /// </summary>
        /// <typeparam name="Response"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
      public async Task<Response> performHttp<Response>(Uri url)
      {
          try
          {
              //get http response
              HttpClient clientHttp = new HttpClient();
              HttpResponseMessage responseFromHttp = await clientHttp.GetAsync(url);

              //deserialization
              using (var stream = await responseFromHttp.Content.ReadAsStreamAsync())
              {
                  DataContractJsonSerializer deserial = new DataContractJsonSerializer(typeof(Response));
                  return (Response)deserial.ReadObject(stream);
              }
          }
          catch (Exception e)
          {
              throw new Exception("Something happened while getting the http request or deserialization. See inner Exception.", e);
          }
      }

        /// <summary>
        /// this is the method that focuses the map and populates the pins
        /// </summary>
        /// <param name="poi"></param>
        /// <param name="loc"></param>
     public void moveMap(Response poi, MyLocation loc)
      {
         //instantiate obj
          LocationCollection locationsOnMap = new LocationCollection();
         
         //each result set pin and place it on map
           foreach(Result results in poi.ResultSet.Results)
           {
               Location mapLocation = new Location(results.Latitude, results.Longitude);

               locationsOnMap.Add(mapLocation);

               Pushpin pin = new Pushpin();
               MapLayer.SetPosition(pin, mapLocation);
               loc._mapLayer.Children.Add(pin);
           }
         //show map and zoom
           loc._map.SetView(new LocationRect(locationsOnMap));
      }

        /// <summary>
        /// checks what continent the search is on, determines the URL
        /// </summary>
        /// <param name="_location"></param>
        /// <returns></returns>
     public string checkContinent(MyLocation _location)
     {
         if (_location._longitude < -30)
         {
             return _location._baseURLNA;
         }
         else
         {
             return _location._baseURLEU;
         }
     }

        /// <summary>
        /// checks if the location obj is not null
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
     public bool checkNullLoc(LocationDataResponse obj)
     {
         if (obj == null || obj.LocationData == null)
         {
             return false;
         }
         else
         {
             if (obj.LocationData.Count > 0)
             {
                 return true;
             }
             return false;
         }
     }

        /// <summary>
        /// checks if the POI object is not null
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
     public bool checkNullPOI(Response obj)
     {
         if (obj == null || obj.ResultSet == null || obj.ResultSet.Results == null)
         {
             return false;
         }
         else
         {
             if (obj.ResultSet.Results.Length > 0)
             {
                 return true;
             }
             return false;
         }
     }

        /// <summary>
        /// converts results to a list of strings
        /// </summary>
        /// <param name="_poiResponse"></param>
        /// <returns></returns>
     public List<string> searchOutput(Response _poiResponse)
     {
         List<string> sList = new List<string>();

         if (_poiResponse == null)
         {
             throw new Exception("POI response is null when trying to convert to list of strings.");
         }

         foreach (Result poi in _poiResponse.ResultSet.Results)
         {
             POI pointOfInt = new POI(poi.DisplayName, poi.AddressLine, poi.AdminDistrict, poi.AdminDistrict2, poi.CountryRegion, poi.Latitude, poi.Longitude, poi.Locality, poi.Phone, poi.PostalCode, poi.EntityTypeID);
             sList.Add(pointOfInt.ToString());
             sList.Add(" ");
         }
         return sList;
     }

    }
}
