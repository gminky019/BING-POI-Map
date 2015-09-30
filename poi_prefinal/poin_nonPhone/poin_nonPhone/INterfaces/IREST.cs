using Bing.Maps;
using NavteqPoiSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poin_nonPhone
{
    public interface IREST
    {
        Response _poiResponse { get; set; }
        Search _searches { get; set;}
        MyLocation _location { get; set; }

        string formatURL(string url);
        Task performSearch();    
    }
}
