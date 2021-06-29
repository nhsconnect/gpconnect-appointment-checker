drop function if exists configuration.get_email_configuration;

create function configuration.get_email_configuration
(
)
returns table
(
    sender_address varchar(100),
    host_name varchar(100),
    port smallint,
    encryption varchar(10),
    user_name varchar(100),
    password varchar(100),
    default_subject varchar(100)
)
as $$
begin
	return query
	select
		e.sender_address,
		e.host_name,
		e.port,
		e.encryption,
		e.user_name,
		e.password,
		e.default_subject
	from
		configuration.email e;
end;
$$ language plpgsql;
