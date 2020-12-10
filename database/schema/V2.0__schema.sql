-- create user
create user readonly_user;
alter user readonly_user valid until 'infinity';

-- revoke default public permissions
revoke all on schema public from public;

-- grant schema public usage
grant usage on schema public to readonly_user;
grant select on all tables in schema public to readonly_user;

-- grant schema application usage
grant usage on schema application to readonly_user;
grant select on all tables in schema application to readonly_user;

-- grant schema audit usage
grant usage on schema audit to readonly_user;
grant select on all tables in schema audit to readonly_user;

-- grant schema configuration usage
grant usage on schema configuration to readonly_user;
grant select on all tables in schema configuration to readonly_user;

-- grant schema logging usage
grant usage on schema logging to readonly_user;
grant select on all tables in schema logging to readonly_user;
