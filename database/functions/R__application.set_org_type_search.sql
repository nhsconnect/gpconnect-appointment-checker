drop function if exists application.set_org_type_search;

create function application.set_org_type_search
(
	_admin_user_id int,
	_user_id int,
	_org_type_search_enabled boolean,
	_user_session_id int
)
returns void
as $$
begin	
	update application.user
	set
		org_type_search_enabled = _org_type_search_enabled
	where user_id = _user_id;

	perform audit.add_entry(_user_id, _user_session_id, 17, (NOT _org_type_search_enabled)::TEXT, _org_type_search_enabled::TEXT, null, null, null, _admin_user_id);
end;
$$ language plpgsql;