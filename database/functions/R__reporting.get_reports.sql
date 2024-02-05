drop function if exists reporting.get_reports;

create function reporting.get_reports
(
)
returns table
(
	report_name varchar(100), 
	function_name varchar(100)
)
as $$
begin
	return query
	select
		r.report_name,
		r.function_name
	from 
		reporting.list r
	where 
		r.interaction_id is null
		and r.function_name is not null
	order by 
		r.report_name;	
end;
$$ language plpgsql;
