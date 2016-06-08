﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;
using fr8.Infrastructure.Data.DataTransferObjects;
using fr8.Infrastructure.Data.Manifests;
using Microsoft.Owin.Hosting;
using Owin;

namespace terminalFr8CoreTests.Fixtures
{
    partial class FixtureData
    {
        public static readonly string CoreEndPoint = "http://localhost:30643";

        public class ActivitiesController_ControllerTypeResolver : IHttpControllerTypeResolver
        {
            public ICollection<Type> GetControllerTypes(IAssembliesResolver assembliesResolver)
            {
                return new Type[] {
                    typeof(HubWeb.Controllers.ManifestsController)
                };
            }
        }

        public class ActivitiesController_SelfHostStartup
        {
            public void Configuration(IAppBuilder app)
            {
                var config = new HttpConfiguration();

                // Web API routes
                //config.MapHttpAttributeRoutes();
                config.Routes.MapHttpRoute(
                    name: "DefaultApiWithAction",
                    routeTemplate: "api/v1/{controller}/{action}/{id}",
                    defaults: new { id = RouteParameter.Optional }
                );
                config.Routes.MapHttpRoute(
                    name: "DefaultApiGet",
                    routeTemplate: "api/v1/{controller}/{id}",
                    defaults: new { id = RouteParameter.Optional, action = "Get" },
                    constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
                    );
                config.Routes.MapHttpRoute(
                    name: "DefaultApiPost",
                    routeTemplate: "api/v1/{controller}",
                    defaults: new { action = "Post" },
                    constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
                    );
                config.Routes.MapHttpRoute(
                    name: "DefaultApiPut",
                    routeTemplate: "api/v1/{controller}",
                    defaults: new { action = "Put" },
                    constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) }
                    );
                config.Routes.MapHttpRoute(
                    name: "DefaultApiDelete",
                    routeTemplate: "api/v1/{controller}/{id}",
                    defaults: new { id = RouteParameter.Optional, action = "Delete" },
                    constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) }
                    );

                config.Services.Replace(
                    typeof(IHttpControllerTypeResolver),
                    new ActivitiesController_ControllerTypeResolver()
                );

                app.UseWebApi(config);
            }
        }

        public static IDisposable CreateCoreServer_ActivitiesController()
        {
            return WebApp.Start<ActivitiesController_SelfHostStartup>(url: CoreEndPoint);
        }

        public static Fr8DataDTO ConnectToSql_InitialConfiguration_Fr8DataDTO()
        {
            var activityTemplate = ConnectToSql_ActivityTemplate();

            var activityDTO = new ActivityDTO()
            {
                Id = Guid.NewGuid(),
                Label = "ConnectToSql Fr8Core",
                ActivityTemplate = activityTemplate
            };

            return new Fr8DataDTO { ActivityDTO = activityDTO };
        }

        public static ActivityTemplateDTO ConnectToSql_ActivityTemplate()
        {
            return new ActivityTemplateDTO()
            {
                Id = Guid.NewGuid(),
                Name = "ConnectToSql_TEST",
                Version = "1"
            };
        }

        public static Fr8DataDTO ExecuteSql_InitialConfiguration_Fr8DataDTO()
        {
            var activityTemplate = ExecuteSql_ActivityTemplate();

            var activityDTO = new ActivityDTO()
            {
                Id = Guid.NewGuid(),
                Label = "ExecuteSql Fr8Core",
                ActivityTemplate = activityTemplate
            };

            return new Fr8DataDTO { ActivityDTO = activityDTO };
        }

        public static ActivityTemplateDTO ExecuteSql_ActivityTemplate()
        {
            return new ActivityTemplateDTO()
            {
                Id = Guid.NewGuid(),
                Name = "ExecuteSql_TEST",
                Version = "1"
            };
        }

        public static Fr8DataDTO SaveToFr8Wareouse_InitialConfiguration_Fr8DataDTO()
        {
            var activityTemplate = SaveToFr8Wareouse_ActivityTemplate();

            var activityDTO = new ActivityDTO()
            {
                Id = Guid.NewGuid(),
                Label = "ExecuteSql Fr8Core",
                ActivityTemplate = activityTemplate
            };

            return new Fr8DataDTO { ActivityDTO = activityDTO };
        }

        public static ActivityTemplateDTO SaveToFr8Wareouse_ActivityTemplate()
        {
            return new ActivityTemplateDTO()
            {
                Id = Guid.NewGuid(),
                Name = "SaveToFr8Warehouse_TEST",
                Version = "1"
            };
        }

        public static ActivityTemplateDTO MonitorFr8Event_ActivityTemplate()
        {
            return new ActivityTemplateDTO()
            {
                Id = Guid.NewGuid(),
                Name = "Monitor_Fr8_Events_TEST",
                Version = "1"
            };
        }

        public static ActivityDTO MonitorFr8Event_InitialConfiguration_ActivityDTO()
        {
            var activityTemplate = MonitorFr8Event_ActivityTemplate();

            return new ActivityDTO()
            {
                Id = Guid.NewGuid(),
                Label = "Monitor Fr8 Events",
                ActivityTemplate = activityTemplate
            };
        }

        public static Fr8DataDTO Loop_InitialConfiguration_ActivityDTO()
        {
            var activityTemplate = Loop_ActivityTemplate();

            var activityDTO = new ActivityDTO()
            {
                Id = Guid.NewGuid(),
                Label = "Loop",
                ActivityTemplate = activityTemplate
            };
            return new Fr8DataDTO() { ActivityDTO = activityDTO };
        }

        private static ActivityTemplateDTO Loop_ActivityTemplate()
        {
            return new ActivityTemplateDTO()
            {
                Id = Guid.NewGuid(),
                Name = "Loop_TEST",
                Version = "1"
            };
        }

        public static StandardTableDataCM StandardTableData_Test1()
        {
            var headerRow = new TableRowDTO()
            {
                Row = new List<TableCellDTO>()
                {
                    new TableCellDTO()
                    {
                        Cell = new FieldDTO("1", "Date")
                    },
                    new TableCellDTO()
                    {
                        Cell = new FieldDTO("2", "Description")
                    },
                    new TableCellDTO()
                    {
                        Cell = new FieldDTO("3", "Phone")
                    },
                    new TableCellDTO()
                    {
                        Cell = new FieldDTO("4", "Travelling")
                    }
                }
            };
            var dataRow1 = new TableRowDTO()
            {
                Row = new List<TableCellDTO>()
                {
                    new TableCellDTO()
                    {
                        Cell = new FieldDTO("5", "30-Dec-2015")
                    },
                    new TableCellDTO()
                    {
                        Cell = new FieldDTO("6", "Trip to Samarkand")
                    },
                    new TableCellDTO()
                    {
                        Cell = new FieldDTO("7", "70")
                    },
                    new TableCellDTO()
                    {
                        Cell = new FieldDTO("8", "90")
                    }
                }
            };
            return new StandardTableDataCM
            {
                FirstRowHeaders = true,
                Table = new List<TableRowDTO> { headerRow, dataRow1 }
            };
        }
    }
}
