drop function if exists application.add_search_result;

create function application.add_search_result
(
	_search_group_id integer
)
returns table
(
	search_result_id integer,
	search_group_id integer	
)
as $$
declare
	_search_result_id integer;	
begin
	insert into application.search_result
	(
		search_group_id	
	)
	values
	(
		_search_group_id
	)
	returning
		application.search_result.search_result_id
	into 
		_search_result_id;	
		
	return query
	select
		sr.search_result_id,
		sr.search_group_id
	from 
		application.search_result sr
	where
		sr.search_result_id = _search_result_id;
end;
$$ language plpgsql;