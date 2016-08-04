﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fr8.Infrastructure.Data.Control;
using Fr8.Infrastructure.Data.Crates;
using Fr8.Infrastructure.Data.DataTransferObjects;
using Fr8.Infrastructure.Data.Managers;
using Fr8.Infrastructure.Data.Manifests;
using Fr8.Infrastructure.Data.States;
using Fr8.TerminalBase.BaseClasses;
using Fr8.TerminalBase.Infrastructure;
using PhoneNumbers;
using Twilio;
using terminalUtilities.Twilio;


namespace terminalTwilio.Activities
{
    public class Send_Via_Twilio_v1 : ExplicitTerminalActivity
    {
        public static ActivityTemplateDTO ActivityTemplateDTO = new ActivityTemplateDTO
        {
            Id = new Guid("ddd5be71-a23c-41e3-baf0-501e34f0517b"),
            Name = "Send_Via_Twilio",
            Label = "Send SMS Using Twilio Account",
            Tags = "Twillio,Notifier",
            Category = ActivityCategory.Forwarders,
            Version = "1",
            MinPaneWidth = 330,
            Terminal = TerminalData.TerminalDTO,
            WebService = TerminalData.WebServiceDTO,
            Categories = new[]
            {
                ActivityCategories.Forward,
                new ActivityCategoryDTO(TerminalData.WebServiceDTO.Name, TerminalData.WebServiceDTO.IconPath)
            }
        };
        protected override ActivityTemplateDTO MyTemplate => ActivityTemplateDTO;


        protected ITwilioService Twilio;

        public Send_Via_Twilio_v1(ICrateManager crateManager, ITwilioService twilioService)
            : base(crateManager)
        {
            Twilio = twilioService;
        }

        public override async Task Initialize()
        {
            Storage.Clear();
            PackCrate_ConfigurationControls();
        }
        

        private void PackCrate_ConfigurationControls()
        {
            var fieldsDTO = new List<ControlDefinitionDTO>()
            {
                UiBuilder.CreateSpecificOrUpstreamValueChooser("SMS Number", "SMS_Number", "Upstream Terminal-Provided Fields", "", addRequestConfigEvent: true),
                UiBuilder.CreateSpecificOrUpstreamValueChooser("SMS Body", "SMS_Body", "Upstream Terminal-Provided Fields", "", addRequestConfigEvent: true)
            };

            AddControls(fieldsDTO);
        }

        public override async Task FollowUp()
        {
          
        }

        public override async Task Run()
        {
            Message curMessage;

            try
            {
                var smsFieldDTO = ParseSMSNumberAndMsg();
                string smsNumber = smsFieldDTO.Key;
                string smsBody = smsFieldDTO.Value + "\nThis message was generated by Fr8. http://www.fr8.co";

                try
                {
                    curMessage = Twilio.SendSms(smsNumber, smsBody);
                    //SendEventReport($"Twilio SMS Sent -> SMSBody: {smsBody} smsNumber: {smsNumber}");
                    var curFieldDTOList = CreateKeyValuePairList(curMessage);
                    Payload.Add(PackCrate_TwilioMessageDetails(curFieldDTOList));
                }
                catch (Exception ex)
                {
                   // SendEventReport($"TwilioSMSSendFailure -> SMSBody: {smsBody} smsNumber: {smsNumber}, Exception {ex.Message}");
                    PackCrate_WarningMessage(ex.Message, "Twilio Service Failure");
                    RaiseError("Twilio Service Failure");
                    return;
                }
            }
            catch (ArgumentException appEx)
            {
                PackCrate_WarningMessage(appEx.Message, "SMS Number");
                RaiseError(appEx.Message);
                return;
            }

            Success();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="crateDTO"></param>
        /// <returns>Key = SMS Number; Value = SMS Body</returns>
        private KeyValueDTO ParseSMSNumberAndMsg()
        {
            var smsNumber = GetSMSNumber((TextSource)ConfigurationControls.Controls[0], Payload);
            var smsBody = GetSMSBody((TextSource)ConfigurationControls.Controls[1], Payload);
            return new KeyValueDTO(smsNumber, smsBody);
        }

        protected override Task Validate()
        {
            ValidationManager.Reset();

            if (ConfigurationControls?.Controls?.Count > 0)
            {
                var numberControl = (TextSource) ConfigurationControls.Controls[0];
                var bodyControl = (TextSource) ConfigurationControls.Controls[1];

                if (numberControl != null)
                {
                    if (numberControl.HasValue)
                    {
                        if (numberControl.HasSpecificValue)
                        {
                            ValidationManager.ValidatePhoneNumber(GeneralisePhoneNumber(numberControl.TextValue), numberControl);
                        }
                    }
                    else ValidationManager.SetError("No SMS Number Provided", numberControl);
                }
                if (bodyControl != null)
                {
                    if (!bodyControl.HasValue)
                    {
                        ValidationManager.SetError("SMS body can not be null.", bodyControl);
                    }
                }
            }
            else
            {
                ValidationManager.SetError("Configuration controls are missing.");
            }

            return Task.FromResult(0);
        }


        private string GetSMSNumber(TextSource control, ICrateStorage payloadCrates)
        {
            string smsNumber = "";
            if (control == null)
            {
                throw new ApplicationException("TextSource control was expected but not found.");
            }
            smsNumber = control.TextValue.Trim();

            smsNumber = GeneralisePhoneNumber(smsNumber);

            return smsNumber;
        }

        private string GetSMSBody(TextSource control, ICrateStorage payloadCrates)
        {
            string smsBody = "";
            if (control == null)
            {
                throw new ApplicationException("TextSource control was expected but not found.");
            }

            smsBody = control.TextValue;
            if (smsBody == null)
            {
                throw new ArgumentException("SMS body can not be null.");
            }


            return smsBody;
        }

        private List<KeyValueDTO> CreateKeyValuePairList(Message curMessage)
        {
            List<KeyValueDTO> returnList = new List<KeyValueDTO>();
            returnList.Add(new KeyValueDTO("Status", curMessage.Status));
            returnList.Add(new KeyValueDTO("ErrorMessage", curMessage.ErrorMessage));
            returnList.Add(new KeyValueDTO("Body", curMessage.Body));
            returnList.Add(new KeyValueDTO("ToNumber", curMessage.To));
            return returnList;
        }
        private Crate PackCrate_TwilioMessageDetails(List<KeyValueDTO> curTwilioMessage)
        {
            return Crate.FromContent("Message Data", new StandardPayloadDataCM(curTwilioMessage));
        }

        private void PackCrate_WarningMessage(string warningMessage, string warningLabel)
        {
            Storage.Clear();

            var textBlock = UiBuilder.GenerateTextBlock(warningLabel, warningMessage, "alert alert-warning");

            AddControls(textBlock);
        }

        private string GeneralisePhoneNumber(string smsNumber)
        {
            PhoneNumberUtil phoneUtil = PhoneNumberUtil.GetInstance();
            smsNumber = new string(smsNumber.Where(s => char.IsDigit(s) || s == '+' || (phoneUtil.IsAlphaNumber(smsNumber) && char.IsLetter(s))).ToArray());
            if (smsNumber.Length == 10 && !smsNumber.Contains("+"))
                smsNumber = "+1" + smsNumber; //we assume that default region is USA
            return smsNumber;
        }
    }
}