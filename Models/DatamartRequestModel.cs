using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadsSaver_RabbitMQ.Models
{
        public class DatamartRequestModel
        {
            public string type { get; set; }
            public string crmLeadId { get; set; }
            public string idDealerCenter { get; set; }
            public string Ref_Key { get; set; }
            public bool DeletionMark { get; set; }
            public string crmDealerCenterId { get; set; }
            public DateTime crmDateTimeMoskvichReceive { get; set; }
            public DateTime crmProcessingDeadline { get; set; }
            public DateTime crmPersonalDataConsentDate { get; set; }
            public DateTime crmMarketingCommAgreementDate { get; set; }
            public string crmLeadSource { get; set; }
            public string crmClientType { get; set; }
            public string crmLeadType { get; set; }
            public string crmLeadSubtype { get; set; }
            public string crmFirstName { get; set; }
            public string crmLastName { get; set; }
            public string crmMiddleName { get; set; }
            public string crmPhone { get; set; }
            public string crmEmail { get; set; }
            public string crmBrand { get; set; }
            public string crmModelOfInterest { get; set; }
            public string crmRequestedVIN { get; set; }
            public string crmLeadText { get; set; }
            public string crmLeadLink { get; set; }
            public string crmDealerNameWithAddress { get; set; }
            public string crmAdditionalField1 { get; set; }
            public string crmAdditionalField2 { get; set; }
            public string crmClientCompanyName { get; set; }
            public string crmClientTIN { get; set; }
            public string crmClientCompanyType { get; set; }
            public string crmClientCompanyActivitySphere { get; set; }
            public string crmOwnedBrand { get; set; }
            public string crmOwnedModel { get; set; }
            public int crmOwnedYear { get; set; }
            public string crmOwnedEngine { get; set; }
            public string crmOwnedDriveType { get; set; }
            public string crmOwnedTransmission { get; set; }
            public int crmOwnedMileage { get; set; }
            public string crmOwnedVIN { get; set; }
            public string crmOwnedLicenceNumber { get; set; }
            public string crmCustomerServiceComment { get; set; }
            public int onlineServiceEstimatedTime { get; set; }
            public DateTime onlineServiceVehicleDropDateTime { get; set; }
            public string onlineServiceManagerId { get; set; }
            public string onlineServiceWorks { get; set; }
            public string onlineServiceRequestId { get; set; }
            public string onlineServiceZoneId { get; set; }
            public string techIdentClientDealerSiteYaGa { get; set; }
            public string techAdditionalAnalyticsField { get; set; }
            public DateTime crmProcessingDeadlineMoscow { get; set; }
        }
}
