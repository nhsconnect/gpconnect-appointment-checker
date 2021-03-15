delete from logging.web_request
where user_agent = 'ELB-HealthChecker/2.0'
and url = '/';