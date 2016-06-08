﻿using fr8.Infrastructure.Data.DataTransferObjects;
using fr8.Infrastructure.Data.States;
using fr8.Infrastructure.Utilities.Configuration;

namespace terminalSendGrid
{
    public static class TerminalData
    {
        public static WebServiceDTO WebServiceDTO = new WebServiceDTO
        {
            Name = "SendGrid"
        };

        public static TerminalDTO TerminalDTO = new TerminalDTO
        {
            Name = "terminalSendGrid",
            Label = "SendGrid",
            TerminalStatus = TerminalStatus.Active,
            Endpoint = CloudConfigurationManager.GetSetting("terminalSendGrid.TerminalEndpoint"),
            Version = "1"
        };
    }
}