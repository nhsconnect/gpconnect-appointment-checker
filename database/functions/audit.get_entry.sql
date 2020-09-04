
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
		e.entry_id,
		e.description
	from audit.entry e;
 	
end;
$$ language plpgsql;

