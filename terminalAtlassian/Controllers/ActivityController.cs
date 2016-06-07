﻿using System.Web.Http;
using System.Threading.Tasks;
using System;
using fr8.Infrastructure.Data.DataTransferObjects;
using StructureMap;
using TerminalBase.Services;

namespace terminalAtlassian.Controllers
{
    [RoutePrefix("activities")]
    public class ActivityController: ApiController
    {
        private const string curTerminal = "terminalAtlassian";
        private readonly ActivityExecutor _activityExecutor;

        public ActivityController()
        {
            _activityExecutor = ObjectFactory.GetInstance<ActivityExecutor>();
        }

        [HttpPost]
        public Task<object> Execute([FromUri] String actionType, [FromBody] Fr8DataDTO curDataDTO)
        {
            return _activityExecutor.HandleFr8Request(curTerminal, actionType, curDataDTO);
        }
    }
}