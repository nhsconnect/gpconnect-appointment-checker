drop function if exists application.update_search_result;

create function application.update_search_result
(
	_search_result_id integer,
	_details text,
	_error_code integer,
	_search_duration_seconds double precision
)
returns void 
as $$
begin
	update
		application.search_result sr
	set 
		details = _details,
		error_code = _error_code,
		search_duration_seconds = _search_duration_seconds
	where
	 	sr.search_result_id = _search_result_id;
end;
$$ language plpgsql;