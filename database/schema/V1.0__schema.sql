/*
    Schema V1.0: Initial schema
*/


/* 
    create schemas
*/
create schema application;
create schema configuration;
create schema logging;
create schema audit;


/*
    create tables - application
*/
create table application.organisation_type
(
    organisation_type_id smallint not null,
    organisation_type_name varchar(200) not null,

    constraint application_organisationtype_organisationtypeid_pk primary key (organisation_type_id),
    constraint application_organisationtype_organisationtypename_ck check (char_length(trim(organisation_type_name)) > 0)
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
    postcode varchar(10) not null,
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
    display_name varchar(200) not null,
    organisation_id integer not null,
    is_authorised boolean not null,
    added_date timestamp not null,
    authorised_date timestamp null,
    last_logon_date timestamp null,

    constraint application_user_userid_pk primary key (user_id),
    constraint application_user_emailaddress_ck check (char_length(trim(email_address)) > 0),
    constraint application_user_displayname_ck check (char_length(trim(display_name)) > 0),
    constraint application_user_organisationid_pk foreign key (organisation_id) references application.organisation (organisation_id),
    constraint application_user_isauthorised_authoriseddate_ck check ((is_authorised and authorised_date is not null) or ((not is_authorised) and authorised_date is null))
);

create unique index application_user_emailaddress_ix on application.user (lower(email_address));

create table application.user_session
(
    user_session_id serial not null,
    user_id integer not null,
    start_time timestamp not null,
    end_time timestamp null,

    constraint application_usersession_usersessionid_pk primary key (user_session_id),
    constraint application_usersession_userid_fk foreign key (user_id) references application.user (user_id),
    constraint application_usersession_starttime_endtime_ck check (start_time <= end_time)
);


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
    constraint configuration_general_singlerowlock_ck check (single_row_lock = true),
    constraint configuration_general_productname_ck check (char_length(trim(product_name)) > 0),
    constraint configuration_general_productversion_ck check (char_length(trim(product_version)) > 0),
    constraint configuration_general_maxnumweekssearch_ck check (max_num_weeks_search > 0)
);

create table configuration.spine
(
    single_row_lock boolean,
    use_ssp boolean not null,
    ssp_hostname varchar(100) null,
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
    constraint configuration_spine_ssphostname_ck check ((char_length(trim(coalesce(ssp_hostname, ''))) > 0) or (not use_ssp)),
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
create table audit.entry_type 
(
    entry_type_id smallint not null,
    entry_description varchar(200) not null,
    item1_description varchar(100) null,
    item2_description varchar(100) null,
    item3_description varchar(100) null,

    constraint audit_entrytype_entrytypeid_pk primary key (entry_type_id),
    constraint audit_entrytype_entrydescription_ck check (char_length(trim(entry_description)) > 0)
);

create unique index audit_entrytype_entrydescription_ix on audit.entry_type (lower(entry_description));

create table audit.entry 
(
    entry_id serial not null,
    user_id integer null,
    user_session_id integer null,
    entry_type_id smallint not null,
    item1 varchar(100) null,
    item2 varchar(100) null,
    item3 varchar(100) null,
    details varchar(1000) null,
    entry_elapsed_ms integer null,
    entry_date timestamp not null,

    constraint audit_entry_entryid_pk primary key (entry_id),
    constraint audit_entry_userid_fk foreign key (user_id) references application.user (user_id),
    constraint audit_entry_usersessionid_fk foreign key (user_session_id) references application.user_session (user_session_id),
    constraint audit_entry_entrytypeid_fk foreign key (entry_type_id) references audit.entry_type (entry_type_id)
);


/*
    create tables - logging
*/
create table logging.error_log
( 
    id serial not null,
    application character varying(100) null,
    logged text,
    level character varying(100) null,
    message character varying(8000) null,
    logger character varying(8000) null, 
    callsite character varying(8000) null, 
    exception character varying(8000) null,

    constraint logging_errorlog_logid_pk primary key (id)
);

create table logging.spine_message
( 
    spine_message_id serial not null,
    spine_message_type_id smallint not null,
    user_session_id integer null,
    command varchar(8000) null,
    request_headers text null,
    request_payload text not null,
    response_status varchar(100) null,
    response_headers text null,
    response_payload text not null,
    logged_date timestamp not null,
    roundtriptime_ms bigint not null,

    constraint logging_spinemessage_spinemessageid_pk primary key (spine_message_id),
    constraint logging_spinemessage_spinemessagetypeid_fk foreign key (spine_message_type_id) references configuration.spine_message_type (spine_message_type_id),
    constraint logging_spinemessage_roundtriptimems_ck check (roundtriptime_ms > 0)
);

create table logging.web_request 
(
    web_request_id serial not null,
    user_id integer null,
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
    constraint logging_webrequest_userid_fk foreign key (user_id) references application.user (user_id),
    constraint logging_webrequest_usersessionid_fk foreign key (user_session_id) references application.user_session (user_session_id)
);


/*
    insert initial data
*/
insert into configuration.general
(
    single_row_lock,
    product_name,
    product_version,
    max_num_weeks_search
)
values
(
    true,
    'GP Connect Appointment Checker',
    '1.0.0',
    12
);

insert into application.organisation_type
(
    organisation_type_id,
    organisation_type_name
)
values
(
    1,
    'Executive Agency Programme'
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
    'X26010',
    1,
    'SYSTEMS & SERVICE DELIVERY',
    'HEXAGON HOUSE',
    'PYNES HILL',
    'RYDON LANE',
    'EXETER',
    'DEVON',
    'EX2 5SE', 
    true, 
    false, 
    now(), 
    now()
);

insert into configuration.spine
(
    single_row_lock,
    use_ssp,
    ssp_hostname,
    sds_hostname,
    sds_port,
    sds_use_ldaps,
    organisation_id,
    party_key,
    asid
)
values
(
    true,
    false,
    null,
    'orange.testlab.nhs.uk',
    636,
    true,
    1,
    'ABC-123456',
    '100000000001'
);

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

insert into audit.entry_type
(
    entry_type_id,
    entry_description,
    item1_description,
    item2_description,
    item3_description
)
values
(
    1,
    'User logon success',
    null,
    null,
    null
),
(
    2,
    'User logon failed',
    'reason',
    null,
    null
),
(
    3,
    'User logoff',
    null,
    null,
    null
),
(
    4,
    'User ran slot search',
    'consumer ods code',
    'provider ods code',
    'slot result count, or error message'
),
(
    5,
    'User display name changed',
    'old display_name',
    'new display_name',
    null
),
(
    6,
    'User organisation changed',
    'old organisation_id',
    'new organisation_id',
    null
),
(
    7,
    'Organisation name changed',
    'old organisation_name',
    'new organisation_name',
    null
),
(
    8,
    'Organisation type changed',
    'old organisation_type_name',
    'new organisation_type_name',
    null
),
(
    9,
    'Organisation address changed',
    'old organisation address',
    'new organisation address',
    null
),
(
    10,
    'Organisation GP Connect consumer status changed',
    'old is_gpconnect_consumer',
    'new is_gpconnect_consumer',
    null
),
(
    11,
    'Organisation GP Connect provider status changed',
    'old is_gpconnect_provider',
    'new is_gpconnect_provider',
    null
);
