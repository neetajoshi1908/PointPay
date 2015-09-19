using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointePayApp.Model
{
    class Area
    {
    }
    public class AreaRequest
    {
        public int country_id { get; set; }
        public int state_id { get; set; }
    }

    #region Json Serialization
    public class data_Area
    {
        public string countryId { get; set; }
        public string stateId { get; set; }
        public string areaId { get; set; }
        public string area { get; set; }

    }
    public class response_Area
    {
        public List<data_Area> data { get; set; }
        public bool message { get; set; }
    }
    public class RootObject_Area
    {
        public response_Area response { get; set; }
        public int success { get; set; }
        public int time { get; set; }
    }
    #endregion
}
