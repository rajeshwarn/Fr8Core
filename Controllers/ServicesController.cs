﻿using Data.Interfaces.DataTransferObjects;
using Hub.Interfaces;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HubWeb.Controllers
{
    public class ServicesController : Controller
    {
        private readonly ITerminal _terminal ;
        public ServicesController ()
        {
            _terminal = ObjectFactory.GetInstance<ITerminal>();
        }
        private async Task<List<SolutionPageDTO>> getDocumentationSolutionList(string terminalName)
        {
            var solutionNameList = await _terminal.GetSolutionDocumentations(terminalName);
            return solutionNameList;
        }
        public async Task<ActionResult> DocuSign()
        {
            var solutionList = await getDocumentationSolutionList("terminalDocuSign");
            return View(solutionList);
        }

        public ActionResult HowItWorks()
        {
            return View();
        }

        public async Task<ActionResult> Salesforce()
        {
            var solutionList = await getDocumentationSolutionList("terminalSalesforce");
            return View(solutionList);
        }

        public async Task<ActionResult> GoogleApps()
        {
            await getDocumentationSolutionList("GoogleApps");
            return View();
        }
    }
}