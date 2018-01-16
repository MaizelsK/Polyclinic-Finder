using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Polyclinic_Finder
{
    public class Polyclinic
    {
        public string Id { get; set; }
        [JsonProperty("naimenovanie_oganizacii")]
        public string Name { get; set; }
        [JsonProperty("adress")]
        public string Address { get; set; }
        [JsonProperty("adress_saita")]
        public string WebName { get; set; }
        [JsonProperty("kontact_tel")]
        public string Phone { get; set; }
        [JsonProperty("fio_rukovoditel")]
        public string LeaderFIO { get; set; }
        [JsonProperty("vremia_priema")]
        public string RecieveTime { get; set; }

        public override string ToString()
        {
            return Address;
        }
    }
}
