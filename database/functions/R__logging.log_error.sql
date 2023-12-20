drop function if exists logging.log_error;

create function logging.log_error
(
	_application text,
	_level text,
	_message text,
	_logger text, 
	_callsite text, 
	_exception text,
	_user_id integer
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

	insert into logging.error_log
	(
		application,
		logged,
		level,
		user_id,
		user_session_id,
		message,
		logger,
		callsite,
		exception
	)
	values
	(
		_application,
		now(),
		_level,
		_user_id,
		_user_session_id,
		_message,
		_logger,
		_callsite,
		_exception
	);

end;
$$ language plpgsql;