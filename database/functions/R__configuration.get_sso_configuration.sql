create or replace function configuration.get_sso_configuration
(
)
returns table
(
    client_id varchar(200),
    client_secret varchar(1000),
    callback_path varchar(1000),
    auth_scheme varchar(100),
    auth_endpoint varchar(1000),
    token_endpoint varchar(1000)
)
as $$
begin

	return query
	select
	    s.client_id,
	    s.client_secret,
	    s.callback_path,
	    s.auth_scheme,
	    s.auth_endpoint,
	    s.token_endpoint
   	from configuration.sso s;
	
end;
$$ language plpgsql;

