using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poin_nonPhone
{
    /// <summary>
    /// general poi class/obj make it easier to write to file, uses overloaded tostring
    /// </summary>
    public class POI
    {
        private string name;
        private string address;
        private string adminD;
        private string adminD2;
        private string country;
        private double Latitude;
        private double longitude;
        private string Locality;
        private string phone;
        private string post;
        private string type;
        
        public POI(string n, string addr, string ad, string ad2, string c, double lat, double lon, string local, string p, string po, string t)
        {
            name = n;
            address = addr;
            adminD = ad;
            adminD2 = ad2;
            country = c;
            Latitude = lat;
            longitude = lon;
            Locality = local;
            phone = p;
            post = po;
            type = t;
        }

        public override string ToString()
        {
            return String.Format("Name: {0} | Address: {1} | Locality: {2} | Ad District 2 (County): {3}| Ad District 1 (State): {4} | Postal Code: {5} | Country: {6} | Latitude & Longitude: {7}, {8} | Phone: {9} | Type: {10}", name,address, Locality, adminD2, adminD, post, country, Latitude.ToString(), longitude.ToString(), phone, type);
        }
    }
}
