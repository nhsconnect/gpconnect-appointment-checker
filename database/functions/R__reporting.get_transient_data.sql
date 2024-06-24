drop function if exists reporting.get_transient_data;

create function reporting.get_transient_data
(
	_transient_report_id text
)
returns table
(
	transient_data json,
	transient_report_id text,
	transient_report_name text
)
as $$
begin	
	return query
	select 
		replace(substring(a.transient_data from 2 for length(a.transient_data)-2),'], [',',')::json as transient_data,
		a.transient_report_id,
		a.transient_report_name
	from (
		select
			json_agg(t.transient_data)::text as transient_data,
			t.transient_report_id,
			t.transient_report_name
		from 
			reporting.transient t
		where 
			UPPER(t.transient_report_id) = UPPER(_transient_report_id)
			and t.entry_date :: date = now() :: date
		group by
			t.transient_report_id,
			t.transient_report_name
		) a 
	group by 
		a.transient_data, 
		a.transient_report_id, 
		a.transient_report_name;
end;
$$ language plpgsql;

