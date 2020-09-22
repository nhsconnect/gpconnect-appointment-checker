alter table configuration.sso add column challenge_scheme varchar(100);
update configuration.sso set challenge_scheme='GpConnectAppointmentChecker';
alter table configuration.sso alter column challenge_scheme set not null;