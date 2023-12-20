select 
	organisation_id,
	ods_code,
	organisation_type_name,
	organisation_name,
	address_line_1,
	address_line_2,
	locality,
	city,
	county,
	postcode
from application.get_organisation
(
    _ods_code := 'X26010'
);
