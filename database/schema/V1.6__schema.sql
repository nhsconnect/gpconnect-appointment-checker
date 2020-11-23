alter table configuration.spine add column sds_use_mutualauth boolean null;
update configuration.spine set sds_use_mutualauth = false;
alter table configuration.spine alter column sds_use_mutualauth set not null;
alter table configuration.spine add constraint configuration_spine_sdsuseldaps_sdsusemutualauth_ck check (sds_use_ldaps or (not sds_use_mutualauth));
