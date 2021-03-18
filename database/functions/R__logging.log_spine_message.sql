drop function if exists logging.log_spine_message;

create function logging.log_spine_message
(
    _user_session_id integer,
    _spine_message_type_id integer,
    _command varchar(8000),
    _request_headers text,
    _request_payload text,
    _response_status varchar(100),
    _response_headers text,
    _response_payload text,
    _roundtriptime_ms integer,
    _search_result_id integer
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
	roundtriptime_ms bigint,
	search_result_id integer
)
as $$
declare _spine_message_id integer;
begin

	insert into logging.spine_message
	(
		user_session_id,
		spine_message_type_id,
		command,
		request_headers,
		request_payload,
		response_status,
		response_headers,
		response_payload,
		logged_date,
		roundtriptime_ms,
		search_result_id
	)
	values
	(
		_user_session_id,
		_spine_message_type_id,
		_command,
		_request_headers,
		_request_payload,
		_response_status,
		_response_headers,
		_response_payload,
		now(),
		_roundtriptime_ms,
		_search_result_id
	)
	returning
		logging.spine_message.spine_message_id 
	into 
		_spine_message_id;

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
	where
		sm.spine_message_id = _spine_message_id;

end;
$$ language plpgsql;