drop function if exists application.get_search_group;

create function application.get_search_group
(
	_search_group_id integer,
	_user_id integer
)
returns table
(
	search_group_id integer, 
	provider_ods_textbox varchar(200),
	consumer_ods_textbox varchar(200),
	selected_daterange varchar(200),
	search_start_at timestamp,
	search_end_at timestamp
) 
as $$
begin
	return query
	select
		sg.search_group_id,
		sg.consumer_ods_textbox,
		sg.provider_ods_textbox,
		sg.selected_daterange,
		sg.search_start_at,
		sg.search_end_at
	from application.search_group sg
	inner join application.user_session us on sg.user_session_id = us.user_session_id
	inner join application.user u on us.user_id = u.user_id
	where u.user_id = _user_id
	and sg.search_group_id = _search_group_id;
end;
$$ language plpgsql;