﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Internal;
using Newtonsoft.Json;
using StructureMap;
using Data.Control;
using Data.Crates;
using Data.Interfaces;
using Data.Interfaces.DataTransferObjects;
using Data.Interfaces.Manifests;
using Data.Repositories.MultiTenant;
using Data.States;
using TerminalBase.BaseClasses;
using TerminalBase.Services;
using TerminalBase.Services.MT;

namespace terminalFr8Core.Actions
{
    public class GetDataFromFr8Warehouse_v1
        : EnhancedTerminalActivity<GetDataFromFr8Warehouse_v1.ActivityUi>
    {
        public class ActivityUi : StandardConfigurationControlsCM
        {
            public DropDownList AvailableObjects { get; set; }

            public TextBlock SelectObjectLabel { get; set; }

            public QueryBuilder QueryBuilder { get; set; }

            public ActivityUi()
            {
                AvailableObjects = new DropDownList()
                {
                    Label = "Object List",
                    Name = "AvailableObjects",
                    Value = null,
                    Events = new List<ControlEvent> { ControlEvent.RequestConfig },
                    Source = null
                };
                Controls.Add(AvailableObjects);

                SelectObjectLabel = new TextBlock()
                {
                    Value = "Please select object before specifying the query.",
                    Name = "SelectObjectLabel",
                    CssClass = "well well-lg",
                    IsHidden = false
                };
                Controls.Add(SelectObjectLabel);

                QueryBuilder = new QueryBuilder()
                {
                    Label = "Find all Fields where:",
                    Name = "QueryBuilder",
                    Required = true,
                    Source = new FieldSourceDTO
                    {
                        Label = "Queryable Criteria",
                        ManifestType = CrateManifestTypes.StandardDesignTimeFields
                    },
                    IsHidden = true
                };
                Controls.Add(QueryBuilder);
            }
        }


        private const string RunTimeCrateLabel = "Table Generated by Get Data From Fr8 Warehouse";

        public GetDataFromFr8Warehouse_v1() : base(false)
        {
        }

        protected override async Task Initialize(RuntimeCrateManager runtimeCrateManager)
        {
            ConfigurationControls.AvailableObjects.ListItems = GetObjects();
            runtimeCrateManager.MarkAvailableAtRuntime<StandardTableDataCM>(RunTimeCrateLabel);

            await Task.Yield();
        }

        protected override async Task Configure(RuntimeCrateManager runtimeCrateManager)
        {
            var selectedObject = ConfigurationControls.AvailableObjects.Value;
            var hasSelectedObject = !string.IsNullOrEmpty(selectedObject);
            if (hasSelectedObject)
            {
                Guid selectedObjectId;
                if (Guid.TryParse(ConfigurationControls.AvailableObjects.Value, out selectedObjectId))
                {
                    CurrentActivityStorage.ReplaceByLabel(
                        Crate.FromContent(
                            "Queryable Criteria",
                            new FieldDescriptionsCM(MTTypesHelper.GetFieldsByTypeId(selectedObjectId, AvailabilityType.RunTime))
                        )
                    );
                }
            }

            ConfigurationControls.QueryBuilder.IsHidden = !hasSelectedObject;
            ConfigurationControls.SelectObjectLabel.IsHidden = hasSelectedObject;
            runtimeCrateManager.MarkAvailableAtRuntime<StandardTableDataCM>(RunTimeCrateLabel);

            await Task.Yield();
        }

        protected override async Task RunCurrentActivity()
        {
            using (var uow = ObjectFactory.GetInstance<IUnitOfWork>())
            {
                var selectedObjectId = Guid.Parse(ConfigurationControls.AvailableObjects.Value);
                var mtType = uow.MultiTenantObjectRepository.FindTypeReference(selectedObjectId);
                if (mtType == null)
                {
                    throw new ApplicationException("Invalid object selected.");
                }

                var conditions = JsonConvert.DeserializeObject<List<FilterConditionDTO>>(
                    ConfigurationControls.QueryBuilder.Value
                );
                
                var manifestType = mtType.ClrType;
                var queryBuilder = MTSearchHelper.CreateQueryProvider(manifestType);
                var converter = CrateManifestToRowConverter(manifestType);
                var foundObjects = queryBuilder
                    .Query(
                        uow,
                        AuthorizationToken.UserID,
                        conditions
                    )
                    .ToArray();
                

                var searchResult = new StandardTableDataCM();

                if (foundObjects.Length > 0)
                {
                    searchResult.FirstRowHeaders = true;

                    var headerRow = new TableRowDTO();

                    var properties = uow.MultiTenantObjectRepository.ListTypePropertyReferences(mtType.Id);
                    foreach (var mtTypeProp in properties)
                    {
                        headerRow.Row.Add(
                            new TableCellDTO()
                            {
                                Cell = new FieldDTO(mtTypeProp.Name, mtTypeProp.Name)
                            });
                    }

                    searchResult.Table.Add(headerRow);
                }

                foreach (var foundObject in foundObjects)
                {
                    searchResult.Table.Add(converter(foundObject));
                }

                CurrentPayloadStorage.Add(
                    Crate.FromContent(
                        RunTimeCrateLabel,
                        searchResult
                    )
                );
            }

            await Task.Yield();
        }

        private Func<object, TableRowDTO> CrateManifestToRowConverter(Type manifestType)
        {
            var accessors = new List<KeyValuePair<string, IMemberAccessor>>();

            foreach (var member in manifestType.GetMembers(BindingFlags.Instance | BindingFlags.Public).OrderBy(x => x.Name))
            {
                IMemberAccessor accessor;

                if (member is FieldInfo)
                {
                    accessor = ((FieldInfo)member).ToMemberAccessor();
                }
                else if (member is PropertyInfo && !((PropertyInfo)member).IsSpecialName)
                {
                    accessor = ((PropertyInfo)member).ToMemberAccessor();
                }
                else
                {
                    continue;
                }

                accessors.Add(new KeyValuePair<string, IMemberAccessor>(member.Name, accessor));
            }

            return x =>
            {
                var row = new TableRowDTO();

                foreach (var accessor in accessors)
                {
                    row.Row.Add(
                        new TableCellDTO()
                        {
                            Cell = new FieldDTO(accessor.Key, string.Format(CultureInfo.InvariantCulture, "{0}", accessor.Value.GetValue(x)))
                        }
                    );
                }

                return row;
            };
        }

        private List<ListItem> GetObjects()
        {
            using (var uow = ObjectFactory.GetInstance<IUnitOfWork>())
            {
                var listTypeReferences = uow.MultiTenantObjectRepository.ListTypeReferences();
                return listTypeReferences
                    .Select(c =>
                        new ListItem()
                        {
                            Key = c.Alias,
                            Value = c.Id.ToString("N")
                        })
                    .ToList();
            }
        }
    }
}