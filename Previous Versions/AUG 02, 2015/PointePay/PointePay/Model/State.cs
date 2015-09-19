using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointePay.Model
{
    class State
    {
    }
    public class StateRequest
    {
        public int country_id { get; set; }
    }

    #region Json Serialization
    public class data_State
    {
        public string stateId { get; set; }
        public string countryId { get; set; }
        public string stateName { get; set; }

    }
    public class response_State
    {
        public List<data_State> data { get; set; }
        public bool message { get; set; }
    }
    public class RootObject_State
    {
        public response_State response { get; set; }
        public int success { get; set; }

        public int time { get; set; }
    }
    #endregion
}
