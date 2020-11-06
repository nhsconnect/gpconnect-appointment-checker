alter table configuration.spine add column client_cert varchar(8000) null;
alter table configuration.spine add constraint configuration_spine_clientcert_ck check (char_length(trim(client_cert)) > 0);

alter table configuration.spine add column client_private_key varchar(8000) null;
alter table configuration.spine add constraint configuration_spine_clientprivatekey_ck check (char_length(trim(client_private_key)) > 0);

alter table configuration.spine add column server_ca_certchain varchar(8000) null;
alter table configuration.spine add constraint configuration_spine_servercacertchain_ck check (char_length(trim(server_ca_certchain)) > 0);
