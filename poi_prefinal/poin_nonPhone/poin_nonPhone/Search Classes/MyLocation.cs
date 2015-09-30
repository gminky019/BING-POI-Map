using Bing.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poin_nonPhone
{
    public class MyLocation
    {
        public readonly string _baseURLNA = "http://spatial.virtualearth.net/REST/v1/data/f22876ec257b474b82fe2ffcb8393150/NavteqNA/NavteqPOIs";
        public readonly string _baseURLEU = "http://spatial.virtualearth.net/REST/v1/data/c2ae584bbccc4916a0acf75d1e6947b4/NavteqEU/NavteqPOIs";
        public double _radius { get; set; }
        public double _latitude { get; set; }
        public double _longitude { get; set; }
        public MainPage _view { get; set; }
        public MapLayer _mapLayer { get; set; }
        public Map _map { get; set; }
        public string _filter { get; set; }
        public bool _useFilter;
        public string _key;
        public string _searcher;
        private Dictionary<string, string> _entityTypes;

        public MyLocation(Map map, MapLayer mLayer, MainPage view, bool filter, string key)
        {
            _map = map;
            _mapLayer = mLayer;
            _view = view;
            _useFilter = filter;
            _key = key;
            _radius = 10;
            
            if(filter)
            {
                _entityTypes = new Dictionary<string,string>();
                setEntityDict();
            }
        }

        private void setEntityDict()
        {
            _entityTypes.Add("School", "8211");
            _entityTypes.Add("Bank", "6000");
            _entityTypes.Add("Restaurant", "5800");
            _entityTypes.Add("Shopping", "6512");
            _entityTypes.Add("Cinema", "7832");
        }
        public void findFilter (string filterName)
        {
            try
            {
                _filter = _entityTypes[filterName];
            }
            catch(Exception e)
            {
                throw new Exception(String.Format("Could not find {0} in the Entity Dictionary.", filterName));
            }
        }
        
    }
}
