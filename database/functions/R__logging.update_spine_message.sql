drop function if exists logging.update_spine_message;

create function logging.update_spine_message
(
    _spine_message_id integer,
    _search_result_id integer
)
returns void
as $$
begin

	update 
		logging.spine_message 
	set 
		search_result_id = _search_result_id 
	where 
		spine_message_id = _spine_message_id
		and search_result_id is null;
end;
$$ language plpgsql;