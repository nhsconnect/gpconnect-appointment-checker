create table configuration.organisation_type
(
    organisation_type_id smallint not null,
    organisation_type_code varchar(50) not null,
    organisation_type_description varchar(100) not null,

    constraint configuration_organisationtype_organisationtypeid_pk primary key (organisation_type_id),
    constraint configuration_organisationtype_organisationtypecode_ck check (char_length(trim(organisation_type_code)) > 0),
    constraint configuration_organisationtype_organisationtypedescription_ck check (char_length(trim(organisation_type_description)) > 0)
);

create unique index configuration_organisationtype_organisationtypecode_ix on configuration.organisation_type (lower(organisation_type_code));

insert into configuration.organisation_type
(
    organisation_type_id,
    organisation_type_code,
    organisation_type_description
)
values
(
    1,
    'gp-practice',
    'GP Practice'
),
(

    2,
    'urgent-care',
    'Urgent Care'
);

grant update, insert, select on table configuration.organisation_type to app_user;
grant execute on all functions in schema configuration to app_user;
