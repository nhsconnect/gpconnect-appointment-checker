drop function if exists application.purge_search_results;

create function application.purge_search_results
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

	delete from 
		application.search_group where search_group_id in 
	(
		select 
			search_group_id 
		from 
			application.search_group 
		where 
			search_start_at < (now() - (interval '1' day * _log_retention_days))
	);

	delete from 
		application.search_result where search_result_id in 
	(
		select 
			sr.search_result_id 
		from 
			application.search_result sr 
		inner join 
		(
			select 
				search_group_id, 
				search_start_at 
			from
				application.search_group
			where 
				search_start_at < (now() - (interval '1' day * 10))
		) sg 
		on sr.search_group_id = sg.search_group_id
	);
end;
$$ language plpgsql;