﻿using System.Xml.Serialization;

namespace Data.Entities.DocuSignParserModels
{
    [XmlRoot(ElementName = "EnvelopeStatus")]
    public class EnvelopeStatus
    {
        [XmlElement("EnvelopeID")]
        public string EnvelopeId { get; set; }

        [XmlElement("Status")]
        public string Status { get; set; }

        [XmlElement("RecipientStatuses")]
        public RecipientStatuses RecipientStatuses { get; set; }

        [XmlElement("Email")]
        public string ExternalAccountId { get; set; }

        [XmlElement("Created")]
        public string CreatedDate { get; set; }

        [XmlElement("Sent")]
        public string SentDate { get; set; }

        [XmlElement("Delivered")]
        public string DeliveredDate { get; set; }

        [XmlElement("Completed")]
        public string CompletedDate { get; set; }
    }
}