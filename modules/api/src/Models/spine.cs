using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.api.Models;

public partial class spine
{
    public bool single_row_lock { get; set; }

    public bool use_ssp { get; set; }

    public string? ssp_hostname { get; set; }

    public string sds_hostname { get; set; } = null!;

    public int sds_port { get; set; }

    public bool sds_use_ldaps { get; set; }

    public int organisation_id { get; set; }

    public string party_key { get; set; } = null!;

    public string asid { get; set; } = null!;

    public int timeout_seconds { get; set; }

    public string? client_cert { get; set; }

    public string? client_private_key { get; set; }

    public string? server_ca_certchain { get; set; }

    public bool sds_use_mutualauth { get; set; }

    public string spine_fqdn { get; set; } = null!;

    public string? sds_tls_version { get; set; }

    public bool sds_use_fhir_api { get; set; }

    public string spine_fhir_api_directory_services_fqdn { get; set; } = null!;

    public string spine_fhir_api_systems_register_fqdn { get; set; } = null!;

    public string? spine_fhir_api_key { get; set; }

    public virtual organisation organisation { get; set; } = null!;
}
