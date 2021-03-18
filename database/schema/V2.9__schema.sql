create schema reporting;

grant usage on schema reporting to app_user;
grant select, insert, update on all tables in schema reporting to app_user;
grant select, update on all sequences in schema reporting to app_user;
grant execute on all functions in schema reporting to app_user;

grant usage on schema reporting to readonly_user;
grant select on all tables in schema reporting to readonly_user;