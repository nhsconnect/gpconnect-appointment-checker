drop function if exists reporting.add_transient_data;

create function reporting.add_transient_data
(
	_transient_id varchar(100),
	_transient_data json,
	_transient_report_id varchar(100),
	_transient_report_name varchar(100)
)
returns void
as $$
begin	
	insert into reporting.transient(transient_id, transient_data, transient_report_id, transient_report_name, entry_date)
	values (_transient_id, _transient_data, _transient_report_id, _transient_report_name, now());
end;
$$ language plpgsql;