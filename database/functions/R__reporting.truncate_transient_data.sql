drop function if exists reporting.truncate_transient_data;

create function reporting.truncate_transient_data
(
)
returns void
as $$
begin	
	truncate table reporting.transient;
end;
$$ language plpgsql;