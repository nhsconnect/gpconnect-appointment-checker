create table application.email_template
(
    email_template_id smallint NOT NULL,
    subject varchar(100),
    body text,

    constraint application_emailtemplate_emailtemplateid_pk PRIMARY KEY (email_template_id),
    constraint application_emailtemplate_subject_ck check (char_length(trim(subject)) > 0),
    constraint application_emailtemplate_body_ck check (char_length(trim(body)) > 0)
);

insert into application.email_template
(
    email_template_id, subject, body
)
values
(
    1, 'GP Connect Appointment Checker - New Account Created', 
'You have been granted access to the GP Connect Appointment Checker Tool.

To use the tool, go to <url> and follow the instructions. Sign on with the email address that this email was sent to, and your usual password.

For more help, use the "Help" link which appears on the Appointment Checker Tool pages.'
);

insert into application.email_template
(
    email_template_id, subject, body
)
values
(
    2, 'GP Connect Appointment Checker - Account Deactivated', 
'Your access to the GP Connect Appointment Checker Tool has been removed. 

If you still need access, please email <address> with your name, email address, organisation, role and reason.'
);

insert into audit.entry_type (entry_type_id, entry_description, item1_description, item2_description, item3_description) values (15, 'User ran multiple slot search', 'consumer ods codes', 'provider ods codes', 'selected date range');

insert into audit.entry_type (entry_type_id, entry_description, item1_description) values (16, 'User create account');

UPDATE audit.entry_type SET entry_description='User ran single slot search', item3_description='selected date range' WHERE entry_type_id=4;

grant select, insert, update on all tables in schema application to app_user;
grant execute on all functions in schema application to app_user;
