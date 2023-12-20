drop function if exists application.get_search_result;

create function application.get_search_result
(
	_search_result_id integer,
	_user_id integer
)
returns table
(
	search_result_id integer, 
	search_group_id integer, 
	response_payload text, 
	details text, 
	search_duration_seconds double precision
)    
as $$
begin
	return query
	select
		sr.search_result_id,
		sr.search_group_id,		
		sm.response_payload,
		sr.details,
		sr.search_duration_seconds
	from
		application.search_result sr
		inner join application.search_group sg on sr.search_group_id = sg.search_group_id
		inner join application.user_session us on sg.user_session_id = us.user_session_id		
		inner join application.user u on us.user_id = u.user_id
		left outer join logging.spine_message sm on sr.search_result_id = sm.search_result_id
	where
		sr.search_result_id = _search_result_id
		and u.user_id = _user_id;
end;
$$ language plpgsql;