drop function if exists reporting.get_transient_data;

create function reporting.get_transient_data
(
	_transient_report_id varchar(100)
)
returns table
(
	transient_id varchar(100), 
	transient_data json,
	transient_report_id varchar(100),
	transient_report_name varchar(100)
)
as $$
begin	
	return query
	select
		t.transient_id,
		t.transient_data,
		t.transient_report_id,
		t.transient_report_name
	from 
		reporting.transient t
	where 
		t.transient_report_id = _transient_report_id
		and entry_date - now() = 0;
end;
$$ language plpgsql;

