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