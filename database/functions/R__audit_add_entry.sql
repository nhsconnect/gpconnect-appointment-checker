create or replace function audit.add_entry
(
    _user_id integer,
    _user_session_id integer,
    _entry_type_id integer,
    _item1 varchar(100),
    _item2 varchar(100),
    _item3 varchar(100),
    _details varchar(1000),
    _entry_elapsed_ms integer
)
returns void
as $$
begin

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
    	entry_date
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
    	now()
    );

end;
$$ language plpgsql;