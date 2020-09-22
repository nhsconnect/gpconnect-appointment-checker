drop table logging.spine_message;

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
    constraint logging_spinemessage_usersessionid_fk foreign key (user_session_id) references application.user_session (user_session_id),
    constraint logging_spinemessage_roundtriptimems_ck check (roundtriptime_ms > 0)
);

create table configuration.sds_query
(
    query_name varchar(100) not null,
    search_base varchar(200) not null,
    query_text varchar(1000) not null,
    
    constraint configuration_sdsquery_queryname_pk primary key (query_name),
    constraint configuration_sdsquery_queryname_ck check (char_length(trim(query_name)) > 0),
    constraint configuration_sdsquery_searchbase_ck check (char_length(trim(search_base)) > 0),
    constraint configuration_sdsquery_querytext_ck check (char_length(trim(query_text)) > 0)
);

create unique index configuration_sdsquery_queryname_ix on configuration.sds_query (lower(query_name));

create table configuration.sso
(
    single_row_lock boolean not null,
    client_id varchar(200) not null,
    client_secret varchar(1000) not null,
    callback_path varchar(1000) not null,
    auth_scheme varchar(100) not null,
    challenge_scheme varchar(100) not null,
    auth_endpoint varchar(1000) not null,
    token_endpoint varchar(1000) not null,
    
    constraint configuration_sso_singlerowlock_pk primary key (single_row_lock),
    constraint configuration_sso_singlerowlock_ck check (single_row_lock = true),
    constraint configuration_sso_clientid_ck check (char_length(trim(client_id)) > 0),
    constraint configuration_sso_clientsecret_ck check (char_length(trim(client_secret)) > 0),
    constraint configuration_sso_callbackpath_ck check (char_length(trim(callback_path)) > 0),
    constraint configuration_sso_authscheme_ck check (char_length(trim(auth_scheme)) > 0),
    constraint configuration_sso_challengescheme_ck check (char_length(trim(challenge_scheme)) > 0),
    constraint configuration_sso_authendpoint_ck check (char_length(trim(auth_endpoint)) > 0),
    constraint configuration_sso_tokenendpoint_ck check (char_length(trim(token_endpoint)) > 0)    
);

insert into configuration.sds_query
(
    query_name,
    search_base,
    query_text
)
values
(
    'GetOrganisationDetailsByOdsCode',
    'ou=organisations, o=nhs',
    '(uniqueidentifier={odsCode})'
),
(
    'OrganisationHasAppointmentsConsumerSystemByOdsCode',
    'ou=services, o=nhs',
    '(&(nhsIDCode={odsCode})(objectClass=nhsAs)(nhsAsSvcIA=urn:nhs:names:services:gpconnect:structured:fhir:rest:read:metadata-1))'
),
(
    'OrganisationHasAppointmentsProviderSystemByOdsCode',
    'ou=services, o=nhs',
    '(&(nhsIDCode={odsCode})(objectClass=nhsMhs)(nhsMhsSvcIA=urn:nhs:names:services:gpconnect:structured:fhir:rest:read:metadata-1))'
),
(
    'GetGpProviderEndpointAndPartyKeyByOdsCode',
    'ou=services, o=nhs',
    '(&(nhsIDCode={odsCode})(objectClass=nhsMhs)(nhsMhsSvcIA=urn:nhs:names:services:gpconnect:structured:fhir:rest:read:metadata-1))'
),
(
    'GetGpProviderAsIdByOdsCodeAndPartyKey',
    'ou=services, o=nhs',
    '(&(nhsIDCode={odsCode})(objectClass=nhsAs)(nhsMhsPartyKey={partyKey}))'
);

insert into configuration.sso
(
    single_row_lock,
    client_id,
    client_secret,
    callback_path,
    auth_scheme,
    challenge_scheme,
    auth_endpoint,
    token_endpoint
)
values
(
    true,
    'client_id',
    'client_secret',
    'https://callback_path',
    'auth_scheme',
    'GpConnectAppointmentChecker',
    'https://auth_endpoint',
    'https://token_endpoint'
);