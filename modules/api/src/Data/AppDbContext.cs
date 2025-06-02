using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using gpconnect_appointment_checker.api.Models;

namespace gpconnect_appointment_checker.api.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<_lock> locks { get; set; }

    public virtual DbSet<aggregatedcounter> aggregatedcounters { get; set; }

    public virtual DbSet<counter> counters { get; set; }

    public virtual DbSet<email> emails { get; set; }

    public virtual DbSet<email_template> email_templates { get; set; }

    public virtual DbSet<entry> entries { get; set; }

    public virtual DbSet<entry_type> entry_types { get; set; }

    public virtual DbSet<error_log> error_logs { get; set; }

    public virtual DbSet<fhir_api_query> fhir_api_queries { get; set; }

    public virtual DbSet<flyway_schema_history> flyway_schema_histories { get; set; }

    public virtual DbSet<general> generals { get; set; }

    public virtual DbSet<hash> hashes { get; set; }

    public virtual DbSet<job> jobs { get; set; }

    public virtual DbSet<jobparameter> jobparameters { get; set; }

    public virtual DbSet<jobqueue> jobqueues { get; set; }

    public virtual DbSet<list> lists { get; set; }

    public virtual DbSet<list1> lists1 { get; set; }

    public virtual DbSet<organisation> organisations { get; set; }

    public virtual DbSet<organisation_type> organisation_types { get; set; }

    public virtual DbSet<organisation_type1> organisation_types1 { get; set; }

    public virtual DbSet<schema> schemas { get; set; }

    public virtual DbSet<sds_query> sds_queries { get; set; }

    public virtual DbSet<search_export> search_exports { get; set; }

    public virtual DbSet<search_group> search_groups { get; set; }

    public virtual DbSet<search_result> search_results { get; set; }

    public virtual DbSet<server> servers { get; set; }

    public virtual DbSet<set> sets { get; set; }

    public virtual DbSet<spine> spines { get; set; }

    public virtual DbSet<spine_message> spine_messages { get; set; }

    public virtual DbSet<spine_message_type> spine_message_types { get; set; }

    public virtual DbSet<sso> ssos { get; set; }

    public virtual DbSet<state> states { get; set; }

    public virtual DbSet<transient> transients { get; set; }

    public virtual DbSet<user> users { get; set; }

    public virtual DbSet<user_account_status> user_account_statuses { get; set; }

    public virtual DbSet<user_session> user_sessions { get; set; }

    public virtual DbSet<web_request> web_requests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<_lock>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("lock", "hangfire");

            entity.HasIndex(e => e.resource, "lock_resource_key").IsUnique();

            entity.Property(e => e.updatecount).HasDefaultValue(0);
        });

        modelBuilder.Entity<aggregatedcounter>(entity =>
        {
            entity.HasKey(e => e.id).HasName("aggregatedcounter_pkey");

            entity.ToTable("aggregatedcounter", "hangfire");

            entity.HasIndex(e => e.key, "aggregatedcounter_key_key").IsUnique();
        });

        modelBuilder.Entity<counter>(entity =>
        {
            entity.HasKey(e => e.id).HasName("counter_pkey");

            entity.ToTable("counter", "hangfire");

            entity.HasIndex(e => e.expireat, "ix_hangfire_counter_expireat");

            entity.HasIndex(e => e.key, "ix_hangfire_counter_key");
        });

        modelBuilder.Entity<email>(entity =>
        {
            entity.HasKey(e => e.single_row_lock).HasName("configuration_email_singlerowlock_pk");

            entity.ToTable("email", "configuration");

            entity.Property(e => e.default_subject).HasMaxLength(100);
            entity.Property(e => e.encryption).HasMaxLength(10);
            entity.Property(e => e.host_name).HasMaxLength(100);
            entity.Property(e => e.password).HasMaxLength(100);
            entity.Property(e => e.sender_address).HasMaxLength(100);
            entity.Property(e => e.user_name).HasMaxLength(100);
        });

        modelBuilder.Entity<email_template>(entity =>
        {
            entity.HasKey(e => e.email_template_id).HasName("application_emailtemplate_emailtemplateid_pk");

            entity.ToTable("email_template", "application");

            entity.Property(e => e.email_template_id).ValueGeneratedNever();
            entity.Property(e => e.subject).HasMaxLength(100);
        });

        modelBuilder.Entity<entry>(entity =>
        {
            entity.HasKey(e => e.entry_id).HasName("audit_entry_entryid_pk");

            entity.ToTable("entry", "audit");

            entity.Property(e => e.details).HasMaxLength(1000);
            entity.Property(e => e.entry_date).HasColumnType("timestamp without time zone");
            entity.Property(e => e.item1).HasMaxLength(1000);
            entity.Property(e => e.item2).HasMaxLength(1000);
            entity.Property(e => e.item3).HasMaxLength(1000);

            entity.HasOne(d => d.admin_user).WithMany(p => p.entryadmin_users)
                .HasForeignKey(d => d.admin_user_id)
                .HasConstraintName("audit_entry_adminuserid_fk");

            entity.HasOne(d => d.entry_type).WithMany(p => p.entries)
                .HasForeignKey(d => d.entry_type_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("audit_entry_entrytypeid_fk");

            entity.HasOne(d => d.user).WithMany(p => p.entryusers)
                .HasForeignKey(d => d.user_id)
                .HasConstraintName("audit_entry_userid_fk");

            entity.HasOne(d => d.user_session).WithMany(p => p.entries)
                .HasForeignKey(d => d.user_session_id)
                .HasConstraintName("audit_entry_usersessionid_fk");
        });

        modelBuilder.Entity<entry_type>(entity =>
        {
            entity.HasKey(e => e.entry_type_id).HasName("audit_entrytype_entrytypeid_pk");

            entity.ToTable("entry_type", "audit");

            entity.Property(e => e.entry_type_id).ValueGeneratedNever();
            entity.Property(e => e.details_description).HasMaxLength(100);
            entity.Property(e => e.entry_description).HasMaxLength(200);
            entity.Property(e => e.item1_description).HasMaxLength(100);
            entity.Property(e => e.item2_description).HasMaxLength(100);
            entity.Property(e => e.item3_description).HasMaxLength(100);
        });

        modelBuilder.Entity<error_log>(entity =>
        {
            entity.HasKey(e => e.id).HasName("logging_errorlog_logid_pk");

            entity.ToTable("error_log", "logging");

            entity.Property(e => e.application).HasMaxLength(100);
            entity.Property(e => e.callsite).HasMaxLength(8000);
            entity.Property(e => e.exception).HasMaxLength(8000);
            entity.Property(e => e.level).HasMaxLength(100);
            entity.Property(e => e.logged).HasColumnType("timestamp without time zone");
            entity.Property(e => e.logger).HasMaxLength(8000);
            entity.Property(e => e.message).HasMaxLength(8000);
        });

        modelBuilder.Entity<fhir_api_query>(entity =>
        {
            entity.HasKey(e => e.query_name).HasName("configuration_fhirapiquery_queryname_pk");

            entity.ToTable("fhir_api_query", "configuration");

            entity.Property(e => e.query_name).HasMaxLength(100);
            entity.Property(e => e.query_text).HasMaxLength(1000);
        });

        modelBuilder.Entity<flyway_schema_history>(entity =>
        {
            entity.HasKey(e => e.installed_rank).HasName("flyway_schema_history_pk");

            entity.ToTable("flyway_schema_history");

            entity.HasIndex(e => e.success, "flyway_schema_history_s_idx");

            entity.Property(e => e.installed_rank).ValueGeneratedNever();
            entity.Property(e => e.description).HasMaxLength(200);
            entity.Property(e => e.installed_by).HasMaxLength(100);
            entity.Property(e => e.installed_on)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.script).HasMaxLength(1000);
            entity.Property(e => e.type).HasMaxLength(20);
            entity.Property(e => e.version).HasMaxLength(50);
        });

        modelBuilder.Entity<general>(entity =>
        {
            entity.HasKey(e => e.single_row_lock).HasName("configuration_general_singlerowlock_pk");

            entity.ToTable("general", "configuration");

            entity.Property(e => e.get_access_email_address).HasColumnType("character varying");
            entity.Property(e => e.product_name).HasMaxLength(100);
            entity.Property(e => e.product_version).HasMaxLength(100);
        });

        modelBuilder.Entity<hash>(entity =>
        {
            entity.HasKey(e => e.id).HasName("hash_pkey");

            entity.ToTable("hash", "hangfire");

            entity.HasIndex(e => new { e.key, e.field }, "hash_key_field_key").IsUnique();

            entity.HasIndex(e => e.expireat, "ix_hangfire_hash_expireat");

            entity.Property(e => e.updatecount).HasDefaultValue(0);
        });

        modelBuilder.Entity<job>(entity =>
        {
            entity.HasKey(e => e.id).HasName("job_pkey");

            entity.ToTable("job", "hangfire");

            entity.HasIndex(e => e.expireat, "ix_hangfire_job_expireat");

            entity.HasIndex(e => e.statename, "ix_hangfire_job_statename");

            entity.HasIndex(e => e.statename, "ix_hangfire_job_statename_is_not_null").HasFilter("(statename IS NOT NULL)");

            entity.Property(e => e.arguments).HasColumnType("jsonb");
            entity.Property(e => e.invocationdata).HasColumnType("jsonb");
            entity.Property(e => e.updatecount).HasDefaultValue(0);
        });

        modelBuilder.Entity<jobparameter>(entity =>
        {
            entity.HasKey(e => e.id).HasName("jobparameter_pkey");

            entity.ToTable("jobparameter", "hangfire");

            entity.HasIndex(e => new { e.jobid, e.name }, "ix_hangfire_jobparameter_jobidandname");

            entity.Property(e => e.updatecount).HasDefaultValue(0);

            entity.HasOne(d => d.job).WithMany(p => p.jobparameters)
                .HasForeignKey(d => d.jobid)
                .HasConstraintName("jobparameter_jobid_fkey");
        });

        modelBuilder.Entity<jobqueue>(entity =>
        {
            entity.HasKey(e => e.id).HasName("jobqueue_pkey");

            entity.ToTable("jobqueue", "hangfire");

            entity.HasIndex(e => new { e.fetchedat, e.queue, e.jobid }, "ix_hangfire_jobqueue_fetchedat_queue_jobid").HasNullSortOrder(new[] { NullSortOrder.NullsFirst, NullSortOrder.NullsLast, NullSortOrder.NullsLast });

            entity.HasIndex(e => new { e.jobid, e.queue }, "ix_hangfire_jobqueue_jobidandqueue");

            entity.HasIndex(e => new { e.queue, e.fetchedat }, "ix_hangfire_jobqueue_queueandfetchedat");

            entity.Property(e => e.updatecount).HasDefaultValue(0);
        });

        modelBuilder.Entity<list>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("list", "reporting");

            entity.Property(e => e.function_name).HasMaxLength(100);
            entity.Property(e => e.interaction).HasColumnType("json");
            entity.Property(e => e.report_id).HasMaxLength(100);
            entity.Property(e => e.report_name).HasMaxLength(100);
            entity.Property(e => e.workflow).HasColumnType("json");
        });

        modelBuilder.Entity<list1>(entity =>
        {
            entity.HasKey(e => e.id).HasName("list_pkey");

            entity.ToTable("list", "hangfire");

            entity.HasIndex(e => e.expireat, "ix_hangfire_list_expireat");

            entity.Property(e => e.updatecount).HasDefaultValue(0);
        });

        modelBuilder.Entity<organisation>(entity =>
        {
            entity.HasKey(e => e.organisation_id).HasName("application_organisation_organisationid_pk");

            entity.ToTable("organisation", "application");

            entity.HasIndex(e => e.ods_code, "application_organisation_odscode_uq").IsUnique();

            entity.Property(e => e.added_date).HasColumnType("timestamp without time zone");
            entity.Property(e => e.address_line_1).HasMaxLength(100);
            entity.Property(e => e.address_line_2).HasMaxLength(100);
            entity.Property(e => e.city).HasMaxLength(100);
            entity.Property(e => e.county).HasMaxLength(100);
            entity.Property(e => e.last_sync_date).HasColumnType("timestamp without time zone");
            entity.Property(e => e.locality).HasMaxLength(100);
            entity.Property(e => e.ods_code).HasMaxLength(10);
            entity.Property(e => e.organisation_name).HasMaxLength(100);
            entity.Property(e => e.postcode).HasMaxLength(10);

            entity.HasOne(d => d.organisation_type).WithMany(p => p.organisations)
                .HasForeignKey(d => d.organisation_type_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("application_organisation_organisationtypeid_fk");
        });

        modelBuilder.Entity<organisation_type>(entity =>
        {
            entity.HasKey(e => e.organisation_type_id).HasName("application_organisationtype_organisationtypeid_pk");

            entity.ToTable("organisation_type", "application");

            entity.Property(e => e.organisation_type_id).ValueGeneratedNever();
            entity.Property(e => e.organisation_type_name).HasMaxLength(200);
        });

        modelBuilder.Entity<organisation_type1>(entity =>
        {
            entity.HasKey(e => e.organisation_type_id).HasName("configuration_organisationtype_organisationtypeid_pk");

            entity.ToTable("organisation_type", "configuration");

            entity.Property(e => e.organisation_type_id).ValueGeneratedNever();
            entity.Property(e => e.organisation_type_code).HasMaxLength(50);
            entity.Property(e => e.organisation_type_description).HasMaxLength(100);
        });

        modelBuilder.Entity<schema>(entity =>
        {
            entity.HasKey(e => e.version).HasName("schema_pkey");

            entity.ToTable("schema", "hangfire");

            entity.Property(e => e.version).ValueGeneratedNever();
        });

        modelBuilder.Entity<sds_query>(entity =>
        {
            entity.HasKey(e => e.query_name).HasName("configuration_sdsquery_queryname_pk");

            entity.ToTable("sds_query", "configuration");

            entity.Property(e => e.query_name).HasMaxLength(100);
            entity.Property(e => e.query_attributes).HasMaxLength(500);
            entity.Property(e => e.query_text).HasMaxLength(1000);
            entity.Property(e => e.search_base).HasMaxLength(200);
        });

        modelBuilder.Entity<search_export>(entity =>
        {
            entity.HasKey(e => e.search_export_id).HasName("application_searchexport_searchexportid_pk");

            entity.ToTable("search_export", "application");

            entity.Property(e => e.created_date)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.user).WithMany(p => p.search_exports)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("application_searchexport_userid");
        });

        modelBuilder.Entity<search_group>(entity =>
        {
            entity.HasKey(e => e.search_group_id).HasName("application_searchgroup_searchid_pk");

            entity.ToTable("search_group", "application");

            entity.Property(e => e.consumer_ods_textbox).HasMaxLength(200);
            entity.Property(e => e.consumer_organisation_type_dropdown).HasMaxLength(50);
            entity.Property(e => e.provider_ods_textbox).HasMaxLength(200);
            entity.Property(e => e.search_end_at).HasColumnType("timestamp without time zone");
            entity.Property(e => e.search_start_at).HasColumnType("timestamp without time zone");
            entity.Property(e => e.selected_daterange).HasMaxLength(200);

            entity.HasOne(d => d.user_session).WithMany(p => p.search_groups)
                .HasForeignKey(d => d.user_session_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("application_searchgroup_usersessionid_fk");
        });

        modelBuilder.Entity<search_result>(entity =>
        {
            entity.HasKey(e => e.search_result_id).HasName("application_searchresult_searchresultid_pk");

            entity.ToTable("search_result", "application");

            entity.Property(e => e.consumer_ods_code).HasMaxLength(10);
            entity.Property(e => e.consumer_organisation_type).HasMaxLength(50);
            entity.Property(e => e.provider_ods_code).HasMaxLength(10);
            entity.Property(e => e.provider_publisher).HasMaxLength(200);

            entity.HasOne(d => d.consumer_organisation).WithMany(p => p.search_resultconsumer_organisations)
                .HasForeignKey(d => d.consumer_organisation_id)
                .HasConstraintName("application_searchresult_consumerorganisationid");

            entity.HasOne(d => d.provider_organisation).WithMany(p => p.search_resultprovider_organisations)
                .HasForeignKey(d => d.provider_organisation_id)
                .HasConstraintName("application_searchresult_providerorganisationid_fk");

            entity.HasOne(d => d.search_group).WithMany(p => p.search_results)
                .HasForeignKey(d => d.search_group_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("application_searchresult_searchgroupid_fk");
        });

        modelBuilder.Entity<server>(entity =>
        {
            entity.HasKey(e => e.id).HasName("server_pkey");

            entity.ToTable("server", "hangfire");

            entity.Property(e => e.data).HasColumnType("jsonb");
            entity.Property(e => e.updatecount).HasDefaultValue(0);
        });

        modelBuilder.Entity<set>(entity =>
        {
            entity.HasKey(e => e.id).HasName("set_pkey");

            entity.ToTable("set", "hangfire");

            entity.HasIndex(e => e.expireat, "ix_hangfire_set_expireat");

            entity.HasIndex(e => new { e.key, e.score }, "ix_hangfire_set_key_score");

            entity.HasIndex(e => new { e.key, e.value }, "set_key_value_key").IsUnique();

            entity.Property(e => e.updatecount).HasDefaultValue(0);
        });

        modelBuilder.Entity<spine>(entity =>
        {
            entity.HasKey(e => e.single_row_lock).HasName("configuration_spine_singlerowlock_pk");

            entity.ToTable("spine", "configuration");

            entity.Property(e => e.asid).HasMaxLength(20);
            entity.Property(e => e.client_cert).HasMaxLength(8000);
            entity.Property(e => e.client_private_key).HasMaxLength(8000);
            entity.Property(e => e.party_key).HasMaxLength(20);
            entity.Property(e => e.sds_hostname).HasMaxLength(100);
            entity.Property(e => e.sds_tls_version).HasMaxLength(9);
            entity.Property(e => e.server_ca_certchain).HasMaxLength(8000);
            entity.Property(e => e.spine_fhir_api_directory_services_fqdn).HasMaxLength(100);
            entity.Property(e => e.spine_fhir_api_key).HasMaxLength(100);
            entity.Property(e => e.spine_fhir_api_systems_register_fqdn).HasMaxLength(100);
            entity.Property(e => e.spine_fqdn).HasColumnType("character varying");
            entity.Property(e => e.ssp_hostname).HasMaxLength(100);

            entity.HasOne(d => d.organisation).WithMany(p => p.spines)
                .HasForeignKey(d => d.organisation_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("configuration_spine_organisationid_fk");
        });

        modelBuilder.Entity<spine_message>(entity =>
        {
            entity.HasKey(e => e.spine_message_id).HasName("logging_spinemessage_spinemessageid_pk");

            entity.ToTable("spine_message", "logging");

            entity.HasIndex(e => e.search_result_id, "logging_spinemessage_searchresultid_ix").HasFilter("(search_result_id IS NOT NULL)");

            entity.Property(e => e.command).HasMaxLength(8000);
            entity.Property(e => e.logged_date).HasColumnType("timestamp without time zone");
            entity.Property(e => e.response_status).HasMaxLength(100);

            entity.HasOne(d => d.search_result).WithMany(p => p.spine_messages)
                .HasForeignKey(d => d.search_result_id)
                .HasConstraintName("logging_spinemessage_searchresultid_fk");

            entity.HasOne(d => d.spine_message_type).WithMany(p => p.spine_messages)
                .HasForeignKey(d => d.spine_message_type_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("logging_spinemessage_spinemessagetypeid_fk");

            entity.HasOne(d => d.user_session).WithMany(p => p.spine_messages)
                .HasForeignKey(d => d.user_session_id)
                .HasConstraintName("logging_spinemessage_usersessionid_fk");
        });

        modelBuilder.Entity<spine_message_type>(entity =>
        {
            entity.HasKey(e => e.spine_message_type_id).HasName("configuration_spinemessagetype_spinemessagetypeid_pk");

            entity.ToTable("spine_message_type", "configuration");

            entity.Property(e => e.spine_message_type_id).ValueGeneratedNever();
            entity.Property(e => e.interaction_id).HasMaxLength(200);
            entity.Property(e => e.spine_message_type_name).HasMaxLength(200);
        });

        modelBuilder.Entity<sso>(entity =>
        {
            entity.HasKey(e => e.single_row_lock).HasName("configuration_sso_singlerowlock_pk");

            entity.ToTable("sso", "configuration");

            entity.Property(e => e.auth_endpoint).HasMaxLength(1000);
            entity.Property(e => e.auth_scheme).HasMaxLength(100);
            entity.Property(e => e.callback_path).HasMaxLength(1000);
            entity.Property(e => e.challenge_scheme).HasMaxLength(100);
            entity.Property(e => e.client_id).HasMaxLength(200);
            entity.Property(e => e.client_secret).HasMaxLength(1000);
            entity.Property(e => e.metadata_endpoint).HasColumnType("character varying");
            entity.Property(e => e.signed_out_callback_path).HasMaxLength(1000);
            entity.Property(e => e.token_endpoint).HasMaxLength(1000);
        });

        modelBuilder.Entity<state>(entity =>
        {
            entity.HasKey(e => e.id).HasName("state_pkey");

            entity.ToTable("state", "hangfire");

            entity.HasIndex(e => e.jobid, "ix_hangfire_state_jobid");

            entity.Property(e => e.data).HasColumnType("jsonb");
            entity.Property(e => e.updatecount).HasDefaultValue(0);

            entity.HasOne(d => d.job).WithMany(p => p.states)
                .HasForeignKey(d => d.jobid)
                .HasConstraintName("state_jobid_fkey");
        });

        modelBuilder.Entity<transient>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("transient", "reporting");

            entity.Property(e => e.entry_date).HasColumnType("timestamp without time zone");
            entity.Property(e => e.transient_data).HasColumnType("json");
        });

        modelBuilder.Entity<user>(entity =>
        {
            entity.HasKey(e => e.user_id).HasName("application_user_userid_pk");

            entity.ToTable("user", "application");

            entity.Property(e => e.added_date).HasColumnType("timestamp without time zone");
            entity.Property(e => e.authorised_date).HasColumnType("timestamp without time zone");
            entity.Property(e => e.display_name).HasMaxLength(200);
            entity.Property(e => e.email_address).HasMaxLength(200);
            entity.Property(e => e.is_admin).HasDefaultValue(false);
            entity.Property(e => e.last_logon_date).HasColumnType("timestamp without time zone");
            entity.Property(e => e.multi_search_enabled).HasDefaultValue(false);
            entity.Property(e => e.org_type_search_enabled).HasDefaultValue(false);

            entity.HasOne(d => d.organisation).WithMany(p => p.users)
                .HasForeignKey(d => d.organisation_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("application_user_organisationid_pk");

            entity.HasOne(d => d.user_account_status).WithMany(p => p.users)
                .HasForeignKey(d => d.user_account_status_id)
                .HasConstraintName("application_user_useraccountstatusid_fk");
        });

        modelBuilder.Entity<user_account_status>(entity =>
        {
            entity.HasKey(e => e.user_account_status_id).HasName("application_useraccountstatus_useraccountstatusid_pk");

            entity.ToTable("user_account_status", "application");

            entity.Property(e => e.user_account_status_id).ValueGeneratedNever();
            entity.Property(e => e.description).HasMaxLength(100);
        });

        modelBuilder.Entity<user_session>(entity =>
        {
            entity.HasKey(e => e.user_session_id).HasName("application_usersession_usersessionid_pk");

            entity.ToTable("user_session", "application");

            entity.Property(e => e.end_time).HasColumnType("timestamp without time zone");
            entity.Property(e => e.start_time).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.user).WithMany(p => p.user_sessions)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("application_usersession_userid_fk");
        });

        modelBuilder.Entity<web_request>(entity =>
        {
            entity.HasKey(e => e.web_request_id).HasName("logging_webrequest_webrequestid_pk");

            entity.ToTable("web_request", "logging");

            entity.Property(e => e.created_by).HasMaxLength(255);
            entity.Property(e => e.created_date).HasColumnType("timestamp without time zone");
            entity.Property(e => e.description).HasMaxLength(1000);
            entity.Property(e => e.ip).HasMaxLength(255);
            entity.Property(e => e.referrer_url).HasMaxLength(2000);
            entity.Property(e => e.server).HasMaxLength(255);
            entity.Property(e => e.session_id).HasMaxLength(1000);
            entity.Property(e => e.url).HasMaxLength(1000);
            entity.Property(e => e.user_agent).HasMaxLength(1000);

            entity.HasOne(d => d.user).WithMany(p => p.web_requests)
                .HasForeignKey(d => d.user_id)
                .HasConstraintName("logging_webrequest_userid_fk");

            entity.HasOne(d => d.user_session).WithMany(p => p.web_requests)
                .HasForeignKey(d => d.user_session_id)
                .HasConstraintName("logging_webrequest_usersessionid_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
