drop function if exists logging.log_web_request;

create function logging.log_web_request
(
	_user_id integer,
	_url text,
	_referrer_url text,
	_description text,
	_ip text,
	_created_date timestamp with time zone,
	_created_by text,
	_server text,
	_response_code integer,
	_session_id text,
	_user_agent text
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

	insert into logging.web_request
	(
		user_id,
		user_session_id,
		url,
		referrer_url,
		description,
		ip,
		created_date,
		created_by,
		server,
		response_code,
		session_id,
		user_agent
	)
	values
	(
		_user_id,
		_user_session_id,
		_url,
		_referrer_url,
		_description,
		_ip,
		_created_date,
		_created_by,
		_server,
		_response_code,
		_session_id,
		_user_agent
	);

end;
$$ language plpgsql;