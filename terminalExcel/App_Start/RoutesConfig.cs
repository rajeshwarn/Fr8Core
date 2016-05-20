﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using StructureMap;
using TerminalBase.BaseClasses;

namespace terminalExcel
{
    public static class RoutesConfig
    {
        public static void Register(HttpConfiguration config)
        {
            BaseTerminalWebApiConfig.Register("Excel", config);            
        }
    }
}