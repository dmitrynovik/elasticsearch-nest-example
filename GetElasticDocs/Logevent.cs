using Nest;

namespace FindSinglePassengerNameMixup
{
    public class logevent
    {
        [Text(Name = "@timestamp")]
        public string Timestamp { get; set; }

        public string Message { get; set; }
        public string CorrelationId { get; set; }
    }
}
