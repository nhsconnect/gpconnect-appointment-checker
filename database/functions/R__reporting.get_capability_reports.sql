drop function if exists reporting.get_capability_reports;

create function reporting.get_capability_reports
(
)
returns table
(
	report_name varchar(100), 
	interaction json,
	workflow json
)
as $$
begin
	return query
	select
		r.report_name,
		r.interaction,
		r.workflow
	from 
		reporting.list r
	where 
		(r.interaction is not null or r.workflow is not null)
		and r.function_name is null;
end;
$$ language plpgsql;