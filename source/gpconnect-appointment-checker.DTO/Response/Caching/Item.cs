using System;

namespace gpconnect_appointment_checker.DTO.Response.Caching
{
    public class Item
    {
        public string Id { get; set; }
        public byte[] Value { get; set; }
        public DateTimeOffset ExpiresAtTime { get; set; }
        public long SlidingExpirationInSeconds { get; set; }
        public DateTimeOffset AbsoluteExpiration { get; set; }
    }
}
