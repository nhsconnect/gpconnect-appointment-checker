insert into application.organisation 
(
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
    'G2Y0B',
    1,
    'GP CONNECT APPOINTMENT CHECKER',
    '1 TREVELYAN SQUARE',
    '',
    '',
    'LEEDS',
    'WEST YORKSHIRE',
    'LS1 6AE', 
    now(), 
    now()
);

update configuration.spine s
set organisation_id = o.organisation_id
from application.organisation o
where o.ods_code = 'G2Y0B'
and s.single_row_lock = true;
