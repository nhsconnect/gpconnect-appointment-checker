CREATE FUNCTION audit.get_entry() RETURNS SETOF audit.entry
    LANGUAGE plpgsql
    AS $$
begin
	return query
	SELECT entry_id, ip, user_name, ods_code, created_date, logged_on, logged_off, description
	FROM audit.entry; 	
end;
$$;

CREATE FUNCTION audit.insert_entry(entryid integer, entryip character varying, username character varying, odscode character varying, createddate date, loggedon date, loggedoff date, entrydescription character varying) RETURNS SETOF audit.entry
    LANGUAGE plpgsql
AS $
begin

	insert into audit.entry
(
	entry_id,
	ip,
	user_name,
	ods_code,
	created_date,
	logged_on,
	logged_off,
	description
)
values
($1, $2, $3, $4, $5, $6, $7, $8);
end;
$$;

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

CREATE FUNCTION configuration.get_entrybykey(configurationkey character varying) RETURNS TABLE(entry_id integer, key character varying, value character varying)
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

CREATE FUNCTION logging.get_entry() RETURNS SETOF logging.entry
    LANGUAGE plpgsql
    AS $$
begin
	return query
	SELECT entry_id, url, referrer_url, description, ip, created_date, created_by, server, response_code, session_id, user_agent
	FROM logging.entry;	
end;
$$;


CREATE FUNCTION logging.insert_entry(entryid integer, entryurl character varying, entryreferrerurl character varying, entrydescription character varying, entryip character varying, entrycreateddate date, entrycreatedby character varying, entryserver character varying, entryresponsecode integer, entrysessionid character varying, entryuseragent character varying) RETURNS SETOF logging.entry
    LANGUAGE plpgsql
    AS $$
begin

	insert into logging.entry
(
	entry_id,
	url,
	referrer_url,
	description,
	ip,
	created_date,
	created_by,
	server,
	response_code,
	session_id,
	user_agent
)
values
($1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11);	
end;
$$;