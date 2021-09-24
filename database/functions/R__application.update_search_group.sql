drop function if exists application.update_search_group;

create function application.update_search_group
(
	_search_group_id integer,
	_user_id integer,
	_search_end_at timestamp
)
returns void 
as $$
begin
	update
		application.search_group sg
	set 
		search_end_at = _search_end_at
	from 
		application.user_session us, 
		application.user u
	where
	 	sg.user_session_id = us.user_session_id
		and us.user_id = u.user_id
		and u.user_id = _user_id
		and sg.search_group_id = _search_group_id;
end;
$$ language plpgsql;