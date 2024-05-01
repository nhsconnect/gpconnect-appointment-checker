drop function if exists reporting.get_capability_reports;

create function reporting.get_capability_reports
(
)
returns table
(
	report_name varchar(100),
	report_id varchar(100), 
	interaction json,
	workflow json
)
as $$
begin
	return query
	select
		rl.report_name,
		rl.report_id,
		rl.interaction,
		rl.workflow
	from 
		reporting.list rl
	where 
		(rl.interaction is not null or rl.workflow is not null)
		and rl.function_name is null;
end;
$$ language plpgsql;