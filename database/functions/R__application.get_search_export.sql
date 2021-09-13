drop function if exists application.get_search_export;

create function application.get_search_export
(
	_search_export_id integer,
	_user_id integer
)
returns table
(	
	search_export_id integer,
	search_export_data text
)
as $$
begin
	return query
	select
		se.search_export_id,
		se.search_export_data
	from
		application.search_export se,
		application.user u
	where
		se.search_export_id = _search_export_id
		and u.user_id = _user_id;
end;
$$ language plpgsql;