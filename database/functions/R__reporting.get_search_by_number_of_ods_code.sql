drop function if exists reporting.get_multi_search_stats;

drop function if exists reporting.get_search_by_number_of_ods_code;

create function reporting.get_search_by_number_of_ods_code
(
)
returns table
(
	"Number of Provider ODS Codes" integer,
	"Number of Consumer ODS Codes" integer,
	"Count" bigint,
	"Average Total Response Time" text,
	"Average Response Per Code" text
)
as $$
begin
	create temp table table_total_response_times on commit drop as		
	select
		a.provider_ods_count, 
		a.consumer_ods_count, 
		avg(a.response_time_in_seconds) average_total_response_time_in_seconds
	from 
	(
		select 
			array_length(string_to_array(sg.provider_ods_textbox, ' '), 1) provider_ods_count,
			coalesce(array_length(string_to_array(sg.consumer_ods_textbox, ' '), 1), 0) consumer_ods_count,
			sum(sm.roundtriptime_ms)/1000 response_time_in_seconds		
		from
			application.search_group sg
			inner join application.search_result sr on sg.search_group_id = sr.search_group_id			
			inner join logging.spine_message sm on sr.search_result_id = sm.search_result_id
		group by
			sg.search_group_id
	) a 
	group by 
		a.provider_ods_count,
		a.consumer_ods_count;
	
	return query
	select
		* 
	from 
	(
		select
			1,
			1,
			count(*),
			to_char(sum(roundtriptime_ms)/1000 / count(*), 'FM9990.99"s"'),
			to_char(sum(roundtriptime_ms)/1000 / count(*), 'FM9990.99"s"')
		from
			logging.spine_message sm
		where
			sm.spine_message_type_id = 3 
			and sm.search_result_id is null			
			and position('ods-organization-code' in sm.request_payload) > 0
		union
		select
			1,
			0,
			count(*),
			to_char(sum(roundtriptime_ms)/1000 / count(*), 'FM9990.99"s"'),
			to_char(sum(roundtriptime_ms)/1000 / count(*), 'FM9990.99"s"')
		from
			logging.spine_message sm
		where
			sm.spine_message_type_id = 3 
			and sm.search_result_id is null			
			and position('ods-organization-code' in sm.request_payload) = 0
			and position('GPConnect-OrganisationType' in sm.request_payload) > 0
		union
		select 
			a.provider_ods_count,
			a.consumer_ods_count,
			a.count,
			to_char(t.average_total_response_time_in_seconds, 'FM9990.99"s"'),
			to_char(a.average_response_per_code_in_seconds, 'FM9990.99"s"')		
		from 
		(
			select 
				array_length(string_to_array(sg.provider_ods_textbox, ' '), 1) provider_ods_count,
				coalesce(array_length(string_to_array(sg.consumer_ods_textbox, ' '), 1), 0) consumer_ods_count,
				count(*) "count",
				avg(sm.roundtriptime_ms)/1000 average_response_per_code_in_seconds
			from
				application.search_group sg		
				inner join application.search_result sr on sg.search_group_id = sr.search_group_id			
				inner join logging.spine_message sm on sr.search_result_id = sm.search_result_id
			group by
				1,2
		) a 
	inner join table_total_response_times t on a.provider_ods_count = t.provider_ods_count and a.consumer_ods_count = t.consumer_ods_count
	) b
	order by
		1,2;


end;
$$ language plpgsql;