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
		a.*
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
			array_length(string_to_array(sg.provider_ods_textbox, ' '), 1),
			coalesce(array_length(string_to_array(sg.consumer_ods_textbox, ' '), 1), 0),
			count(*),
			to_char(extract(second from avg(a.max_logged_date - sg.search_start_at)), 'FM9990.99"s"'),
			to_char(extract(second from avg(a.max_logged_date - sg.search_start_at)) / greatest(array_length(string_to_array(sg.provider_ods_textbox, ' '), 1), coalesce(array_length(string_to_array(sg.consumer_ods_textbox, ' '), 1), 0)), 'FM9990.99"s"')
		from
			application.search_group sg
			left outer join
			(
				select 
					sg.search_group_id,
					max(sm.logged_date) max_logged_date
				from
					logging.spine_message sm
					inner join application.search_result sr on sm.search_result_id = sr.search_result_id
					inner join application.search_group sg on sr.search_group_id = sg.search_group_id
				group by
				sg.search_group_id
			) a on sg.search_group_id = a.search_group_id
		group by
			array_length(string_to_array(sg.provider_ods_textbox, ' '), 1),
			coalesce(array_length(string_to_array(sg.consumer_ods_textbox, ' '), 1), 0)
	) a
	order by 
		1, 2;
end;
$$ language plpgsql;