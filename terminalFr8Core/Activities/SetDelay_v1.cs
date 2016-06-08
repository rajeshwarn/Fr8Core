﻿using System;
using System.Threading.Tasks;
using fr8.Infrastructure.Data.Control;
using fr8.Infrastructure.Data.Crates;
using fr8.Infrastructure.Data.DataTransferObjects;
using fr8.Infrastructure.Data.Managers;
using fr8.Infrastructure.Data.States;
using TerminalBase;
using TerminalBase.BaseClasses;

namespace terminalFr8Core.Activities
{
    public class SetDelay_v1 : BaseTerminalActivity
    {
        public static ActivityTemplateDTO ActivityTemplateDTO = new ActivityTemplateDTO
        {
            Name = "SetDelay",
            Label = "Delay Action Processing",
            Category = ActivityCategory.Processors,
            Version = "1",
            MinPaneWidth = 330,
            Type = ActivityType.Standard,
            WebService = TerminalData.WebServiceDTO,
            Terminal = TerminalData.TerminalDTO
        };
        protected override ActivityTemplateDTO MyTemplate => ActivityTemplateDTO;

        private const int MinDurationSeconds = 10;
        private AlarmDTO CreateAlarm(TimeSpan duration)
                {
            if (duration.TotalSeconds == 0)
            {
                duration.Add(TimeSpan.FromSeconds(MinDurationSeconds));
            }
            return new AlarmDTO
            {
                ContainerId = ExecutionContext.ContainerId,
                StartTime = DateTime.UtcNow.Add(duration)
            };
        }
        private TimeSpan GetUserDefinedDelayDuration()
        {
            var manifestTypeDropdown = GetControl<Duration>("Delay_Duration");
            if (manifestTypeDropdown.Value == null)
            {
                throw new TerminalCodedException(TerminalErrorCode.PAYLOAD_DATA_MISSING, "Delay activity can't create a delay without a selected duration on design time");
            }
            return manifestTypeDropdown.Value;
        }

        private Crate CreateControlsCrate()
        {
            var duration = new Duration
            {
                Label = "Please enter delay duration",
                Name = "Delay_Duration"
            };

            return PackControlsCrate(duration);
        }

        public SetDelay_v1(ICrateManager crateManager)
            : base(crateManager)
        {
        }

        public override async Task Run()
        {
            //find our action state in operations crate
            var delayState = OperationalState.CallStack.GetLocalData<string>("Delay");
            //extract ActivityResponse type from result
            if (delayState == "suspended")
            {
                //this is second time we are being called. this means alarm has triggered
                Success();
                return;
            }

            //get user selected design time duration
            var delayDuration = GetUserDefinedDelayDuration();
            var alarmDTO = CreateAlarm(delayDuration);
            //post to hub to create an alarm
            await HubCommunicator.CreateAlarm(alarmDTO);

            OperationalState.CallStack.StoreLocalData("Delay", "suspended");

            SuspendHubExecution();
            }

        public override Task Initialize()
            {
            var configurationControlsCrate = CreateControlsCrate();
            Storage.Add(configurationControlsCrate);
            return Task.FromResult(0);
            }

        public override Task FollowUp()
        {
            return Task.FromResult(0);
        }
    }
}