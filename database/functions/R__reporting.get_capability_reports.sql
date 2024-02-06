drop function if exists reporting.get_capability_reports;

create function reporting.get_capability_reports
(
)
returns table
(
	report_name varchar(100), 
	interaction_id varchar(100)
)
as $$
begin
	return query
	select
		r.report_name,
		r.interaction_id
	from 
		reporting.list r
	where 
		r.interaction_id is not null
		and r.function_name is null
	order by 
		r.report_name;	
end;
$$ language plpgsql;