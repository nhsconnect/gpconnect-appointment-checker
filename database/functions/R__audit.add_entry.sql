drop function if exists audit.add_entry;

create function audit.add_entry
(
    _user_id integer,
    _entry_type_id integer,
    _item1 varchar(1000) default null,
    _item2 varchar(1000) default null,
    _item3 varchar(1000) default null,
    _details varchar(1000) default null,
    _entry_elapsed_ms integer default null,
    _admin_user_id integer default null
)
returns void
as $$
declare	_user_session_id integer;
begin
	select 
		user_session_id into _user_session_id 
	from
		application.user_session 
	where
		user_id = _user_id 
		and end_time is null 
	order by 
		start_time desc 
	limit 1;

    insert into audit.entry
    (
    	user_id,
    	user_session_id,
    	entry_type_id,
    	item1,
    	item2,
    	item3,
    	details,
    	entry_elapsed_ms,
    	entry_date,
	admin_user_id
    )
    values
    (
    	_user_id,
    	_user_session_id,
    	_entry_type_id,
    	_item1,
    	_item2,
    	_item3,
    	_details,
    	_entry_elapsed_ms,
    	now(),
	_admin_user_id
    );

end;
$$ language plpgsql;