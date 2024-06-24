drop function if exists reporting.add_transient_data;

create function reporting.add_transient_data
(
	_transient_id text,
	_transient_data text,
	_transient_report_id text,
	_transient_report_name text
)
returns void
as $$
begin	
	insert into reporting.transient(transient_id, transient_data, transient_report_id, transient_report_name, entry_date)
	values (_transient_id, _transient_data::json, _transient_report_id, _transient_report_name, now());
end;
$$ language plpgsql;