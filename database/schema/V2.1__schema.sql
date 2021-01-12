create schema caching;

drop table caching.dist_cache;

create table caching.dist_cache
(
    Id text not null,
    Value bytea,
    ExpiresAtTime timestamp with time zone,
    SlidingExpirationInSeconds double precision,
    AbsoluteExpiration timestamp with time zone,

	constraint caching_distcache_id_pk primary key (Id)
);

grant usage on schema caching to app_user;
grant select, insert, update, delete on all tables in schema caching to app_user;
grant select, update on all sequences in schema caching to app_user;
grant execute on all functions in schema caching to app_user;