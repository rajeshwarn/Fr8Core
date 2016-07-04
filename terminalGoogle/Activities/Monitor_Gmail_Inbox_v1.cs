﻿using Fr8.Infrastructure.Data.Manifests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using terminalGoogle.Actions;
using Fr8.Infrastructure.Data.DataTransferObjects;
using System.Threading.Tasks;
using Fr8.Infrastructure.Data.States;
using Fr8.Infrastructure.Data.Managers;
using terminalGoogle.Interfaces;
using terminalGoogle.Services;
using Newtonsoft.Json;
using terminalGoogle.DataTransferObjects;
using Fr8.Infrastructure.Data.Crates;
using Fr8.Infrastructure.Data.Constants;
using Fr8.Infrastructure.Data.Control;

namespace terminalGoogle.Activities
{
    public class Monitor_Gmail_Inbox_v1 : BaseGoogleTerminalActivity<Monitor_Gmail_Inbox_v1.ActivityUi>
    {

        public Monitor_Gmail_Inbox_v1(ICrateManager crateManager, IGoogleIntegration googleIntegration, IGoogleGmailPolling _gmailPolling)
            : base(crateManager, googleIntegration)
        {
            _gmailPollingService = _gmailPolling;
        }

        public static ActivityTemplateDTO ActivityTemplateDTO = new ActivityTemplateDTO
        {
            Name = "Monitor_Gmail_Inbox",
            Label = "Monitor Gmail Inbox",
            Version = "1",
            Category = ActivityCategory.Monitors,
            Terminal = TerminalData.TerminalDTO,
            NeedsAuthentication = true,
            WebService = TerminalData.GmailWebServiceDTO,
            MinPaneWidth = 300,
            Categories = new[] { ActivityCategories.Monitor }
        };

        private IGoogleGmailPolling _gmailPollingService;

        protected override ActivityTemplateDTO MyTemplate => ActivityTemplateDTO;

        private readonly string RuntimeCrateLabel = "Email fields";

        public async override Task Initialize()
        {
            Storage.Remove<EventSubscriptionCM>();
            Storage.Add
                (CrateManager.CreateStandardEventSubscriptionsCrate(
                "Standard Event Subscriptions",
                "Google",
                "GmailInbox"));

            CrateSignaller.MarkAvailableAtRuntime<StandardEmailMessageCM>(RuntimeCrateLabel);
        }


        public override async Task Activate()
        {
            await _gmailPollingService.SchedulePolling(HubCommunicator, AuthorizationToken.ExternalAccountId, true);
        }

        public override Task Run()
        {
            StandardEmailMessageCM mail = null;
            var eventCrate = Payload.CratesOfType<EventReportCM>().FirstOrDefault()?.Get<EventReportCM>()?.EventPayload;
            if (eventCrate != null)
                mail = eventCrate.CrateContentsOfType<StandardEmailMessageCM>().SingleOrDefault();

            if (mail == null)
            {
                TerminateHubExecution("Letter was not found in the payload.");
            }

            Payload.Add(Crate.FromContent(RuntimeCrateLabel, mail));

            Success();

            return Task.FromResult(0);
        }


        public async override Task FollowUp()
        {
        }

        public class ActivityUi : StandardConfigurationControlsCM
        {
            public TextBlock infoBlock;

            public ActivityUi()
            {
                infoBlock = new TextBlock()
                {
                    Value = "This Activity doesn't require any configuration. Once the plan is activated this activity will launch a plan everytime a new email message appears in the inbox."
                };
                Controls.Add(infoBlock);
            }
        }
    }
}