using System.Text.Json.Serialization;

namespace GratitudeLog.Models
{
    public class GratitudeEntry
    {
        public int ID { get; set; }
        public string Entry { get; set; } = string.Empty;
        public int Repeated { get; set; } = 1;
    }
}
