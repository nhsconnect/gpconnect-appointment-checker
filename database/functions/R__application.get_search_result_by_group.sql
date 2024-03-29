drop function if exists application.get_search_result_by_group;

create function application.get_search_result_by_group
(
	_search_group_id integer,
	_user_id integer
)
returns table
(
	search_result_id integer,
	search_group_id integer,
	details text
)
as $$
begin
	return query
	select
		sr.search_result_id,
		sr.search_group_id,
		sr.details
	from
		application.search_result sr
		left outer join application.organisation provider_organisation on sr.provider_organisation_id = provider_organisation.organisation_id
		left outer join application.organisation consumer_organisation on sr.consumer_organisation_id = consumer_organisation.organisation_id
		inner join application.search_group sg on sr.search_group_id = sg.search_group_id
		inner join application.user_session us on sg.user_session_id = us.user_session_id
		inner join application.user u on us.user_id = u.user_id
	where
		sr.search_group_id = _search_group_id
		and u.user_id = _user_id;
end;
$$ language plpgsql;