SELECT cron.schedule('PURGE logs', '0 23 * * *', 'SELECT logging.purge_logs()');
UPDATE cron.job SET database='GpConnectAppointmentChecker' WHERE jobname='PURGE logs';
SELECT cron.schedule('PURGE search results', '0 23 * * *', 'SELECT application.purge_search_results()');
UPDATE cron.job SET database='GpConnectAppointmentChecker' WHERE jobname='PURGE search results';
