alter table application.search_result add consumer_organisation_type varchar(50) null;
alter table application.search_result alter column consumer_ods_code drop not null;
alter table application.search_group add consumer_organisation_type_dropdown varchar(50) null;
alter table application.search_group alter column consumer_ods_textbox  drop not null;
