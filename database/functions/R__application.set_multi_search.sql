drop function if exists application.set_multi_search;

create function application.set_multi_search
(
	_admin_user_id integer,
	_user_id integer,
	_multi_search_enabled boolean
)
returns void
as $$
begin
	update
		application.user
	set
		multi_search_enabled = _multi_search_enabled
	where 
		user_id = _user_id;

	perform audit.add_entry(_user_id, 12, (NOT _multi_search_enabled)::TEXT, _multi_search_enabled::TEXT, null, null, null, _admin_user_id);
end;
$$ language plpgsql;
