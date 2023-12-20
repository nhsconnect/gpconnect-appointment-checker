drop function if exists configuration.get_organisation_type;

create function configuration.get_organisation_type
(
)
returns table
(
    organisation_type_id smallint,
    organisation_type_code varchar(50),
    organisation_type_description varchar(100)
)
as $$
begin
	return query
	select
	    ot.organisation_type_id,
	    ot.organisation_type_code,
	    ot.organisation_type_description
	from configuration.organisation_type ot
	order by ot.organisation_type_description;
end;
$$ language plpgsql;
