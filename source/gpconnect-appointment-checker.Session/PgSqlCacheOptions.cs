using Microsoft.Extensions.Options;
using System;

namespace gpconnect_appointment_checker.Caching
{
    public class PgSqlCacheOptions : IOptions<PgSqlCacheOptions>
    {
        public TimeSpan ExpiredItemsDeletionInterval { get; set; }

        public TimeSpan DefaultSlidingExpiration { get; set; }

        PgSqlCacheOptions IOptions<PgSqlCacheOptions>.Value => this;
    }
}
