alter table configuration.general add column last_access_highlight_threshold_in_days integer null;
update configuration.general set last_access_highlight_threshold_in_days = 30;
alter table configuration.general alter column last_access_highlight_threshold_in_days set not null;

alter table application.user add column terms_and_conditions_accepted boolean null;

create table application.user_account_status
(
    user_account_status_id smallint NOT NULL,
    description varchar(100) NOT NULL,

    constraint application_useraccountstatus_useraccountstatusid_pk PRIMARY KEY (user_account_status_id),
    constraint application_useraccountstatus_description_ck check (char_length(trim(description)) > 0)
);

GRANT UPDATE, INSERT, SELECT ON TABLE application.user_account_status TO app_user;

CREATE UNIQUE INDEX application_useraccountstatus_description_ix
    ON application.user_account_status USING btree
    (lower(description::text) COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default;

INSERT INTO application.user_account_status(user_account_status_id, description) VALUES (1, 'Pending');
INSERT INTO application.user_account_status(user_account_status_id, description) VALUES (2, 'Authorised');
INSERT INTO application.user_account_status(user_account_status_id, description) VALUES (3, 'Deauthorised');
INSERT INTO application.user_account_status(user_account_status_id, description) VALUES (4, 'Request Denied');

alter table application.user add column user_account_status_id integer null;

alter table application.user add constraint application_user_useraccountstatusid_fk 
foreign key (user_account_status_id) references application.user_account_status (user_account_status_id);

update application.user set user_account_status_id = 2 where is_authorised='true';
update application.user set user_account_status_id = 3 where is_authorised='false';

alter table application.user drop column is_authorised;

insert into application.email_template
(
    email_template_id, subject, body
)
values
(3, 'GP Connect Appointment Checker - User Details Form', 
'A new user create account form has been posted with the following details:\n
\n
Email Address: <email_address>\n
Job Role: <job_role>\n
Organisation: <organisation_name>\n
Access Reason: <access_reason>');


