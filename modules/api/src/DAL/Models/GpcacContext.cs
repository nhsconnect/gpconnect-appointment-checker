using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace gpconnect_appointment_checker.api.DAL.Models;

public partial class GpcacContext : DbContext
{
    public GpcacContext()
    {
    }

    public GpcacContext(DbContextOptions<GpcacContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Aggregatedcounter> Aggregatedcounters { get; set; }

    public virtual DbSet<Counter> Counters { get; set; }

    public virtual DbSet<Email> Emails { get; set; }

    public virtual DbSet<EmailTemplate> EmailTemplates { get; set; }

    public virtual DbSet<Entry> Entries { get; set; }

    public virtual DbSet<EntryType> EntryTypes { get; set; }

    public virtual DbSet<ErrorLog> ErrorLogs { get; set; }

    public virtual DbSet<FhirApiQuery> FhirApiQueries { get; set; }

    public virtual DbSet<FlywaySchemaHistory> FlywaySchemaHistories { get; set; }

    public virtual DbSet<General> Generals { get; set; }

    public virtual DbSet<Hash> Hashes { get; set; }

    public virtual DbSet<Job> Jobs { get; set; }

    public virtual DbSet<Jobparameter> Jobparameters { get; set; }

    public virtual DbSet<Jobqueue> Jobqueues { get; set; }

    public virtual DbSet<List> Lists { get; set; }

    public virtual DbSet<List1> Lists1 { get; set; }

    public virtual DbSet<Lock> Locks { get; set; }

    public virtual DbSet<Organisation> Organisations { get; set; }

    public virtual DbSet<OrganisationType> OrganisationTypes { get; set; }

    public virtual DbSet<OrganisationType1> OrganisationTypes1 { get; set; }

    public virtual DbSet<Schema> Schemas { get; set; }

    public virtual DbSet<SdsQuery> SdsQueries { get; set; }

    public virtual DbSet<SearchExport> SearchExports { get; set; }

    public virtual DbSet<SearchGroup> SearchGroups { get; set; }

    public virtual DbSet<SearchResult> SearchResults { get; set; }

    public virtual DbSet<Server> Servers { get; set; }

    public virtual DbSet<Set> Sets { get; set; }

    public virtual DbSet<Spine> Spines { get; set; }

    public virtual DbSet<SpineMessage> SpineMessages { get; set; }

    public virtual DbSet<SpineMessageType> SpineMessageTypes { get; set; }

    public virtual DbSet<Sso> Ssos { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<Transient> Transients { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAccountStatus> UserAccountStatuses { get; set; }

    public virtual DbSet<UserSession> UserSessions { get; set; }

    public virtual DbSet<WebRequest> WebRequests { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=gpcac;Username=postgres;Password='@nswerP123'");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Aggregatedcounter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("aggregatedcounter_pkey");

            entity.ToTable("aggregatedcounter", "hangfire");

            entity.HasIndex(e => e.Key, "aggregatedcounter_key_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Expireat).HasColumnName("expireat");
            entity.Property(e => e.Key).HasColumnName("key");
            entity.Property(e => e.Value).HasColumnName("value");
        });

        modelBuilder.Entity<Counter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("counter_pkey");

            entity.ToTable("counter", "hangfire");

            entity.HasIndex(e => e.Expireat, "ix_hangfire_counter_expireat");

            entity.HasIndex(e => e.Key, "ix_hangfire_counter_key");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Expireat).HasColumnName("expireat");
            entity.Property(e => e.Key).HasColumnName("key");
            entity.Property(e => e.Value).HasColumnName("value");
        });

        modelBuilder.Entity<Email>(entity =>
        {
            entity.HasKey(e => e.SingleRowLock).HasName("configuration_email_singlerowlock_pk");

            entity.ToTable("email", "configuration");

            entity.Property(e => e.SingleRowLock).HasColumnName("single_row_lock");
            entity.Property(e => e.DefaultSubject)
                .HasMaxLength(100)
                .HasColumnName("default_subject");
            entity.Property(e => e.Encryption)
                .HasMaxLength(10)
                .HasColumnName("encryption");
            entity.Property(e => e.HostName)
                .HasMaxLength(100)
                .HasColumnName("host_name");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasColumnName("password");
            entity.Property(e => e.Port).HasColumnName("port");
            entity.Property(e => e.SenderAddress)
                .HasMaxLength(100)
                .HasColumnName("sender_address");
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .HasColumnName("user_name");
        });

        modelBuilder.Entity<EmailTemplate>(entity =>
        {
            entity.HasKey(e => e.EmailTemplateId).HasName("application_emailtemplate_emailtemplateid_pk");

            entity.ToTable("email_template", "application");

            entity.Property(e => e.EmailTemplateId)
                .ValueGeneratedNever()
                .HasColumnName("email_template_id");
            entity.Property(e => e.Body).HasColumnName("body");
            entity.Property(e => e.Subject)
                .HasMaxLength(100)
                .HasColumnName("subject");
        });

        modelBuilder.Entity<Entry>(entity =>
        {
            entity.HasKey(e => e.EntryId).HasName("audit_entry_entryid_pk");

            entity.ToTable("entry", "audit");

            entity.Property(e => e.EntryId).HasColumnName("entry_id");
            entity.Property(e => e.AdminUserId).HasColumnName("admin_user_id");
            entity.Property(e => e.Details)
                .HasMaxLength(1000)
                .HasColumnName("details");
            entity.Property(e => e.EntryDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("entry_date");
            entity.Property(e => e.EntryElapsedMs).HasColumnName("entry_elapsed_ms");
            entity.Property(e => e.EntryTypeId).HasColumnName("entry_type_id");
            entity.Property(e => e.Item1)
                .HasMaxLength(1000)
                .HasColumnName("item1");
            entity.Property(e => e.Item2)
                .HasMaxLength(1000)
                .HasColumnName("item2");
            entity.Property(e => e.Item3)
                .HasMaxLength(1000)
                .HasColumnName("item3");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.UserSessionId).HasColumnName("user_session_id");

            entity.HasOne(d => d.AdminUser).WithMany(p => p.EntryAdminUsers)
                .HasForeignKey(d => d.AdminUserId)
                .HasConstraintName("audit_entry_adminuserid_fk");

            entity.HasOne(d => d.EntryType).WithMany(p => p.Entries)
                .HasForeignKey(d => d.EntryTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("audit_entry_entrytypeid_fk");

            entity.HasOne(d => d.User).WithMany(p => p.EntryUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("audit_entry_userid_fk");

            entity.HasOne(d => d.UserSession).WithMany(p => p.Entries)
                .HasForeignKey(d => d.UserSessionId)
                .HasConstraintName("audit_entry_usersessionid_fk");
        });

        modelBuilder.Entity<EntryType>(entity =>
        {
            entity.HasKey(e => e.EntryTypeId).HasName("audit_entrytype_entrytypeid_pk");

            entity.ToTable("entry_type", "audit");

            entity.Property(e => e.EntryTypeId)
                .ValueGeneratedNever()
                .HasColumnName("entry_type_id");
            entity.Property(e => e.DetailsDescription)
                .HasMaxLength(100)
                .HasColumnName("details_description");
            entity.Property(e => e.EntryDescription)
                .HasMaxLength(200)
                .HasColumnName("entry_description");
            entity.Property(e => e.Item1Description)
                .HasMaxLength(100)
                .HasColumnName("item1_description");
            entity.Property(e => e.Item2Description)
                .HasMaxLength(100)
                .HasColumnName("item2_description");
            entity.Property(e => e.Item3Description)
                .HasMaxLength(100)
                .HasColumnName("item3_description");
        });

        modelBuilder.Entity<ErrorLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("logging_errorlog_logid_pk");

            entity.ToTable("error_log", "logging");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Application)
                .HasMaxLength(100)
                .HasColumnName("application");
            entity.Property(e => e.Callsite)
                .HasMaxLength(8000)
                .HasColumnName("callsite");
            entity.Property(e => e.Exception)
                .HasMaxLength(8000)
                .HasColumnName("exception");
            entity.Property(e => e.Level)
                .HasMaxLength(100)
                .HasColumnName("level");
            entity.Property(e => e.Logged)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("logged");
            entity.Property(e => e.Logger)
                .HasMaxLength(8000)
                .HasColumnName("logger");
            entity.Property(e => e.Message)
                .HasMaxLength(8000)
                .HasColumnName("message");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.UserSessionId).HasColumnName("user_session_id");
        });

        modelBuilder.Entity<FhirApiQuery>(entity =>
        {
            entity.HasKey(e => e.QueryName).HasName("configuration_fhirapiquery_queryname_pk");

            entity.ToTable("fhir_api_query", "configuration");

            entity.Property(e => e.QueryName)
                .HasMaxLength(100)
                .HasColumnName("query_name");
            entity.Property(e => e.QueryText)
                .HasMaxLength(1000)
                .HasColumnName("query_text");
        });

        modelBuilder.Entity<FlywaySchemaHistory>(entity =>
        {
            entity.HasKey(e => e.InstalledRank).HasName("flyway_schema_history_pk");

            entity.ToTable("flyway_schema_history");

            entity.HasIndex(e => e.Success, "flyway_schema_history_s_idx");

            entity.Property(e => e.InstalledRank)
                .ValueGeneratedNever()
                .HasColumnName("installed_rank");
            entity.Property(e => e.Checksum).HasColumnName("checksum");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .HasColumnName("description");
            entity.Property(e => e.ExecutionTime).HasColumnName("execution_time");
            entity.Property(e => e.InstalledBy)
                .HasMaxLength(100)
                .HasColumnName("installed_by");
            entity.Property(e => e.InstalledOn)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("installed_on");
            entity.Property(e => e.Script)
                .HasMaxLength(1000)
                .HasColumnName("script");
            entity.Property(e => e.Success).HasColumnName("success");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");
            entity.Property(e => e.Version)
                .HasMaxLength(50)
                .HasColumnName("version");
        });

        modelBuilder.Entity<General>(entity =>
        {
            entity.HasKey(e => e.SingleRowLock).HasName("configuration_general_singlerowlock_pk");

            entity.ToTable("general", "configuration");

            entity.Property(e => e.SingleRowLock).HasColumnName("single_row_lock");
            entity.Property(e => e.GetAccessEmailAddress)
                .HasColumnType("character varying")
                .HasColumnName("get_access_email_address");
            entity.Property(e => e.LastAccessHighlightThresholdInDays).HasColumnName("last_access_highlight_threshold_in_days");
            entity.Property(e => e.LogRetentionDays).HasColumnName("log_retention_days");
            entity.Property(e => e.MaxNumWeeksSearch).HasColumnName("max_num_weeks_search");
            entity.Property(e => e.MaxNumberConsumerCodesSearch).HasColumnName("max_number_consumer_codes_search");
            entity.Property(e => e.MaxNumberProviderCodesSearch).HasColumnName("max_number_provider_codes_search");
            entity.Property(e => e.ProductName)
                .HasMaxLength(100)
                .HasColumnName("product_name");
            entity.Property(e => e.ProductVersion)
                .HasMaxLength(100)
                .HasColumnName("product_version");
        });

        modelBuilder.Entity<Hash>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("hash_pkey");

            entity.ToTable("hash", "hangfire");

            entity.HasIndex(e => new { e.Key, e.Field }, "hash_key_field_key").IsUnique();

            entity.HasIndex(e => e.Expireat, "ix_hangfire_hash_expireat");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Expireat).HasColumnName("expireat");
            entity.Property(e => e.Field).HasColumnName("field");
            entity.Property(e => e.Key).HasColumnName("key");
            entity.Property(e => e.Updatecount)
                .HasDefaultValue(0)
                .HasColumnName("updatecount");
            entity.Property(e => e.Value).HasColumnName("value");
        });

        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("job_pkey");

            entity.ToTable("job", "hangfire");

            entity.HasIndex(e => e.Expireat, "ix_hangfire_job_expireat");

            entity.HasIndex(e => e.Statename, "ix_hangfire_job_statename");

            entity.HasIndex(e => e.Statename, "ix_hangfire_job_statename_is_not_null").HasFilter("(statename IS NOT NULL)");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Arguments)
                .HasColumnType("jsonb")
                .HasColumnName("arguments");
            entity.Property(e => e.Createdat).HasColumnName("createdat");
            entity.Property(e => e.Expireat).HasColumnName("expireat");
            entity.Property(e => e.Invocationdata)
                .HasColumnType("jsonb")
                .HasColumnName("invocationdata");
            entity.Property(e => e.Stateid).HasColumnName("stateid");
            entity.Property(e => e.Statename).HasColumnName("statename");
            entity.Property(e => e.Updatecount)
                .HasDefaultValue(0)
                .HasColumnName("updatecount");
        });

        modelBuilder.Entity<Jobparameter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("jobparameter_pkey");

            entity.ToTable("jobparameter", "hangfire");

            entity.HasIndex(e => new { e.Jobid, e.Name }, "ix_hangfire_jobparameter_jobidandname");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Jobid).HasColumnName("jobid");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Updatecount)
                .HasDefaultValue(0)
                .HasColumnName("updatecount");
            entity.Property(e => e.Value).HasColumnName("value");

            entity.HasOne(d => d.Job).WithMany(p => p.Jobparameters)
                .HasForeignKey(d => d.Jobid)
                .HasConstraintName("jobparameter_jobid_fkey");
        });

        modelBuilder.Entity<Jobqueue>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("jobqueue_pkey");

            entity.ToTable("jobqueue", "hangfire");

            entity.HasIndex(e => new { e.Fetchedat, e.Queue, e.Jobid }, "ix_hangfire_jobqueue_fetchedat_queue_jobid").HasNullSortOrder(new[] { NullSortOrder.NullsFirst, NullSortOrder.NullsLast, NullSortOrder.NullsLast });

            entity.HasIndex(e => new { e.Jobid, e.Queue }, "ix_hangfire_jobqueue_jobidandqueue");

            entity.HasIndex(e => new { e.Queue, e.Fetchedat }, "ix_hangfire_jobqueue_queueandfetchedat");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Fetchedat).HasColumnName("fetchedat");
            entity.Property(e => e.Jobid).HasColumnName("jobid");
            entity.Property(e => e.Queue).HasColumnName("queue");
            entity.Property(e => e.Updatecount)
                .HasDefaultValue(0)
                .HasColumnName("updatecount");
        });

        modelBuilder.Entity<List>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("list", "reporting");

            entity.Property(e => e.FunctionName)
                .HasMaxLength(100)
                .HasColumnName("function_name");
            entity.Property(e => e.Interaction)
                .HasColumnType("json")
                .HasColumnName("interaction");
            entity.Property(e => e.ReportId)
                .HasMaxLength(100)
                .HasColumnName("report_id");
            entity.Property(e => e.ReportName)
                .HasMaxLength(100)
                .HasColumnName("report_name");
            entity.Property(e => e.Workflow)
                .HasColumnType("json")
                .HasColumnName("workflow");
        });

        modelBuilder.Entity<List1>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("list_pkey");

            entity.ToTable("list", "hangfire");

            entity.HasIndex(e => e.Expireat, "ix_hangfire_list_expireat");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Expireat).HasColumnName("expireat");
            entity.Property(e => e.Key).HasColumnName("key");
            entity.Property(e => e.Updatecount)
                .HasDefaultValue(0)
                .HasColumnName("updatecount");
            entity.Property(e => e.Value).HasColumnName("value");
        });

        modelBuilder.Entity<Lock>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("lock", "hangfire");

            entity.HasIndex(e => e.Resource, "lock_resource_key").IsUnique();

            entity.Property(e => e.Acquired).HasColumnName("acquired");
            entity.Property(e => e.Resource).HasColumnName("resource");
            entity.Property(e => e.Updatecount)
                .HasDefaultValue(0)
                .HasColumnName("updatecount");
        });

        modelBuilder.Entity<Organisation>(entity =>
        {
            entity.HasKey(e => e.OrganisationId).HasName("application_organisation_organisationid_pk");

            entity.ToTable("organisation", "application");

            entity.HasIndex(e => e.OdsCode, "application_organisation_odscode_uq").IsUnique();

            entity.Property(e => e.OrganisationId).HasColumnName("organisation_id");
            entity.Property(e => e.AddedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("added_date");
            entity.Property(e => e.AddressLine1)
                .HasMaxLength(100)
                .HasColumnName("address_line_1");
            entity.Property(e => e.AddressLine2)
                .HasMaxLength(100)
                .HasColumnName("address_line_2");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.County)
                .HasMaxLength(100)
                .HasColumnName("county");
            entity.Property(e => e.LastSyncDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_sync_date");
            entity.Property(e => e.Locality)
                .HasMaxLength(100)
                .HasColumnName("locality");
            entity.Property(e => e.OdsCode)
                .HasMaxLength(10)
                .HasColumnName("ods_code");
            entity.Property(e => e.OrganisationName)
                .HasMaxLength(100)
                .HasColumnName("organisation_name");
            entity.Property(e => e.OrganisationTypeId).HasColumnName("organisation_type_id");
            entity.Property(e => e.Postcode)
                .HasMaxLength(10)
                .HasColumnName("postcode");

            entity.HasOne(d => d.OrganisationType).WithMany(p => p.Organisations)
                .HasForeignKey(d => d.OrganisationTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("application_organisation_organisationtypeid_fk");
        });

        modelBuilder.Entity<OrganisationType>(entity =>
        {
            entity.HasKey(e => e.OrganisationTypeId).HasName("application_organisationtype_organisationtypeid_pk");

            entity.ToTable("organisation_type", "application");

            entity.Property(e => e.OrganisationTypeId)
                .ValueGeneratedNever()
                .HasColumnName("organisation_type_id");
            entity.Property(e => e.OrganisationTypeName)
                .HasMaxLength(200)
                .HasColumnName("organisation_type_name");
        });

        modelBuilder.Entity<OrganisationType1>(entity =>
        {
            entity.HasKey(e => e.OrganisationTypeId).HasName("configuration_organisationtype_organisationtypeid_pk");

            entity.ToTable("organisation_type", "configuration");

            entity.Property(e => e.OrganisationTypeId)
                .ValueGeneratedNever()
                .HasColumnName("organisation_type_id");
            entity.Property(e => e.OrganisationTypeCode)
                .HasMaxLength(50)
                .HasColumnName("organisation_type_code");
            entity.Property(e => e.OrganisationTypeDescription)
                .HasMaxLength(100)
                .HasColumnName("organisation_type_description");
        });

        modelBuilder.Entity<Schema>(entity =>
        {
            entity.HasKey(e => e.Version).HasName("schema_pkey");

            entity.ToTable("schema", "hangfire");

            entity.Property(e => e.Version)
                .ValueGeneratedNever()
                .HasColumnName("version");
        });

        modelBuilder.Entity<SdsQuery>(entity =>
        {
            entity.HasKey(e => e.QueryName).HasName("configuration_sdsquery_queryname_pk");

            entity.ToTable("sds_query", "configuration");

            entity.Property(e => e.QueryName)
                .HasMaxLength(100)
                .HasColumnName("query_name");
            entity.Property(e => e.QueryAttributes)
                .HasMaxLength(500)
                .HasColumnName("query_attributes");
            entity.Property(e => e.QueryText)
                .HasMaxLength(1000)
                .HasColumnName("query_text");
            entity.Property(e => e.SearchBase)
                .HasMaxLength(200)
                .HasColumnName("search_base");
        });

        modelBuilder.Entity<SearchExport>(entity =>
        {
            entity.HasKey(e => e.SearchExportId).HasName("application_searchexport_searchexportid_pk");

            entity.ToTable("search_export", "application");

            entity.Property(e => e.SearchExportId).HasColumnName("search_export_id");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.SearchExportData).HasColumnName("search_export_data");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.SearchExports)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("application_searchexport_userid");
        });

        modelBuilder.Entity<SearchGroup>(entity =>
        {
            entity.HasKey(e => e.SearchGroupId).HasName("application_searchgroup_searchid_pk");

            entity.ToTable("search_group", "application");

            entity.Property(e => e.SearchGroupId).HasColumnName("search_group_id");
            entity.Property(e => e.ConsumerOdsTextbox)
                .HasMaxLength(200)
                .HasColumnName("consumer_ods_textbox");
            entity.Property(e => e.ConsumerOrganisationTypeDropdown)
                .HasMaxLength(50)
                .HasColumnName("consumer_organisation_type_dropdown");
            entity.Property(e => e.ProviderOdsTextbox)
                .HasMaxLength(200)
                .HasColumnName("provider_ods_textbox");
            entity.Property(e => e.SearchEndAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("search_end_at");
            entity.Property(e => e.SearchStartAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("search_start_at");
            entity.Property(e => e.SelectedDaterange)
                .HasMaxLength(200)
                .HasColumnName("selected_daterange");
            entity.Property(e => e.UserSessionId).HasColumnName("user_session_id");

            entity.HasOne(d => d.UserSession).WithMany(p => p.SearchGroups)
                .HasForeignKey(d => d.UserSessionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("application_searchgroup_usersessionid_fk");
        });

        modelBuilder.Entity<SearchResult>(entity =>
        {
            entity.HasKey(e => e.SearchResultId).HasName("application_searchresult_searchresultid_pk");

            entity.ToTable("search_result", "application");

            entity.Property(e => e.SearchResultId).HasColumnName("search_result_id");
            entity.Property(e => e.ConsumerOdsCode)
                .HasMaxLength(10)
                .HasColumnName("consumer_ods_code");
            entity.Property(e => e.ConsumerOrganisationId).HasColumnName("consumer_organisation_id");
            entity.Property(e => e.ConsumerOrganisationType)
                .HasMaxLength(50)
                .HasColumnName("consumer_organisation_type");
            entity.Property(e => e.Details).HasColumnName("details");
            entity.Property(e => e.ErrorCode).HasColumnName("error_code");
            entity.Property(e => e.ProviderOdsCode)
                .HasMaxLength(10)
                .HasColumnName("provider_ods_code");
            entity.Property(e => e.ProviderOrganisationId).HasColumnName("provider_organisation_id");
            entity.Property(e => e.ProviderPublisher)
                .HasMaxLength(200)
                .HasColumnName("provider_publisher");
            entity.Property(e => e.SearchDurationSeconds).HasColumnName("search_duration_seconds");
            entity.Property(e => e.SearchGroupId).HasColumnName("search_group_id");

            entity.HasOne(d => d.ConsumerOrganisation).WithMany(p => p.SearchResultConsumerOrganisations)
                .HasForeignKey(d => d.ConsumerOrganisationId)
                .HasConstraintName("application_searchresult_consumerorganisationid");

            entity.HasOne(d => d.ProviderOrganisation).WithMany(p => p.SearchResultProviderOrganisations)
                .HasForeignKey(d => d.ProviderOrganisationId)
                .HasConstraintName("application_searchresult_providerorganisationid_fk");

            entity.HasOne(d => d.SearchGroup).WithMany(p => p.SearchResults)
                .HasForeignKey(d => d.SearchGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("application_searchresult_searchgroupid_fk");
        });

        modelBuilder.Entity<Server>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("server_pkey");

            entity.ToTable("server", "hangfire");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Data)
                .HasColumnType("jsonb")
                .HasColumnName("data");
            entity.Property(e => e.Lastheartbeat).HasColumnName("lastheartbeat");
            entity.Property(e => e.Updatecount)
                .HasDefaultValue(0)
                .HasColumnName("updatecount");
        });

        modelBuilder.Entity<Set>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("set_pkey");

            entity.ToTable("set", "hangfire");

            entity.HasIndex(e => e.Expireat, "ix_hangfire_set_expireat");

            entity.HasIndex(e => new { e.Key, e.Score }, "ix_hangfire_set_key_score");

            entity.HasIndex(e => new { e.Key, e.Value }, "set_key_value_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Expireat).HasColumnName("expireat");
            entity.Property(e => e.Key).HasColumnName("key");
            entity.Property(e => e.Score).HasColumnName("score");
            entity.Property(e => e.Updatecount)
                .HasDefaultValue(0)
                .HasColumnName("updatecount");
            entity.Property(e => e.Value).HasColumnName("value");
        });

        modelBuilder.Entity<Spine>(entity =>
        {
            entity.HasKey(e => e.SingleRowLock).HasName("configuration_spine_singlerowlock_pk");

            entity.ToTable("spine", "configuration");

            entity.Property(e => e.SingleRowLock).HasColumnName("single_row_lock");
            entity.Property(e => e.Asid)
                .HasMaxLength(20)
                .HasColumnName("asid");
            entity.Property(e => e.ClientCert)
                .HasMaxLength(8000)
                .HasColumnName("client_cert");
            entity.Property(e => e.ClientPrivateKey)
                .HasMaxLength(8000)
                .HasColumnName("client_private_key");
            entity.Property(e => e.OrganisationId).HasColumnName("organisation_id");
            entity.Property(e => e.PartyKey)
                .HasMaxLength(20)
                .HasColumnName("party_key");
            entity.Property(e => e.SdsHostname)
                .HasMaxLength(100)
                .HasColumnName("sds_hostname");
            entity.Property(e => e.SdsPort).HasColumnName("sds_port");
            entity.Property(e => e.SdsTlsVersion)
                .HasMaxLength(9)
                .HasColumnName("sds_tls_version");
            entity.Property(e => e.SdsUseFhirApi).HasColumnName("sds_use_fhir_api");
            entity.Property(e => e.SdsUseLdaps).HasColumnName("sds_use_ldaps");
            entity.Property(e => e.SdsUseMutualauth).HasColumnName("sds_use_mutualauth");
            entity.Property(e => e.ServerCaCertchain)
                .HasMaxLength(8000)
                .HasColumnName("server_ca_certchain");
            entity.Property(e => e.SpineFhirApiDirectoryServicesFqdn)
                .HasMaxLength(100)
                .HasColumnName("spine_fhir_api_directory_services_fqdn");
            entity.Property(e => e.SpineFhirApiKey)
                .HasMaxLength(100)
                .HasColumnName("spine_fhir_api_key");
            entity.Property(e => e.SpineFhirApiSystemsRegisterFqdn)
                .HasMaxLength(100)
                .HasColumnName("spine_fhir_api_systems_register_fqdn");
            entity.Property(e => e.SpineFqdn)
                .HasColumnType("character varying")
                .HasColumnName("spine_fqdn");
            entity.Property(e => e.SspHostname)
                .HasMaxLength(100)
                .HasColumnName("ssp_hostname");
            entity.Property(e => e.TimeoutSeconds).HasColumnName("timeout_seconds");
            entity.Property(e => e.UseSsp).HasColumnName("use_ssp");

            entity.HasOne(d => d.Organisation).WithMany(p => p.Spines)
                .HasForeignKey(d => d.OrganisationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("configuration_spine_organisationid_fk");
        });

        modelBuilder.Entity<SpineMessage>(entity =>
        {
            entity.HasKey(e => e.SpineMessageId).HasName("logging_spinemessage_spinemessageid_pk");

            entity.ToTable("spine_message", "logging");

            entity.HasIndex(e => e.SearchResultId, "logging_spinemessage_searchresultid_ix").HasFilter("(search_result_id IS NOT NULL)");

            entity.Property(e => e.SpineMessageId).HasColumnName("spine_message_id");
            entity.Property(e => e.Command)
                .HasMaxLength(8000)
                .HasColumnName("command");
            entity.Property(e => e.LoggedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("logged_date");
            entity.Property(e => e.RequestHeaders).HasColumnName("request_headers");
            entity.Property(e => e.RequestPayload).HasColumnName("request_payload");
            entity.Property(e => e.ResponseHeaders).HasColumnName("response_headers");
            entity.Property(e => e.ResponsePayload).HasColumnName("response_payload");
            entity.Property(e => e.ResponseStatus)
                .HasMaxLength(100)
                .HasColumnName("response_status");
            entity.Property(e => e.RoundtriptimeMs).HasColumnName("roundtriptime_ms");
            entity.Property(e => e.SearchResultId).HasColumnName("search_result_id");
            entity.Property(e => e.SpineMessageTypeId).HasColumnName("spine_message_type_id");
            entity.Property(e => e.UserSessionId).HasColumnName("user_session_id");

            entity.HasOne(d => d.SearchResult).WithMany(p => p.SpineMessages)
                .HasForeignKey(d => d.SearchResultId)
                .HasConstraintName("logging_spinemessage_searchresultid_fk");

            entity.HasOne(d => d.SpineMessageType).WithMany(p => p.SpineMessages)
                .HasForeignKey(d => d.SpineMessageTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("logging_spinemessage_spinemessagetypeid_fk");

            entity.HasOne(d => d.UserSession).WithMany(p => p.SpineMessages)
                .HasForeignKey(d => d.UserSessionId)
                .HasConstraintName("logging_spinemessage_usersessionid_fk");
        });

        modelBuilder.Entity<SpineMessageType>(entity =>
        {
            entity.HasKey(e => e.SpineMessageTypeId).HasName("configuration_spinemessagetype_spinemessagetypeid_pk");

            entity.ToTable("spine_message_type", "configuration");

            entity.Property(e => e.SpineMessageTypeId)
                .ValueGeneratedNever()
                .HasColumnName("spine_message_type_id");
            entity.Property(e => e.InteractionId)
                .HasMaxLength(200)
                .HasColumnName("interaction_id");
            entity.Property(e => e.SpineMessageTypeName)
                .HasMaxLength(200)
                .HasColumnName("spine_message_type_name");
        });

        modelBuilder.Entity<Sso>(entity =>
        {
            entity.HasKey(e => e.SingleRowLock).HasName("configuration_sso_singlerowlock_pk");

            entity.ToTable("sso", "configuration");

            entity.Property(e => e.SingleRowLock).HasColumnName("single_row_lock");
            entity.Property(e => e.AuthEndpoint)
                .HasMaxLength(1000)
                .HasColumnName("auth_endpoint");
            entity.Property(e => e.AuthScheme)
                .HasMaxLength(100)
                .HasColumnName("auth_scheme");
            entity.Property(e => e.CallbackPath)
                .HasMaxLength(1000)
                .HasColumnName("callback_path");
            entity.Property(e => e.ChallengeScheme)
                .HasMaxLength(100)
                .HasColumnName("challenge_scheme");
            entity.Property(e => e.ClientId)
                .HasMaxLength(200)
                .HasColumnName("client_id");
            entity.Property(e => e.ClientSecret)
                .HasMaxLength(1000)
                .HasColumnName("client_secret");
            entity.Property(e => e.MetadataEndpoint)
                .HasColumnType("character varying")
                .HasColumnName("metadata_endpoint");
            entity.Property(e => e.SignedOutCallbackPath)
                .HasMaxLength(1000)
                .HasColumnName("signed_out_callback_path");
            entity.Property(e => e.TokenEndpoint)
                .HasMaxLength(1000)
                .HasColumnName("token_endpoint");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("state_pkey");

            entity.ToTable("state", "hangfire");

            entity.HasIndex(e => e.Jobid, "ix_hangfire_state_jobid");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Createdat).HasColumnName("createdat");
            entity.Property(e => e.Data)
                .HasColumnType("jsonb")
                .HasColumnName("data");
            entity.Property(e => e.Jobid).HasColumnName("jobid");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Reason).HasColumnName("reason");
            entity.Property(e => e.Updatecount)
                .HasDefaultValue(0)
                .HasColumnName("updatecount");

            entity.HasOne(d => d.Job).WithMany(p => p.States)
                .HasForeignKey(d => d.Jobid)
                .HasConstraintName("state_jobid_fkey");
        });

        modelBuilder.Entity<Transient>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("transient", "reporting");

            entity.Property(e => e.EntryDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("entry_date");
            entity.Property(e => e.TransientData)
                .HasColumnType("json")
                .HasColumnName("transient_data");
            entity.Property(e => e.TransientId).HasColumnName("transient_id");
            entity.Property(e => e.TransientReportId).HasColumnName("transient_report_id");
            entity.Property(e => e.TransientReportName).HasColumnName("transient_report_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("application_user_userid_pk");

            entity.ToTable("user", "application");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.AddedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("added_date");
            entity.Property(e => e.AuthorisedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("authorised_date");
            entity.Property(e => e.DisplayName)
                .HasMaxLength(200)
                .HasColumnName("display_name");
            entity.Property(e => e.EmailAddress)
                .HasMaxLength(200)
                .HasColumnName("email_address");
            entity.Property(e => e.IsAdmin)
                .HasDefaultValue(false)
                .HasColumnName("is_admin");
            entity.Property(e => e.LastLogonDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_logon_date");
            entity.Property(e => e.MultiSearchEnabled)
                .HasDefaultValue(false)
                .HasColumnName("multi_search_enabled");
            entity.Property(e => e.OrgTypeSearchEnabled)
                .HasDefaultValue(false)
                .HasColumnName("org_type_search_enabled");
            entity.Property(e => e.OrganisationId).HasColumnName("organisation_id");
            entity.Property(e => e.TermsAndConditionsAccepted).HasColumnName("terms_and_conditions_accepted");
            entity.Property(e => e.UserAccountStatusId).HasColumnName("user_account_status_id");

            entity.HasOne(d => d.Organisation).WithMany(p => p.Users)
                .HasForeignKey(d => d.OrganisationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("application_user_organisationid_pk");

            entity.HasOne(d => d.UserAccountStatus).WithMany(p => p.Users)
                .HasForeignKey(d => d.UserAccountStatusId)
                .HasConstraintName("application_user_useraccountstatusid_fk");
        });

        modelBuilder.Entity<UserAccountStatus>(entity =>
        {
            entity.HasKey(e => e.UserAccountStatusId).HasName("application_useraccountstatus_useraccountstatusid_pk");

            entity.ToTable("user_account_status", "application");

            entity.Property(e => e.UserAccountStatusId)
                .ValueGeneratedNever()
                .HasColumnName("user_account_status_id");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .HasColumnName("description");
        });

        modelBuilder.Entity<UserSession>(entity =>
        {
            entity.HasKey(e => e.UserSessionId).HasName("application_usersession_usersessionid_pk");

            entity.ToTable("user_session", "application");

            entity.Property(e => e.UserSessionId).HasColumnName("user_session_id");
            entity.Property(e => e.EndTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("end_time");
            entity.Property(e => e.StartTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("start_time");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserSessions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("application_usersession_userid_fk");
        });

        modelBuilder.Entity<WebRequest>(entity =>
        {
            entity.HasKey(e => e.WebRequestId).HasName("logging_webrequest_webrequestid_pk");

            entity.ToTable("web_request", "logging");

            entity.Property(e => e.WebRequestId).HasColumnName("web_request_id");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(255)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.Ip)
                .HasMaxLength(255)
                .HasColumnName("ip");
            entity.Property(e => e.ReferrerUrl)
                .HasMaxLength(2000)
                .HasColumnName("referrer_url");
            entity.Property(e => e.ResponseCode).HasColumnName("response_code");
            entity.Property(e => e.Server)
                .HasMaxLength(255)
                .HasColumnName("server");
            entity.Property(e => e.SessionId)
                .HasMaxLength(1000)
                .HasColumnName("session_id");
            entity.Property(e => e.Url)
                .HasMaxLength(1000)
                .HasColumnName("url");
            entity.Property(e => e.UserAgent)
                .HasMaxLength(1000)
                .HasColumnName("user_agent");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.UserSessionId).HasColumnName("user_session_id");

            entity.HasOne(d => d.User).WithMany(p => p.WebRequests)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("logging_webrequest_userid_fk");

            entity.HasOne(d => d.UserSession).WithMany(p => p.WebRequests)
                .HasForeignKey(d => d.UserSessionId)
                .HasConstraintName("logging_webrequest_usersessionid_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
