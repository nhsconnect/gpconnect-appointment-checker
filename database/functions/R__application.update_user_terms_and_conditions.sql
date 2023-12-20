drop function if exists application.update_user_terms_and_conditions;

create function application.update_user_terms_and_conditions
(
	_user_id integer,
	_accepted boolean
)
returns void
as $$
begin
	update
		application.user
	set
		terms_and_conditions_accepted = _accepted
	where
		user_id = _user_id;	
end;
$$ language plpgsql;