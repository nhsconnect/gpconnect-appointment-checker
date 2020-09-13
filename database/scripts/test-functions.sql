select * 
from configuration.get_general_configuration();

select * 
from configuration.get_spine_configuration();

select *
from application.get_organisation
(
    'YES'
);

select * 
from application.synchronise_organisation
(
    'ABC',
    'GP Practice',
    'Main Street Surgery',
    '1 Main Street',
    '',
    '',
    'Leeds',
    '',
    'LS1 1AA',
    false,
    false
);

select *
from application.logon_user
(
    'test@test.com',
    'Test Person',
    1
);

update application.user 
set
    is_authorised = true,
    authorised_date = now()
where email_address = 'test@test.com';

select *
from application.logon_user
(
    'test@test.com',
    'Test Person',
    1
);

select * 
from application.logoff_user
(
    'test@test.com',
    1
);

select * 
from logging.log_error
(
    'Appointments Checker', 
    current_date::timestamp, 
    'Trace', 
    'Test', 
    'SQL script', 
    null, 
    null
);