//using gpconnect_appointment_checker.DTO.Response.Application;
//using gpconnect_appointment_checker.DTO.Response.Configuration;
//using gpconnect_appointment_checker.Helpers;
//using gpconnect_appointment_checker.Helpers.Enumerations;
//using gpconnect_appointment_checker.SDS.Interfaces;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Security;
//using System.Security.Cryptography;
//using System.Security.Cryptography.X509Certificates;
//using System.Threading.Tasks;

//namespace gpconnect_appointment_checker.SDS
//{
//    public class SdsQueryExecutionBase : ISdsQueryExecutionBase
//    {
//        private static ILogger<SdsQueryExecutionBase> _logger;
//        private static X509Certificate _clientCertificate;
//        private readonly ILdapService _ldapService;
//        private readonly IFhirApiService _fhirApiService;
//        protected readonly IOptionsMonitor<Spine> _spineOptionsDelegate;

//        public SdsQueryExecutionBase(ILogger<SdsQueryExecutionBase> logger, ILdapService ldapService, IFhirApiService fhirApiService, IOptionsMonitor<Spine> spineOptionsDelegate)
//        {
//            _logger = logger;
//            _ldapService = ldapService;
//            _fhirApiService = fhirApiService;
//            _spineOptionsDelegate = spineOptionsDelegate;
//        }

//        public async Task<Organisation> GetOrganisationDetailsByOdsCode(string odsCode)
//        {
//            if (_spineOptionsDelegate.CurrentValue.SdsUseFhirApi)
//            {
//                return await _fhirApiService.GetOrganisationDetailsByOdsCode(odsCode);
//            }
//            return _ldapService.GetOrganisationDetailsByOdsCode(odsCode);
//        }

//        public async Task<List<OrganisationList>> GetOrganisationDetailsByOdsCode(List<string> odsCodes, ErrorCode errorCodeToRaise)
//        {
//            if (_spineOptionsDelegate.CurrentValue.SdsUseFhirApi)
//            {
//                return await _fhirApiService.GetOrganisationDetailsByOdsCode(odsCodes, errorCodeToRaise);
//            }
//            return _ldapService.GetOrganisationDetailsByOdsCode(odsCodes, errorCodeToRaise);
//        }

//        public async Task<Spine> GetProviderDetails(string odsCode)
//        {
//            if (_spineOptionsDelegate.CurrentValue.SdsUseFhirApi)
//            {
//                var spineProviderDetails = await _fhirApiService.GetProviderDetails(odsCode);
//                return LoadAdditionalDependencies(spineProviderDetails);
//            }
//            else
//            {
//                var spineProviderDetails = _ldapService.GetGpProviderEndpointAndPartyKeyByOdsCode(odsCode);
//                if (spineProviderDetails != null)
//                {
//                    var spineProviderAsId = _ldapService.GetGpProviderAsIdByOdsCodeAndPartyKey(odsCode, spineProviderDetails.PartyKey);
//                    spineProviderDetails.AsId = spineProviderAsId.AsId;
//                    spineProviderDetails.ProductName = spineProviderAsId.ProductName;
//                }
//                return LoadAdditionalDependencies(spineProviderDetails);
//            }
//        }

//        public async Task<List<SpineList>> GetProviderDetails(List<string> odsCodes, ErrorCode errorCodeToRaise)
//        {
//            List<SpineList> spineList = null;
//            if (_spineOptionsDelegate.CurrentValue.SdsUseFhirApi)
//            {
//                spineList = await _fhirApiService.GetProviderDetails(odsCodes, errorCodeToRaise);
//                foreach (var entry in spineList.Where(x => x.Spine != null))
//                {
//                    entry.Spine = LoadAdditionalDependencies(entry.Spine);
//                }
//                return spineList;
//            }
//            else
//            {
//                var spineProviderDetails = _ldapService.GetGpProviderEndpointAndPartyKeyByOdsCode(odsCodes, errorCodeToRaise);
//                var spineProviderAsIds = _ldapService.GetGpProviderAsIdByOdsCodeAndPartyKey(spineProviderDetails);

//                UpdateSpineProviderDetailsWithAsIds(spineProviderDetails, spineProviderAsIds);
//                foreach (var entry in spineProviderDetails.Where(x => x.Spine != null))
//                {
//                    entry.Spine = LoadAdditionalDependencies(entry.Spine);
//                }
//                return spineProviderDetails;
//            }
//        }

//        private void UpdateSpineProviderDetailsWithAsIds(List<SpineList> spineProviderDetails, List<SpineList> spineProviderAsIds)
//        {
//            foreach (var spineProviderAsId in spineProviderAsIds)
//            {
//                var itemToChange = spineProviderDetails.FirstOrDefault(d => d.OdsCode == spineProviderAsId.OdsCode);
//                if (itemToChange?.Spine != null)
//                {
//                    itemToChange.Spine.AsId = spineProviderAsId.Spine.AsId;
//                    itemToChange.Spine.ProductName = spineProviderAsId.Spine.ProductName;
//                }
//            }
//        }

//        public async Task<Spine> GetConsumerDetails(string odsCode)
//        {
//            if (_spineOptionsDelegate.CurrentValue.SdsUseFhirApi)
//            {
//                return await _fhirApiService.GetConsumerDetails(odsCode);
//            }
//            else
//            {
//                return _ldapService.GetGpConsumerAsIdByOdsCode(odsCode);
//            }
//        }

//        public async Task<List<SpineList>> GetConsumerDetails(List<string> odsCodes, ErrorCode errorCodeToRaise)
//        {
//            List<SpineList> spineConsumerDetails = null;
//            if (_spineOptionsDelegate.CurrentValue.SdsUseFhirApi)
//            {
//                spineConsumerDetails = await _fhirApiService.GetConsumerDetails(odsCodes, errorCodeToRaise);
//            }
//            else
//            {
//                spineConsumerDetails = _ldapService.GetGpConsumerAsIdByOdsCode(odsCodes, errorCodeToRaise);
//            }
//            foreach (var entry in spineConsumerDetails.Where(x => x.Spine != null))
//            {
//                entry.Spine = LoadAdditionalDependencies(entry.Spine);
//            }
//            return spineConsumerDetails;
//        }

//        private Spine LoadAdditionalDependencies(Spine spine)
//        {
//            if (spine != null)
//            {
//                spine.SpineFqdn = _spineOptionsDelegate.CurrentValue.SpineFqdn;
//                spine.SspFrom = _spineOptionsDelegate.CurrentValue.AsId;
//                spine.SspHostname = _spineOptionsDelegate.CurrentValue.SspHostname;
//                spine.UseSSP = _spineOptionsDelegate.CurrentValue.UseSSP;
//            }
//            return spine;
//        }

//        protected void SetupMutualAuth()
//        {
//            if (_spineOptionsDelegate.CurrentValue.SdsUseMutualAuth)
//            {
//                var clientCertData = CertificateHelper.ExtractCertInstances(_spineOptionsDelegate.CurrentValue.ClientCert);
//                var clientPrivateKeyData = CertificateHelper.ExtractKeyInstance(_spineOptionsDelegate.CurrentValue.ClientPrivateKey);
//                var x509ClientCertificate = new X509Certificate2(clientCertData.FirstOrDefault());

//                var privateKey = RSA.Create();
//                privateKey.ImportRSAPrivateKey(clientPrivateKeyData, out _);
//                var x509CertificateWithPrivateKey = x509ClientCertificate.CopyWithPrivateKey(privateKey);
//                var pfxFormattedCertificate = new X509Certificate(x509CertificateWithPrivateKey.Export(X509ContentType.Pfx, string.Empty), string.Empty);

//                _clientCertificate = pfxFormattedCertificate;
//            }
//        }

//        protected static X509Certificate SelectLocalCertificate(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
//        {
//            return _clientCertificate;
//        }

//        protected static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
//        {
//            if (sslPolicyErrors == SslPolicyErrors.None)
//                return true;

//            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors)
//                return true;

//            _logger.LogError($"An error has occurred while attempting to validate the LDAP server certificate: {sslPolicyErrors}");
//            return true;
//        }
//    }
//}
