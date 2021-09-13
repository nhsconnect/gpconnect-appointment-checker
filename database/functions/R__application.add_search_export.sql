drop function if exists application.add_search_export;

create function application.add_search_export
(
	_search_export_data text,
	_user_id integer
)
returns table
(
	search_export_id integer
)
as $$
declare
	_search_export_id integer;
begin
	insert into application.search_export
	(
		search_export_data,
		user_id
	)
	values
	(
		_search_export_data,
		_user_id
	)
	returning
		application.search_export.search_export_id
	into 
		_search_export_id;	
		
	return query
	select
		se.search_export_id
	from 
		application.search_export se
	where
		se.search_export_id = _search_export_id;
end;
$$ language plpgsql;