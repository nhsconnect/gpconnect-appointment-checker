create or replace function caching.delete_expired_cache_items
(
	_utc_now timestamp with time zone
)
returns void
as $$
begin
	delete from 
		caching.dist_cache 
	where
		_utc_now > ExpiresAtTime;
end;
$$ language plpgsql;