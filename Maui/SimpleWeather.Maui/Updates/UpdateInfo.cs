using System.Runtime.Serialization;

namespace SimpleWeather.Maui.Updates
{
    public class UpdateInfo
    {
        [DataMember(Name = "version")]
        public int VersionCode { get; set; }
        [DataMember(Name = "updatePriority")]
        public int UpdatePriority { get; set; }
    }
}
