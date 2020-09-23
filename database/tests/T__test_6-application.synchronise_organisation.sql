-- new organisation and new organisation type
select
	*
from application.synchronise_organisation
(
	_ods_code := 'A11111',
	_organisation_type_name := 'Test Organisation Type',
	_organisation_name := 'Test Organisation',
	_address_line_1 := '1 Main Street',
	_address_line_2 := '',
	_locality := '',
	_city := 'City',
	_county := '',
	_postcode := 'AA1 1AA',
	_is_gpconnect_consumer := true,
	_is_gpconnect_provider := true
);


-- new organisation and existing organisation type
select
	*
from application.synchronise_organisation
(
	_ods_code := 'B11111',
	_organisation_type_name := 'Test Organisation Type',
	_organisation_name := 'Test Organisation 2',
	_address_line_1 := '1 Main Street',
	_address_line_2 := '',
	_locality := '',
	_city := 'City',
	_county := '',
	_postcode := 'AA1 1AA',
	_is_gpconnect_consumer := true,
	_is_gpconnect_provider := true
);

-- existing organisation and existing organisation type
select
	*
from application.synchronise_organisation
(
	_ods_code := 'A11111',
	_organisation_type_name := 'Test Organisation Type',
	_organisation_name := 'Test Organisation - name changed',
	_address_line_1 := '2 Main Street',
	_address_line_2 := '',
	_locality := '',
	_city := 'City',
	_county := '',
	_postcode := 'AA2 1AA',
	_is_gpconnect_consumer := true,
	_is_gpconnect_provider := false
);