using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointePayApp.Model
{
    class City
    {
    }
    public class CityRequest
    {
        public int country_id { get; set; }
        public int state_id { get; set; }
        public int area_id { get; set; }

    }
    #region Json Serialization
    public class data_City
    {
        public string countryId { get; set; }
        public string stateId { get; set; }
        public string zipId { get; set; } //here zipId is cityID
        public string city { get; set; }

    }
    public class response_City
    {
        public List<data_City> data { get; set; }
        public bool message { get; set; }
    }
    public class RootObject_City
    {
        public response_City response { get; set; }
        public int success { get; set; }
        public int time { get; set; }
    }
    #endregion
}
