using Newtonsoft.Json;

namespace ParkHyderabad.Model.APIOutPutModel
{
    public class OTPResponse
    {
        [JsonProperty("Jod Id")]
        public int JobId { get; set; }
        public string Ack { get; set; }
        public string mobileNo { get; set; }
    }
}
