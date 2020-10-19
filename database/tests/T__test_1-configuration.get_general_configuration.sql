select
    product_name,
    product_version,
    max_num_weeks_search,
    audit_retention_period,
    log_retention_period
from configuration.get_general_configuration
(
);
