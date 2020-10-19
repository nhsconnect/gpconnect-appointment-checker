select
    product_name,
    product_version,
    max_num_weeks_search,
    log_retention_period
from configuration.get_general_configuration
(
);
