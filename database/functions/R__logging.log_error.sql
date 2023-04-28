drop function if exists logging.log_error;

create function logging.log_error
(
	_application text,
	_logged timestamp with time zone,
	_level text,
	_message text,
	_logger text, 
	_callsite text, 
	_exception text,
	_user_id integer default null,
	_user_session_id integer default null
)
returns void
as $$
begin

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
		_logged,
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