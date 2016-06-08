﻿using System.Web.Http;
using System.Web.Http.Description;
using fr8.Infrastructure.Data.Manifests;
using TerminalBase.Services;

namespace terminalSalesforce.Controllers
{
    // [RoutePrefix("terminals")]
    public class TerminalController : ApiController
    {
        [HttpGet]
        [Route("discover")]
        [ResponseType(typeof(StandardFr8TerminalCM))]
        public IHttpActionResult Get()
        {
            StandardFr8TerminalCM curStandardFr8TerminalCM = new StandardFr8TerminalCM()
            {
                Definition = TerminalData.TerminalDTO,
                Activities = ActivityStore.GetAllActivities(TerminalData.TerminalDTO)
            };

            return Json(curStandardFr8TerminalCM);
        }
    }
}