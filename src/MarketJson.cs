using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;


namespace ppaocr {

    [DataContract]
    class MarketData
    {
        [DataMember]
        public string island { get; set; }

        [DataMember]
        public string ocean { get; set; }

        [DataMember]
        public Dictionary<string, Good> goods { get; set; }
    }


    [DataContract]
    class Good
    {
        [DataMember]
        public List<Order> buy_orders { get; set; }

        [DataMember]
        public List<Order> sell_orders { get; set; }
    }

    [DataContract]
    class Order
    {
        [DataMember]
        public string shop { get; set; }

        [DataMember]
        public int? price { get; set; }

        [DataMember]
        public int? amount { get; set; }
    }
}