alter table configuration.spine add sds_tls_version varchar(9) null;

alter table configuration.spine add constraint configuration_spine_sdstlsversion_ck check (sds_tls_version in ('negotiate', '1.2', '1.3'));

update configuration.spine
set sds_tls_version = 'negotiate'
where single_row_lock = true
and sds_use_ldaps = true;

alter table configuration.spine add constraint configuration_spine_sdsuseldaps_sdstlsversion_ck check ((sds_use_ldaps and sds_tls_version is not null) or ((not sds_use_ldaps) and sds_tls_version is null));
