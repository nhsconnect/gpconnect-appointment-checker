insert into audit.entry_type (entry_type_id, entry_description, item1_description, item2_description) values (10, 'User status changed', 'old status', 'new status');
insert into audit.entry_type (entry_type_id, entry_description, item1_description, item2_description) values (11, 'User access level changed', 'old access level', 'new access level');
insert into audit.entry_type (entry_type_id, entry_description, item1_description, item2_description) values (12, 'User multi search enabled status changed', 'old multi search enabled status', 'new multi search enabled status');
insert into audit.entry_type (entry_type_id, entry_description) values (13, 'New user added');
insert into audit.entry_type (entry_type_id, entry_description, item1_description, item2_description) values (14, 'Email sent', 'email address of sender', 'email address of recipient');

alter table audit.entry add admin_user_id integer null;
alter table audit.entry add constraint audit_entry_adminuserid_fk foreign key (admin_user_id) references application.user (user_id);

create table configuration.email
(
    single_row_lock boolean,
    sender_address varchar(100),
    host_name varchar(100),
    port smallint,
    encryption varchar(10),
    authentication_required boolean,
    user_name varchar(100),
    password varchar(100),

    constraint configuration_email_singlerowlock_pk primary key (single_row_lock),
    constraint configuration_email_singlerowlock_ck check (single_row_lock = true),
    constraint configuration_email_senderaddress_ck check (char_length(trim(sender_address)) > 0),
    constraint configuration_email_hostname_ck check (char_length(trim(host_name)) > 0),
    constraint configuration_email_port_ck check (port > 0),
    constraint configuration_email_encryption_ck check (char_length(trim(encryption)) > 0)
);

grant select, insert, update on all tables in schema application to app_user;

insert into configuration.email
(
    single_row_lock, sender_address, host_name, port, encryption, authentication_required, user_name, password
)
values
(
    true, 'gpconnect.appointmentchecker@nhs.net', 'send.nhs.net', 587, 'TLS', true, '', ''
);

grant select, insert, update on all tables in schema configuration to app_user;
grant select, update on all sequences in schema configuration to app_user;
grant execute on all functions in schema configuration to app_user;