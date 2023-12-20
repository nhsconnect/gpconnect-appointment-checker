drop table if exists application.search_export cascade;

create table application.search_export
(
    search_export_id serial not null,
    user_id integer not null,
    search_export_data text not null,
    created_date timestamp not null default now(),
    constraint application_searchexport_searchexportid_pk primary key (search_export_id),
    constraint application_searchexport_userid foreign key (user_id) references application.user (user_id)
);

grant select, insert, update on all tables in schema application to app_user;
grant select, update on all sequences in schema application to app_user;
grant execute on all functions in schema application to app_user;