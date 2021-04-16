alter table configuration.general add column last_access_highlight_threshold_in_days integer null;
update configuration.general set last_access_highlight_threshold_in_days = 30;
alter table configuration.general alter column last_access_highlight_threshold_in_days set not null;