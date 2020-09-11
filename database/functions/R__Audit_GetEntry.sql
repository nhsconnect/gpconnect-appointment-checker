CREATE OR REPLACE FUNCTION audit.get_entry() RETURNS SETOF audit.entry
    LANGUAGE plpgsql
    AS $$
begin
	return query
	SELECT entry_id, ip, user_name, ods_code, created_date, logged_on, logged_off, description
	FROM audit.entry; 	
end;
$$;