﻿using Data.Interfaces;
using Data.Interfaces.DataTransferObjects;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Data.Wrappers
{
    public class DocuSignEnvelope : DocuSign.Integrations.Client.Envelope, IEnvelope
    {
        private string _baseUrl;
        private readonly ITab _tab;
        private readonly ISigner _signer;

        public DocuSignEnvelope()
        {
            //TODO change baseUrl later. Remove it to constructor parameter etc.
            _baseUrl = string.Empty; 

            //TODO move ioc container.
            _tab = new Tab();
            _signer = new Signer();
        }

        public DocuSignEnvelope(DocuSign.Integrations.Client.Envelope envelope)
        {

        }

        /// <summary>
        /// Get Envelope Data from a docusign envelope. 
        /// Each EnvelopeData row is essentially a specific DocuSign "Tab".
        /// </summary>
        /// <param name="envelope">DocuSign.Integrations.Client.Envelope envelope domain.</param>
        /// <returns>
        /// List of Envelope Data.
        /// It returns empty list of envelope data if tab and signers not found.
        /// </returns>
        public List<EnvelopeDataDTO> GetEnvelopeData(DocuSignEnvelope envelope)
        {
            Signer[] curSignersSet = _signer.GetFromRecipients(envelope);
            if (curSignersSet != null)
            {
                foreach (Signer curSigner in curSignersSet)
                {
                    return _tab.ExtractEnvelopeData(envelope, curSigner);
                }
            }

            return new List<EnvelopeDataDTO>();
        }

        /// <summary>
        /// Get Envelope Data from a docusign envelope. 
        /// Each EnvelopeData row is essentially a specific DocuSign "Tab".
        /// </summary>
        /// <param name="curEnvelopeId">DocuSign.Integrations.Client.Envelope envelope id.</param>
        /// <returns>
        /// List of Envelope Data.
        /// It returns empty list of envelope data if tab and signers not found.
        /// </returns>
        public List<EnvelopeDataDTO> GetEnvelopeData(string curEnvelopeId)
        {
            if (String.IsNullOrEmpty(curEnvelopeId))
            {
                throw new ArgumentNullException("envelopeId");
            }

            using (var uow = ObjectFactory.GetInstance<IUnitOfWork>())
            {
                var curDocusignEnvelope = new DocuSignEnvelope();
                curDocusignEnvelope.EnvelopeId = curEnvelopeId;
                curDocusignEnvelope.GetRecipients();
                return GetEnvelopeData(curDocusignEnvelope);
            }
        }
    }
}
