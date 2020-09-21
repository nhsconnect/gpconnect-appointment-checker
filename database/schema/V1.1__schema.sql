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
    constraint logging_spinemessage_roundtriptimems_ck check (roundtriptime_ms > 0)
);
