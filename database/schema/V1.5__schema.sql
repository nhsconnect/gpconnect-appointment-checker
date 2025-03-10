-- create user
do
$$
BEGIN
	IF NOT EXISTS (SELECT * FROM pg_roles WHERE rolname = 'app_user') THEN
		CREATE USER app_user;
	END IF;
END
$$;

alter user app_user valid until 'infinity';

-- revoke default public permissions
revoke all on schema public from public;

-- grant schema application usage
grant usage on schema application to app_user;
grant select, insert, update on all tables in schema application to app_user;
grant select, update on all sequences in schema application to app_user;
grant execute on all functions in schema application to app_user;

-- grant schema audit usage
grant usage on schema audit to app_user;
grant select, insert, update on all tables in schema audit to app_user;
grant select, update on all sequences in schema audit to app_user;
grant execute on all functions in schema audit to app_user;

-- grant schema configuration usage
grant usage on schema configuration to app_user;
grant select, insert, update on all tables in schema configuration to app_user;
grant select, update on all sequences in schema configuration to app_user;
grant execute on all functions in schema configuration to app_user;

-- grant schema logging usage
grant usage on schema logging to app_user;
grant select, insert, update, delete on all tables in schema logging to app_user;
grant select, update on all sequences in schema logging to app_user;
grant execute on all functions in schema logging to app_user;
