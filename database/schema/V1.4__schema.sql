-- add new get access email address column
alter table configuration.general add column get_access_email_address varchar null;
alter table configuration.general add constraint configuration_general_getaccessemailaddress_ck check (char_length(trim(get_access_email_address)) > 0);
update configuration.general set get_access_email_address = 'gpconnect@nhs.uk';
alter table configuration.general alter column get_access_email_address set not null;