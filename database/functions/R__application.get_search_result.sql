drop function if exists application.get_search_result;

create function application.get_search_result
(
	_search_result_id integer,
	_user_id integer
)
returns table
(
	
	search_result_id integer,
	search_group_id integer,
	response_payload text,
	provider_ods_code character varying, 
	consumer_ods_code character varying, 
	provider_organisation_name character varying, 
	provider_address text, 
	provider_postcode character varying, 
	consumer_organisation_name character varying,
	consumer_address text,
	consumer_postcode character varying,
	provider_publisher character varying
)
as $$
begin
	return query
	select
		sr.search_result_id,
		sr.search_group_id,
		sm.response_payload,
		sr.provider_ods_code,
		sr.consumer_ods_code,
		provider_organisation.organisation_name as provider_organisation_name,		
		CONCAT(provider_organisation.address_line_1, ', ', provider_organisation.address_line_2, ', ', provider_organisation.locality, ', ', provider_organisation.city, ', ', provider_organisation.county) as provider_address,
		provider_organisation.postcode as provider_postcode,
		consumer_organisation.organisation_name as consumer_organisation_name,
		CONCAT(consumer_organisation.address_line_1, ', ', consumer_organisation.address_line_2, ', ', consumer_organisation.locality, ', ', consumer_organisation.city, ', ', consumer_organisation.county) as consumer_address,
		consumer_organisation.postcode as consumer_postcode,
		sr.provider_publisher
	from
		application.search_result sr
		inner join application.search_group sg on sr.search_group_id = sg.search_group_id
		inner join application.user_session us on sg.user_session_id = us.user_session_id		
		inner join application.user u on us.user_id = u.user_id
		left outer join application.organisation provider_organisation on sr.provider_organisation_id = provider_organisation.organisation_id
		left outer join application.organisation consumer_organisation on sr.consumer_organisation_id = consumer_organisation.organisation_id		
		left outer join logging.spine_message sm on sr.search_result_id = sm.search_result_id
	where
		sr.search_result_id = _search_result_id
		and u.user_id = _user_id;
end;
$$ language plpgsql;