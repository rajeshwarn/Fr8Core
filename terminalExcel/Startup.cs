﻿using System;
using System.Collections.Generic;
using System.Web.Http.Dispatcher;
using Microsoft.Owin;
using Owin;
using TerminalBase.BaseClasses;
using TerminalBase.Services;
using terminalExcel.Actions;

[assembly: OwinStartup("TerminalExcelConfiguration", typeof(terminalExcel.Startup))]

namespace terminalExcel
{
    public class Startup : BaseConfiguration
    {
        public void Configuration(IAppBuilder app)
        {
            Configuration(app, false);
        }

        public void Configuration(IAppBuilder app, bool selfHost)
        {
            ConfigureProject(selfHost, TerminalExcelStructureMapRegistries.LiveConfiguration);
            RoutesConfig.Register(_configuration);

            app.UseWebApi(_configuration);

            if (!selfHost)
            {
                StartHosting("terminalExcel");
            }
        }

        public override ICollection<Type> GetControllerTypes(IAssembliesResolver assembliesResolver)
        {
            return new Type[] {
                    typeof(Controllers.ActivityController),
                    typeof(Controllers.EventController),
                    typeof(Controllers.TerminalController)
                };
        }
        protected override void RegisterActivities()
        {
            ActivityStore.RegisterActivity<Load_Excel_File_v1>(Load_Excel_File_v1.ActivityTemplateDTO);
            ActivityStore.RegisterActivity<Save_To_Excel_v1>(Save_To_Excel_v1.ActivityTemplateDTO);
            ActivityStore.RegisterActivity<SetExcelTemplate_v1>(SetExcelTemplate_v1.ActivityTemplateDTO);
        }
    }
}
