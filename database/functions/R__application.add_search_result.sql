drop function if exists application.add_search_result;

create function application.add_search_result
(
	_search_group_id integer,
	_provider_ods_code varchar(200),
	_consumer_ods_code varchar(200),
	_error_code integer,
	_details character varying(8000),
	_provider_publisher varchar(200),
	_search_duration_seconds double precision
)
returns table
(
	search_result_id integer,
	search_group_id integer	
)
as $$
declare
	_search_result_id integer;
	_provider_organisation_id integer;
	_consumer_organisation_id integer;
begin
	select 
		o.organisation_id into _provider_organisation_id
	from application.organisation o
	where o.ods_code = _provider_ods_code;

	select 
		o.organisation_id into _consumer_organisation_id
	from application.organisation o
	where o.ods_code = _consumer_ods_code;

	insert into application.search_result
	(
		search_group_id, 
		provider_ods_code,		
		provider_organisation_id,
		consumer_ods_code,
		consumer_organisation_id,
		error_code,
		details,
		provider_publisher,
		search_duration_seconds
	)
	values
	(
		_search_group_id,
		_provider_ods_code,
		_provider_organisation_id,
		_consumer_ods_code,
		_consumer_organisation_id,
		_error_code,
		_details,
		_provider_publisher,
		_search_duration_seconds
	)
	returning
		application.search_result.search_result_id
	into 
		_search_result_id;	
		
	return query
	select
		sr.search_result_id,
		sr.search_group_id
	from 
		application.search_result sr
	where
		sr.search_result_id = _search_result_id;
end;
$$ language plpgsql;