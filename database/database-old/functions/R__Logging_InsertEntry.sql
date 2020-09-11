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