/*
    Schema V1.0: Initial schema
*/

/* 
    create schemas
*/
create schema application;
create schema configuration;
create schema log;
create schema audit;

/*
    create tables - application
*/
create table application.organisation_type
(
    organisation_type_id smallint not null,
    organisation_type_name varchar(200) not null,

    constraint application_organisationtype_organisationtypeid_pk primary key (organisation_type_id),
    constraint application_organisationtype_organisationtypename check (char_length(trim(spine_message_type_name)) > 0),
);

create unique index application_organisationtype_organisationtypename_ix on application.organisation_type (lower(organisation_type_name));

create table application.organisation
(
    organisation_id serial not null,
    ods_code varchar(10) not null,
    organisation_type_id smallint not null,
    organisation_name varchar(100) not null,
    address_line_1 varchar(100) not null,
    address_line_2 varchar(100) not null,
    locality varchar(100) not null,
    city varchar(100) not null,
    county varchar(100) not null,
    postcode varchar(100) not null,
    is_gpconnect_consumer boolean not null,
    is_gpconnect_provider boolean not null,
    added_date timestamp not null,
    last_sync_date timestamp not null,

    constraint application_organisation_organisationid_pk primary key (organisation_id),
    constraint application_organisation_odscode_uq unique (ods_code), 
    constraint application_organisation_odscode_ck_1 check (char_length(trim(ods_code)) > 0),
    constraint application_organisation_odscode_ck_2 check (upper(ods_code) = ods_code),
    constraint application_organisation_organisationtypeid_fk foreign key (organisation_type_id) references application.organisation_type (organisation_type_id),
    constraint application_organisation_organisationname_ck check (char_length(trim(organisation_name)) > 0)
);

create table application.user
(
    user_id serial not null,
    email_address varchar(200) not null,
    display_name varchar(100) not null,
    organisation_id integer not null,
    user_auth_status boolean not null,

    added_date timestamp not null,
    last_loggedin_date timestamp not null,

    constraint application_user_userid_pk primary key (user_id),
    constraint application_user_emailaddress_ck check (char_length(trim(email_address)) > 0),
    constraint application_user_displayname_ck check (char_length(trim(display_name)) > 0),
    constraint application_user_organisationid_pk foreign key (organisation_id) references application.organisation (organisation_id)
);

create unique index application_user_emailaddress_ix on application.user (lower(email_address));

/*
    create tables - configuration
*/
create table configuration.general
(
    single_row_lock boolean,
    product_name varchar(100),
    product_version varchar(100),
    max_num_weeks_search smallint,

    constraint configuration_general_singlerowlock_pk primary key (single_row_lock),
    constraint configuration_general_singlerowlock_ck check (single_row_lock = true)
    constraint configuration_general_productname_ck check (char_length(trim(product_name)) > 0),
    constraint configuration_general_productversion_ck check (char_length(trim(product_version)) > 0),
    constraint configuration_general_maxnumweekssearch_ck check (max_num_weeks_search > 0)
);

create table configuration.spine
(
    single_row_lock boolean,
    ssp_hostname varchar(100) not null,
    sds_hostname varchar(100) not null,
    sds_port integer not null,
    sds_use_ldaps boolean not null,
    organisation_id integer not null,
    party_key varchar(20) not null,
    asid varchar(20) not null,
    --tls_certificate text not null,
    --tls_private_key text not null,
    --tls_ca_chain text not null

    constraint configuration_spine_singlerowlock_pk primary key (single_row_lock),
    constraint configuration_spine_singlerowlock_ck check (single_row_lock = true),
    constraint configuration_spine_ssphostname_ck check (char_length(trim(ssp_hostname)) > 0),
    constraint configuration_spine_sdshostname_ck check (char_length(trim(sds_hostname)) > 0),
    constraint configuration_spine_sdsport_ck check (sds_port > 0),
    constraint configuration_spine_organisationid_fk foreign key (organisation_id) references application.organisation (organisation_id),
    constraint configuration_spine_partykey_ck_1 check (char_length(trim(party_key)) > 0),
    constraint configuration_spine_partykey_ck_2 check (party_key = upper(party_key)),
    constraint configuration_spine_asid_ck check (char_length(trim(asid)) > 0)
);

create table configuration.spine_message_type
(
    spine_message_type_id smallint not null,
    spine_message_type_name varchar(200) not null,
    interaction_id varchar(200) not null,

    constraint configuration_spinemessagetype_spinemessagetypeid_pk primary key (spine_message_type_id),
    constraint configuration_spinemessagetype_spinemessagetypename_ck check (char_length(trim(spine_message_type_name)) > 0),
    constraint configuration_spinemessagetype_interactionid_ck check (char_length(trim(interaction_id)) > 0)
);

create unique index configuration_spinemessagetype_spinemessagetypename_ix on configuration.spine_message_type (lower(spine_message_type_name));
create unique index configuration_spinemessagetype_interactionid_ix on configuration.spine_message_type (lower(interaction_id));

/*
    create tables - audit
*/
create table audit.user_session
(
    user_session_id serial primary key,
    user_id integer not null,
    start_time timestamp not null,
    end_time timestamp null,

    constraint audit_usersession_usersessionid_pk primary key (user_session_id),
    constraint audit_usersession_userid_fk foreign key (user_id) references application.user (user_id),
    constraint audit_usersession_starttime_endtime_ck check (start_time < end_time)
);

create table audit.slot_search
(
    slot_search_id serial primary key,
    user_id integer not null,
    consumer_ods_code varchar(20) not null,
    provider_ods_code varchar(20) not null,
    searchtime_ms integer not null,
    results_count integer null,
    logged_time timestamp not null,
    was_success boolean not null,
    error_message varchar(1000) null

    constraint audit_usersession_usersessionid_pk primary key (user_session_id),
    constraint audit_usersession_userid_fk foreign key (user_id) references application.user (user_id),
    constraint audit_usersession_searchtimems_ck check (searchtime_ms >= 0),

    constraint audit_slotsearch_wassuccess_errormessage_ck check ((was_success and error_message is null) or ((!was_success) and (error_message is not null)))
);

/*
    create tables - logging
*/
create table logging.log
( 
    log_id serial primary key,
    application varchar(100) null,
    logged timestamp,
    level varchar(100) null,
    message varchar(8000) null,
    logger varchar(8000) null, 
    callsite varchar(8000) null, 
    exception varchar(8000) null

    constraint logging_log_logid_pk primary key (log_id)
);

create table logging.spine_message
( 
    spine_message_id serial primary key,
    spine_message_type_id smallint not null,
    user_session_id integer not null,
    command varchar(8000) not null,
    request_headers text not null,
    request_payload text null
    response_status varchar(100) not null,
    response_headers text null,
    response_payload text null,
    logged_date timestamp not null,
    roundtriptime_ms integer not null

    constraint logging_spinemessage_spinemessageid_pk primary key (error_id),
    constraint logging_spinemessage_spinemessagetypeid_fk foreign key (spine_message_type_id) references configuration.spine_message_type (spine_message_type_id),
    constraint logging_spinemessage_usersessionid_fk foreign key (user_session_id) references audit.user_session (user_session_id),
    constraint logging_spinemessage_roundtriptimems_ck check (roundtriptime_ms > 0)
);

create table logging.web_request (
    web_request_id integer not null,
    user_session_id integer null,
    url varchar(1000) not null,
    referrer_url varchar(1000),
    description varchar(1000) not null,
    ip varchar(255) not null,
    created_date timestamp not null,
    created_by varchar(255) not null,
    server varchar(255) not null,
    response_code integer not null,
    session_id varchar(1000) not null,
    user_agent varchar(1000) not null,

    constraint logging_webrequest_webrequestid_pk primary key (web_request_id),
    constraint logging_webrequest_usersessionid_fk foreign key (user_session_id) references audit.user_session (user_session_id)
);



/*
    insert initial data
*/
insert into configuration.spine_message_type
(
    spine_message_type_id,
    spine_message_type_name,
    interaction_id
)
values
(
    1,
    'Spine Directory Service - LDAP query',
    'urn:nhs:names:services:sds:ldap'
),
(

    2,
    'GP Connect - Read metadata (Appointment Management)',
    'urn:nhs:names:services:gpconnect:fhir:rest:read:metadata-1'
),
(
    3,
    'GP Connect - Search for free slots',
    'urn:nhs:names:services:gpconnect:fhir:rest:search:slot-1'
);

insert into application.organisation_type
(
    organisation_type_id,
    organisation_type_name
)
values
(
    1,
    'National Application Service Provider'
);

insert into application.organisation 
(
    ods_code,
    organisation_type_id,
    organisation_name,
    address_line_1,
    address_line_2,
    locality,
    city,
    county,
    postcode,
    is_gpconnect_consumer,
    is_gpconnect_provider,
    added_date,
    last_sync_date
)
values
(
    'YES',
    1,
    'NATIONAL CARE RECORDS SERVICE SPINE II',
    'PRINCES EXCHANGE',
    '2 PRINCES SQUARE',
    '',
    'LEEDS',
    'WEST YORKSHIRE',
    'LS1 4HY', 
    true, 
    false, 
    now(), 
    now()
);

