drop table if exists application.search_group cascade;

create table application.search_group
(
    search_group_id serial not null,
    user_session_id integer not null,

    consumer_ods_textbox varchar(200) not null,
    provider_ods_textbox varchar(200) not null,
    selected_daterange varchar(200) not null,

    search_start_at timestamp not null,
    search_end_at timestamp null,

    constraint application_searchgroup_searchid_pk primary key (search_group_id),
    constraint application_searchgroup_usersessionid_fk foreign key (user_session_id) references application.user_session (user_session_id)
);

drop table if exists application.search_result cascade;

create table application.search_result
(
    search_result_id serial not null,
    search_group_id integer not null,

    consumer_ods_code varchar(10) not null,
    consumer_organisation_id integer null,
    provider_ods_code varchar(10) not null,
    provider_organisation_id integer null,
    error_code integer null,
    details character varying(8000) null,
    provider_publisher varchar(200),

    constraint application_searchresult_searchresultid_pk primary key (search_result_id),
    constraint application_searchresult_searchgroupid_fk foreign key (search_group_id) references application.search_group (search_group_id),

    constraint application_searchresult_consumerodscode_ck_1 check (char_length(trim(consumer_ods_code)) > 0),
    constraint application_searchresult_consumerodscode_ck_2 check (upper(consumer_ods_code) = consumer_ods_code),
    constraint application_searchresult_consumerorganisationid foreign key (consumer_organisation_id) references application.organisation (organisation_id),

    constraint application_searchresult_providerodscode_ck_1 check (char_length(trim(provider_ods_code)) > 0),
    constraint application_searchresult_providerodscode_ck_2 check (upper(provider_ods_code) = provider_ods_code),
    constraint application_searchresult_providerorganisationid_fk foreign key (provider_organisation_id) references application.organisation (organisation_id)
);

alter table logging.spine_message add search_result_id integer null;
alter table logging.spine_message add constraint logging_spinemessage_searchresultid_fk foreign key (search_result_id) references application.search_result (search_result_id);

grant select, insert, update on all tables in schema application to app_user;
grant select, update on all sequences in schema application to app_user;
grant execute on all functions in schema application to app_user;