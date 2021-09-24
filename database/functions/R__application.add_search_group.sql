drop function if exists application.add_search_group;

create function application.add_search_group
(
	_user_session_id integer,
	_consumer_ods_textbox varchar(200),
	_provider_ods_textbox varchar(200),
	_search_date_range varchar(200),
	_search_start_at timestamp,
	_consumer_organisation_type_dropdown varchar(50)
)
returns table
(
	search_group_id integer,
	user_session_id integer,
	consumer_ods_textbox varchar(200),
	provider_ods_textbox varchar(200),
	selected_daterange varchar(200),
	search_start_at timestamp,
	consumer_organisation_type_dropdown varchar(50)
)
as $$
declare
	_search_group_id integer;
begin
	insert into application.search_group
	(
		user_session_id, 
		consumer_ods_textbox,
		provider_ods_textbox,
		selected_daterange,
		search_start_at,
		consumer_organisation_type_dropdown
	)
	values
	(
		_user_session_id,
		_consumer_ods_textbox,
		_provider_ods_textbox,
		_search_date_range,
		_search_start_at,
		_consumer_organisation_type_dropdown
	)
	returning
		application.search_group.search_group_id
	into 
		_search_group_id;
		
	return query
	select
		sg.search_group_id,
		sg.user_session_id,
		sg.consumer_ods_textbox,
		sg.provider_ods_textbox,
		sg.selected_daterange,
		sg.search_start_at,
		sg.consumer_organisation_type_dropdown
	from 
		application.search_group sg
	where
		sg.search_group_id = _search_group_id;
end;
$$ language plpgsql;