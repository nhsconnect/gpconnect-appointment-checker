using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Novell.Directory.Ldap;
using Npgsql;

namespace gpconnect_appointment_checker.Console
{
    class Program
    {
        static readonly string _connectionstring = Environment.GetEnvironmentVariable("ConnectionStrings:DefaultConnection");
        static readonly List<LdapQuery> _ldapQueries = GetLdapQueries();
        static readonly SpineConfiguration _spineConfiguration = GetSpineConfiguration();

        static void Main(string[] args)
        {
            try
            {
                RunLdapQueries();
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

        private static void RunLdapQueries()
        {
            string[] odsCodes = {"A20047", "X26", "J82132", "B82619", "B82617", "B82614", "J82132", "RR8"};
            var query = _ldapQueries.FirstOrDefault(x => x.query_name == "GetOrganisationDetailsByOdsCode");
            for (var i = 0; i <= 100; i++)
            {
                for (var j = 0; j < odsCodes.Length; j++)
                {
                    var filter = query.query_text.Replace("{odsCode}", odsCodes[j]);
                    var results = new Dictionary<string, object>();
                    using (ILdapConnection ldapConnection = new LdapConnection
                    {
                        SecureSocketLayer = _spineConfiguration.sds_use_ldaps,
                        ConnectionTimeout = _spineConfiguration.timeout_seconds * 1000
                    })
                    {
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
                    System.Console.WriteLine($"For query {filter} - iteration {i}, result count is {results.Count}");
                }
            }
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
                        ssp_hostname = reader.GetString("ssp_hostname"),
                        sds_hostname = reader.GetString("sds_hostname"),
                        sds_port = reader.GetInt32("sds_port"),
                        sds_use_ldaps = reader.GetBoolean("sds_use_ldaps"),
                        party_key = reader.GetString("party_key"),
                        asid = reader.GetString("asid"),
                        organisation_id = reader.GetInt32("organisation_id"),
                        timeout_seconds = reader.GetInt32("timeout_seconds"),
                        client_cert = reader.GetString("client_cert"),
                        client_private_key = reader.GetString("client_private_key"),
                        server_ca_certchain = reader.GetString("server_ca_certchain"),
                    };
                }

                connection.Close();
            }
            return result;
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
    }
}
