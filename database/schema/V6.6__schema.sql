drop table if exists reporting.transient;

create table reporting.transient
(
	transient_id varchar(100) not null,
	transient_data json not null,
	transient_report_id varchar(100) not null,
	transient_report_name varchar(100) not null,
	entry_date timestamp without time zone not null
);

grant select, insert, update, delete on table reporting.transient to app_user;
