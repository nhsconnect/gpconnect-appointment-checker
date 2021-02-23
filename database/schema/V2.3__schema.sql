insert into application.organisation_type
(
	organisation_type_id,
	organisation_type_name
)
values
(
	-1,
	'Unknown'
);

insert into application.organisation
(
	organisation_id,
	ods_code,
	organisation_type_id,
	organisation_name,
	address_line_1,
	address_line_2,
	locality,
	city,
	county,
	postcode,
	added_date,
	last_sync_date
)
values
(
	-1,
	'UNKNOWN',
	-1,
	'Unknown',
	'Unknown',
	'',
	'',
	'Unknown',
	'',
	'UU1 1UU',
	now(),
	now()
);
