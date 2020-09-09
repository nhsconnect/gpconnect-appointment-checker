CREATE FUNCTION logging.get_entry() RETURNS SETOF logging.entry
    LANGUAGE plpgsql
    AS $$
begin
	return query
	SELECT entry_id, url, referrer_url, description, ip, created_date, created_by, server, response_code, session_id, user_agent
	FROM logging.entry;	
end;
$$;