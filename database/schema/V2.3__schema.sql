alter table configuration.general add column max_number_provider_codes_search smallint null;
alter table configuration.general add column max_number_consumer_codes_search smallint null;

update configuration.general set max_number_provider_codes_search = 20;
update configuration.general set max_number_consumer_codes_search = 20;

alter table configuration.general alter column max_number_provider_codes_search set not null;
alter table configuration.general alter column max_number_consumer_codes_search set not null;

alter table configuration.general add constraint configuration_general_maxnumberprovidercodes_ck check (max_number_provider_codes_search > 0);
alter table configuration.general add constraint configuration_general_maxnumberconsumercodes_ck check (max_number_consumer_codes_search > 0);