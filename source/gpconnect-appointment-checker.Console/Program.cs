﻿using gpconnect_appointment_checker.Console.Helpers;
using Novell.Directory.Ldap;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace gpconnect_appointment_checker.Console
{
    class Program
    {
        static string _connectionstring;
        static List<LdapQuery> _ldapQueries; 
        static SpineConfiguration _spineConfiguration; 
        static X509Certificate _clientCertificate;

        static void Main(string[] args)
        {
            try
            {
                if ((args == null) || (args.Length == 0))
                {
                    System.Console.WriteLine("Please pass the first argument as DB connection string in the format Server=PG_HOST;Port=PG_PORT;Database=PG_DB;User Id=PG_USER;Password=PG_PASS");
                    Environment.Exit(-1);
                }

                _connectionstring = args.First();

                System.Console.WriteLine($"Using DB connection string: {_connectionstring}");

                int numberOfGoes = 1;

                if (args.Length > 1)
                    int.TryParse(args.Skip(1).First(), out numberOfGoes);

                _ldapQueries = GetLdapQueries();
                _spineConfiguration = GetSpineConfiguration();

                System.Console.WriteLine($"Running {numberOfGoes} iteration(s).");
                RunLdapQueries(numberOfGoes);
            }
            catch (InterThreadException e)
            {
                System.Console.WriteLine($"An InterThreadException has occurred: {e}");
                throw;
            }
            catch (LdapException e)
            {
                System.Console.WriteLine($"An LdapException has occurred: {e}");
                throw;
            }
            catch (Exception e)
            {
                System.Console.WriteLine($"An Exception has occurred: {e}");
                throw;
            }
        }

        private static void RunLdapQueries(int numberOfGoes)
        {
            string[] odsCodes = {"A20047", "X26", "J82132", "B82619", "B82617", "B82614", "J82132", "RR8"};
            var query = _ldapQueries.FirstOrDefault(x => x.query_name == "GetOrganisationDetailsByOdsCode");

            for (var i = 0; i < numberOfGoes; i++)
            {
                for (var j = 0; j < odsCodes.Length; j++)
                {
                    var filter = query.query_text.Replace("{odsCode}", odsCodes[j]);
                    var results = new Dictionary<string, object>();

                    using (LdapConnection ldapConnection = new LdapConnection())
                    {
                        ldapConnection.SecureSocketLayer = _spineConfiguration.sds_use_ldaps;
                        ldapConnection.ConnectionTimeout = _spineConfiguration.timeout_seconds * 1000;

                        if (_spineConfiguration.sds_use_mutualauth)
                        {
                            System.Console.WriteLine("Using Mutual Auth");

                            var clientCertData = CertificateHelper.ExtractCertInstances(_spineConfiguration.client_cert);
                            var clientPrivateKeyData = CertificateHelper.ExtractKeyInstance(_spineConfiguration.client_private_key);
                            var x509ClientCertificate = new X509Certificate2(clientCertData.FirstOrDefault());

                            var privateKey = RSA.Create();
                            privateKey.ImportRSAPrivateKey(clientPrivateKeyData, out _);
                            var x509CertificateWithPrivateKey = x509ClientCertificate.CopyWithPrivateKey(privateKey);
                            var pfxFormattedCertificate = new X509Certificate2(x509CertificateWithPrivateKey.Export(X509ContentType.Pfx, string.Empty), string.Empty);

                            _clientCertificate = pfxFormattedCertificate;

                            ldapConnection.UserDefinedServerCertValidationDelegate += ValidateServerCertificateChain;
                            ldapConnection.UserDefinedClientCertSelectionDelegate += SelectLocalCertificate;
                        }

                        ldapConnection.Connect(_spineConfiguration.sds_hostname, _spineConfiguration.sds_port);
                        var searchResults = ldapConnection.Search(query.search_base, LdapConnection.ScopeSub, filter, null, false);

                        while (searchResults.HasMore())
                        {
                            var nextEntry = searchResults.Next();
                            var attributeSet = nextEntry.GetAttributeSet();

                            foreach (var attribute in attributeSet)
                            {
                                results.TryAdd(attribute.Name, attribute.StringValue);
                            }
                        }
                    }
                    System.Console.WriteLine($"For query {filter} - iteration {i+1}, result count is {results.Count}");
                }
            }
        }



        private static bool ValidateServerCertificateChain(object sender, X509Certificate pfxFormattedCertificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;
            System.Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            var serverCert = _spineConfiguration.server_ca_certchain;
            var serverCertData = CertificateHelper.ExtractCertInstances(serverCert);
            var x509ServerCertificateSubCa = new X509Certificate2(serverCertData[0]);
            var x509ServerCertificateRootCa = new X509Certificate2(serverCertData[1]);

            chain.Reset();
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.IgnoreRootRevocationUnknown;
            chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
            chain.ChainPolicy.ExtraStore.Add(x509ServerCertificateSubCa);
            chain.ChainPolicy.ExtraStore.Add(x509ServerCertificateRootCa);

            //if (chain.Build(pfxFormattedCertificate)) return true;
            if (chain.ChainStatus.Where(chainStatus => chainStatus.Status != X509ChainStatusFlags.NoError).All(chainStatus => chainStatus.Status != X509ChainStatusFlags.UntrustedRoot)) return false;
            var providedRoot = chain.ChainElements[^1];
            return x509ServerCertificateRootCa.Thumbprint == providedRoot.Certificate.Thumbprint;
        }

        private static X509Certificate SelectLocalCertificate(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
        {
            return _clientCertificate;
        }

        private static List<LdapQuery> GetLdapQueries()
        {
            var results = new List<LdapQuery>();

            using NpgsqlConnection connection = new NpgsqlConnection(_connectionstring);
            {
                var command = new NpgsqlCommand("configuration.get_sds_queries", connection);
                command.CommandType = CommandType.StoredProcedure;
                connection.Open();

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    results.Add(new LdapQuery
                    {
                        query_name = reader.GetString("query_name"),
                        search_base = reader.GetString("search_base"),
                        query_text = reader.GetString("query_text")
                    });
                }

                connection.Close();
            }
            return results;
        }

        private static SpineConfiguration GetSpineConfiguration()
        {
            var result = new SpineConfiguration();

            using NpgsqlConnection connection = new NpgsqlConnection(_connectionstring);
            {
                var command = new NpgsqlCommand("configuration.get_spine_configuration", connection);
                command.CommandType = CommandType.StoredProcedure;
                connection.Open();

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    result = new SpineConfiguration
                    {
                        use_ssp = reader.GetBoolean("use_ssp"),
                        ssp_hostname = reader.GetNullableString("ssp_hostname"),
                        sds_hostname = reader.GetString("sds_hostname"),
                        sds_port = reader.GetInt32("sds_port"),
                        sds_use_ldaps = reader.GetBoolean("sds_use_ldaps"),
                        party_key = reader.GetString("party_key"),
                        asid = reader.GetString("asid"),
                        organisation_id = reader.GetInt32("organisation_id"),
                        timeout_seconds = reader.GetInt32("timeout_seconds"),
                        client_cert = reader.GetNullableString("client_cert"),
                        client_private_key = reader.GetNullableString("client_private_key"),
                        server_ca_certchain = reader.GetNullableString("server_ca_certchain"),
                        sds_use_mutualauth = reader.GetBoolean("sds_use_mutualauth")
                    };
                }

                connection.Close();
            }
            return result;
        }
    }

    internal static class NgpsqlDataReaderExtensionMethods
    {
        public static string GetNullableString(this NpgsqlDataReader reader, string columnName)
        {
            if (reader.IsDBNull(columnName))
                return null;

            return reader.GetString(columnName);
        }
    }

    public class LdapQuery
    {
        public string query_name { get; set; }
        public string search_base { get; set; }
        public string query_text { get; set; }
    }

    public class SpineConfiguration
    {
        public bool use_ssp { get; set; }
        public string ssp_hostname { get; set; }
        public string sds_hostname { get; set; }
        public int sds_port { get; set; }
        public bool sds_use_ldaps { get; set; }
        public int organisation_id { get; set; }
        public string party_key { get; set; }
        public string asid { get; set; }
        public int timeout_seconds { get; set; }
        public string client_cert { get; set; }
        public string client_private_key { get; set; }
        public string server_ca_certchain { get; set; }
        public bool sds_use_mutualauth { get; set; }
    }
}
