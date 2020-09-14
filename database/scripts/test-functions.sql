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
from audit.add_entry
(
    1,
    1,
    1,
    'one',
    'two',
    'three',
    'details',
    1
);

select * 
from configuration.get_general_configuration();

select * 
from configuration.get_spine_configuration();

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

select * 
from logging.log_spine_message
(
    1,
    1,
    'command',
    'headers',
    'payload',
    'status',
    'headers',
    'payload',
    1
);

select *
from logging.log_web_request
(
    1,
    1,
    'https://url',
    'https://url',
    'Description',
    '100.1.1.1',
    current_date::timestamp,
    'Test',
    'Test',
    '200',
    'Test',
    'Test'
);