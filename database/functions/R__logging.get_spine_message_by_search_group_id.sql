drop function if exists logging.get_spine_message_by_search_group_id;

create function logging.get_spine_message_by_search_group_id
(
    _search_group_id integer
)
returns table
(	
	spine_message_id integer,
	spine_message_type_id smallint,
	user_session_id integer,
	command character varying(8000),
	request_headers text,
	request_payload text,
	response_status character varying(100),
	response_headers text,
	response_payload text,
	logged_date timestamp without time zone,
	roundtriptime_ms double precision,
	search_result_id integer
)
as $$
begin
	return query
	select
		sm.spine_message_id,
		sm.spine_message_type_id,
		sm.user_session_id,
		sm.command,
		sm.request_headers,
		sm.request_payload,
		sm.response_status,
		sm.response_headers,
		sm.response_payload,
		sm.logged_date,
		sm.roundtriptime_ms,
		sm.search_result_id
	from 
		logging.spine_message sm
	inner join
		application.search_result sr on sm.search_result_id = sr.search_result_id
	where
		sr.search_group_id = _search_group_id;	
end;
$$ language plpgsql;