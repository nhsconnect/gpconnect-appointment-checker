-- add new spine fqdn column
alter table configuration.spine add column spine_fqdn varchar null;
alter table configuration.spine add constraint configuration_spine_spinefqdn_ck check (char_length(trim(spine_fqdn)) > 0);
update configuration.spine set spine_fqdn = 'ldap.nis1.national.ncrs.nhs.uk';
alter table configuration.spine alter column spine_fqdn set not null;