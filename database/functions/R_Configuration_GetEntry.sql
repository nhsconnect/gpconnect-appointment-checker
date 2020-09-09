CREATE FUNCTION configuration.get_entry() RETURNS SETOF configuration.entry
    LANGUAGE plpgsql
    AS $$
begin

	return query
	select
		entry_id,
		key,
		value
	from configuration.entry;
 	
end;
$$;