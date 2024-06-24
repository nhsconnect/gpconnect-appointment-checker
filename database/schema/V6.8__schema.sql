drop table if exists reporting.transient;

create table reporting.transient
(
	transient_id text not null,
	transient_data json not null,
	transient_report_id text not null,
	transient_report_name text not null,
	entry_date timestamp without time zone not null
);

grant select, insert on table reporting.transient to app_user;