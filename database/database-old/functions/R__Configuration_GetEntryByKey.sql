CREATE FUNCTION configuration.get_entrybykey(configurationkey character varying) RETURNS SETOF configuration.entry
    LANGUAGE plpgsql
    AS $$
begin

	return query
	select
		entry_id,
		key,
		value
	from 
		configuration.entry
	where
		key = $1; 	
end;
$$;