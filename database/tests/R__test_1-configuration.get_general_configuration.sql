select
    product_name,
    product_version,
    max_num_weeks_search,
    log_retention_days
from configuration.get_general_configuration
(
);
