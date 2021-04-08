drop function if exists logging.purge_logs;

create function logging.purge_logs
(
)
returns void
as $$
declare
	_log_retention_days integer;
begin
	select 
		log_retention_days into _log_retention_days 
	from 
		configuration.general;

	if (_log_retention_days is null)
	then
		raise exception 'configuration.general.log_retention_days is not set';
		return;
	end if;

	delete
	from
		logging.error_log
	where 
		logged < (now() - (interval '1' day * _log_retention_days));

	delete from 
		logging.spine_message 
	where
		logged_date < (now() - (interval '1' day * _log_retention_days));

	delete from 
		logging.web_request
	where 
		created_date < (now() - (interval '1' day * _log_retention_days));
end;
$$ language plpgsql;