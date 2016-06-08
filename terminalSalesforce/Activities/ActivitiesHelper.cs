﻿using System.Collections.Generic;
using System.Linq;
using fr8.Infrastructure.Data.Control;
using fr8.Infrastructure.Data.Crates;
using fr8.Infrastructure.Data.DataTransferObjects;
using fr8.Infrastructure.Data.Manifests;
using fr8.Infrastructure.Data.States;

namespace terminalSalesforce.Actions
{
    public static class ActivitiesHelper
    {
        private static List<ListItem> FillSalesforceSupportedObjects()
        {
            var fields =
            new FieldDTO[]
                {
                    new FieldDTO("Account") { Availability = AvailabilityType.Configuration},
                    new FieldDTO("Contact") { Availability = AvailabilityType.Configuration},
                    new FieldDTO("Lead") { Availability = AvailabilityType.Configuration},
                    new FieldDTO("Opportunity") { Availability = AvailabilityType.Configuration},
                    //new FieldDTO("Forecast") {Availability = AvailabilityType.Configuration},
                    new FieldDTO("Contract") { Availability = AvailabilityType.Configuration},
                    new FieldDTO("Order") { Availability = AvailabilityType.Configuration},
                    new FieldDTO("Case") { Availability = AvailabilityType.Configuration},
                    new FieldDTO("Solution") { Availability = AvailabilityType.Configuration},
                    new FieldDTO("Product2") { Availability = AvailabilityType.Configuration},
                    new FieldDTO("Document") { Availability = AvailabilityType.Configuration}
                    //new FieldDTO("File") {Availability = AvailabilityType.Configuration}
                };
            return fields.Select(x => new ListItem() { Key = x.Key, Value = x.Key }).ToList();
            
        }

        public static void GetAvailableFields(Crate configurationCrate, string controlName)
        {
            var configurationControl = configurationCrate.Get<StandardConfigurationControlsCM>();
            GetAvailableFields(configurationControl.FindByNameNested<DropDownList>(controlName));
        }

        public static void GetAvailableFields(DropDownList dropDownControl)
        {
            if (dropDownControl != null)
            {
                dropDownControl.ListItems = FillSalesforceSupportedObjects();
            }
        }

        public static IDictionary<string, object> GenerateSalesforceObjectDictionary(IEnumerable<FieldDTO> fieldsList, 
                                                                                     IEnumerable<TextSource> fieldControlsList, 
                                                                                    ICrateStorage payloadStorage)
        {
            var jsonInputObject = new Dictionary<string, object>();
            fieldsList.ToList().ForEach(field =>
            {
                var jsonKey = field.Key;
                var jsonValue = fieldControlsList.Single(ts => ts.Name.Equals(jsonKey)).GetValue(payloadStorage);

                if (!string.IsNullOrEmpty(jsonValue))
                {
                    jsonInputObject.Add(jsonKey, jsonValue);
                }
                else if(field.IsRequired)
                {
                    jsonInputObject.Add(jsonKey, "Not Available");
                }
            });
            return jsonInputObject;
        }
    }
}