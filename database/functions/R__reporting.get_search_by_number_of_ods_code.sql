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
	return query
	select
		a.number_of_provider_codes,
		a.number_of_consumer_codes,
		a.count,
		to_char(a.average_total_response_time, 'FM9990.99"s"'),
		to_char(a.average_response_per_code, 'FM9990.99"s"')
	from 
	(
		select
			1 "number_of_provider_codes",
			1 "number_of_consumer_codes",
			count(*) "count",
			round(sum(roundtriptime_ms)/1000 / count(*), 2) "average_response_per_code",
			round(sum(roundtriptime_ms)/1000 / count(*), 2) "average_total_response_time"
		from
			logging.spine_message sm
		where
			sm.spine_message_type_id = 3 
			and sm.search_result_id is null			
			and position('ods-organization-code' in sm.request_payload) > 0
		union
		select
			1 "number_of_provider_codes",
			0 "number_of_consumer_codes",
			count(*) "count",
			round(sum(roundtriptime_ms)/1000 / count(*), 2) "average_response_per_code",
			round(sum(roundtriptime_ms)/1000 / count(*), 2) "average_total_response_time"
		from
			logging.spine_message sm
		where
			sm.spine_message_type_id = 3 
			and sm.search_result_id is null			
			and position('ods-organization-code' in sm.request_payload) = 0
			and position('GPConnect-OrganisationType' in sm.request_payload) > 0
		union
		select 
			array_length(string_to_array(sg.provider_ods_textbox, ' '), 1) "number_of_provider_codes",
			array_length(string_to_array(sg.consumer_ods_textbox, ' '), 1) "number_of_consumer_codes",
			count(*) "count",
			round(sum(sm.roundtriptime_ms)/1000 / greatest(array_length(string_to_array(sg.provider_ods_textbox, ' '), 1), array_length(string_to_array(sg.consumer_ods_textbox, ' '), 1)), 2) "average_response_per_code",
			round(sum(sm.roundtriptime_ms)/1000, 2) "average_total_response_time"
		from
			logging.spine_message sm
			inner join application.search_result sr on sm.search_result_id = sr.search_result_id
			inner join application.search_group sg on sr.search_group_id = sg.search_group_id
		where
			sm.spine_message_type_id = 3 
			and sm.search_result_id is not null
			and position('ods-organization-code' in sm.request_payload) > 0
		group by
			array_length(string_to_array(sg.provider_ods_textbox, ' '), 1),
			array_length(string_to_array(sg.consumer_ods_textbox, ' '), 1)
		union
		select 
			array_length(string_to_array(sg.provider_ods_textbox, ' '), 1),
			coalesce(array_length(string_to_array(sg.consumer_ods_textbox, ' '), 1), 0),
			count(*),
			round(sum(sm.roundtriptime_ms)/1000 / greatest(array_length(string_to_array(sg.provider_ods_textbox, ' '), 1), array_length(string_to_array(sg.consumer_ods_textbox, ' '), 1)), 2) "average_response_per_code",
			round(sum(sm.roundtriptime_ms)/1000, 2) "average_total_response_time"
		from
			logging.spine_message sm
			inner join application.search_result sr on sm.search_result_id = sr.search_result_id
			inner join application.search_group sg on sr.search_group_id = sg.search_group_id
		where
			sm.spine_message_type_id = 3 
			and sm.search_result_id is not null
			and position('ods-organization-code' in sm.request_payload) = 0
			and position('GPConnect-OrganisationType' in sm.request_payload) > 0
		group by
			array_length(string_to_array(sg.provider_ods_textbox, ' '), 1),
			array_length(string_to_array(sg.consumer_ods_textbox, ' '), 1)
	) a 
	order by 2,1,3;
end;
$$ language plpgsql;