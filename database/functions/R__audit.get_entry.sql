
create or replace function audit.get_entry
(
	
)
returns table
(
	entry_id integer,
	description varchar(100)
)
as $$
begin

	return query
	select
		entry_id,
		description
	from audit.entry;
 	
end;
$$ language plpgsql;

