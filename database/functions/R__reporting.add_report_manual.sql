drop function if exists reporting.add_report_manual;

create function reporting.add_report_manual
(
	_report_name varchar(100),
	_function_name varchar(100)
)
returns void
as $$
begin	
	IF NOT EXISTS (SELECT * FROM reporting.list WHERE report_name = _report_name) THEN
		INSERT INTO reporting.list (report_name, function_name) VALUES (_report_name, _function_name);
	END IF;
end;
$$ language plpgsql;