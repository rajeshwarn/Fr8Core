﻿using Fr8Data.DataTransferObjects;
using Fr8Data.States;
using Utilities.Configuration.Azure;

namespace terminalSlack
{
    public static class TerminalData
    {
        public static WebServiceDTO WebServiceDTO = new WebServiceDTO
        {
            Name = "Slack",
            IconPath = "/Content/icons/web_services/slack-icon-64x64.png"
        };

        public static TerminalDTO TerminalDTO = new TerminalDTO
        {
            Endpoint = CloudConfigurationManager.GetSetting("terminalSlack.TerminalEndpoint"),
            TerminalStatus = TerminalStatus.Active,
            Name = "terminalSlack",
            Label = "Slack",
            Version = "1",
            AuthenticationType = AuthenticationType.External
        };
    }
}