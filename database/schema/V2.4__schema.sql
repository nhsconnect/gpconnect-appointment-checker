alter table configuration.spine add ldaps_tls_version varchar(9) null;

alter table configuration.spine add constraint configuration_spine_ldapstlsversion_ck check (ldaps_tls_version in ('negotiate', '1.2', '1.3'));

update configuration.spine
set ldaps_tls_version = 'negotiate'
where single_row_lock = true
and sds_use_ldaps = true;

alter table configuration.spine add constraint configuration_spine_sdsuseldaps_ldapstlsversion_ck check ((sds_use_ldaps and ldaps_tls_version is not null) or ((not sds_use_ldaps) and ldaps_tls_version is null));
