﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Fr8Data.Constants;
using Fr8Data.Control;
using Fr8Data.Crates;
using Fr8Data.DataTransferObjects;
using Fr8Data.Managers;
using Fr8Data.Manifests;
using Fr8Data.Manifests.Helpers;
using Fr8Data.States;
using terminalUtilities.Excel;
using TerminalBase.BaseClasses;
using TerminalBase.Infrastructure;
using TerminalBase.Services;

namespace terminalExcel.Actions
{
    public class Save_To_Excel_v1 : EnhancedTerminalActivity<Save_To_Excel_v1.ActivityUi>
    {
        public class ActivityUi : StandardConfigurationControlsCM
        {
            public CrateChooser UpstreamCrateChooser { get; set; }

            public RadioButtonGroup SpreadsheetSelectionGroup { get; set; }

            public RadioButtonOption UseNewSpreadsheetOption { get; set; }

            public TextBox NewSpreadsheetName { get; set; }

            public RadioButtonOption UseExistingSpreadsheetOption { get; set; }

            public DropDownList ExistingSpreadsheetsList { get; set; }

            public RadioButtonGroup WorksheetSelectionGroup { get; set; }

            public RadioButtonOption UseNewWorksheetOption { get; set; }

            public TextBox NewWorksheetName { get; set; }

            public RadioButtonOption UseExistingWorksheetOption { get; set; }

            public DropDownList ExistingWorksheetsList { get; set; }

            public ActivityUi(UiBuilder builder)
            {
                UpstreamCrateChooser = builder.CreateCrateChooser(
                        "Available_Crates",
                        "This Loop will process the data inside of",
                        true,
                        requestConfig: true
                    );

                Controls.Add(UpstreamCrateChooser);
                NewSpreadsheetName = new TextBox
                                     {
                                         Value = $"NewFr8Data{DateTime.Now.Date:dd-MM-yyyy}",
                                         Name = nameof(NewSpreadsheetName)
                                     };
                ExistingSpreadsheetsList = new DropDownList
                                           {
                                               Name = nameof(ExistingSpreadsheetsList),
                                               Events = new List<ControlEvent> { ControlEvent.RequestConfig }
                                           };
                UseNewSpreadsheetOption = new RadioButtonOption
                                          {
                                              Selected = true,
                                              Name = nameof(UseNewSpreadsheetOption),
                                              Value = "Store in a new Excel Spreadsheet",
                                              Controls = new List<ControlDefinitionDTO> { NewSpreadsheetName }
                                          };
                UseExistingSpreadsheetOption = new RadioButtonOption()
                                               {
                                                   Selected = false,
                                                   Name = nameof(UseExistingSpreadsheetOption),
                                                   Value = "Store in an existing Spreadsheet",
                                                   Controls = new List<ControlDefinitionDTO> { ExistingSpreadsheetsList }
                                               };
                SpreadsheetSelectionGroup = new RadioButtonGroup
                                            {
                                                GroupName = nameof(SpreadsheetSelectionGroup),
                                                Name = nameof(SpreadsheetSelectionGroup),
                                                Events = new List<ControlEvent> { ControlEvent.RequestConfig },
                                                Radios = new List<RadioButtonOption>
                                                         {
                                                             UseNewSpreadsheetOption,
                                                             UseExistingSpreadsheetOption
                                                         }
                                            };
                Controls.Add(SpreadsheetSelectionGroup);
                NewWorksheetName = new TextBox
                                   {
                                       Value = "Sheet1",
                                       Name = nameof(NewWorksheetName)
                                   };
                ExistingWorksheetsList = new DropDownList
                                         {
                                             Name = nameof(ExistingWorksheetsList),
                                         };
                UseNewWorksheetOption = new RadioButtonOption()
                                        {
                                            Selected = true,
                                            Name = nameof(UseNewWorksheetOption),
                                            Value = "A new Sheet (Pane)",
                                            Controls = new List<ControlDefinitionDTO> { NewWorksheetName }
                                        };
                UseExistingWorksheetOption = new RadioButtonOption()
                                             {
                                                 Selected = false,
                                                 Name = nameof(UseExistingWorksheetOption),
                                                 Value = "Existing Pane",
                                                 Controls = new List<ControlDefinitionDTO> { ExistingWorksheetsList }
                                             };
                WorksheetSelectionGroup = new RadioButtonGroup()
                                          {
                                              Label = "Inside the spreadsheet, store in",
                                              GroupName = nameof(WorksheetSelectionGroup),
                                              Name = nameof(WorksheetSelectionGroup),
                                              Radios = new List<RadioButtonOption>
                                                       {
                                                           UseNewWorksheetOption,
                                                           UseExistingWorksheetOption
                                                       }
                                          };
                Controls.Add(WorksheetSelectionGroup);
            }
        }

        private const string SelectedSpreadsheetCrateLabel = "Selected Spreadsheet";


        public Save_To_Excel_v1(ICrateManager crateManager)
            : base(false, crateManager)
        {
        }

        public override async Task Initialize()
        {
            CrateSignaller.MarkAvailableAtRuntime<StandardFileDescriptionCM>("StoredFile");
            ActivityUI.ExistingSpreadsheetsList.ListItems = await GetCurrentUsersFiles();
        }

        public override async Task FollowUp()
        {
            //If different existing spreadsheet is selected then we have to load worksheet list for it
            if (ActivityUI.UseExistingSpreadsheetOption.Selected && !string.IsNullOrEmpty(ActivityUI.ExistingSpreadsheetsList.Value))
            {
                var previousSpreadsheet = SelectedSpreadsheet;
                if (string.IsNullOrEmpty(previousSpreadsheet) || !string.Equals(previousSpreadsheet, ActivityUI.ExistingSpreadsheetsList.Value))
                {
                    ActivityUI.ExistingWorksheetsList.ListItems = (
                        await GetWorksheets(
                            int.Parse(ActivityUI.ExistingSpreadsheetsList.Value),
                            ActivityUI.ExistingSpreadsheetsList.selectedKey)
                        )
                        .Select(x => new ListItem { Key = x.Value, Value = x.Key })
                        .ToList();
                    var firstWorksheet = ActivityUI.ExistingWorksheetsList.ListItems.First();
                    ActivityUI.ExistingWorksheetsList.SelectByValue(firstWorksheet.Value);
                }
                SelectedSpreadsheet = ActivityUI.ExistingSpreadsheetsList.Value;
            }
            else
            {
                ActivityUI.ExistingWorksheetsList.ListItems.Clear();
                ActivityUI.ExistingWorksheetsList.selectedKey = string.Empty;
                ActivityUI.ExistingWorksheetsList.Value = string.Empty;
                SelectedSpreadsheet = string.Empty;
            }
        }

        private async Task<List<ListItem>> GetWorksheets(int fileId, string fileName)
        {
            //let's download this file
            Stream file = await HubCommunicator.DownloadFile(fileId);
            var fileBytes = ExcelUtils.StreamToByteArray(file);

            //TODO: Optimize this to retrieve spreadsheet list only. Now it reads and loads into memory whole file. 
            var spreadsheetList = ExcelUtils.GetSpreadsheets(fileBytes, Path.GetExtension(fileName));
            return spreadsheetList.Select(s => new ListItem() { Key = s.Key.ToString(), Value = s.Value }).ToList();
        }

        private string SelectedSpreadsheet
        {
            get
            {
                var storedValue = Storage.FirstCrateOrDefault<FieldDescriptionsCM>(x => x.Label == SelectedSpreadsheetCrateLabel);
                return storedValue?.Content.Fields.First().Key;
            }
            set
            {
                Storage.RemoveByLabel(SelectedSpreadsheetCrateLabel);
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }
                Storage.Add(Crate<FieldDescriptionsCM>.FromContent(SelectedSpreadsheetCrateLabel, new FieldDescriptionsCM(new FieldDTO(value)), AvailabilityType.Configuration));
            }
        }
        public static ActivityTemplateDTO ActivityTemplateDTO = new ActivityTemplateDTO
        {
            Name = "Save_To_Excel",
            Label = "Save to Excel",
            Version = "1",
            Category = ActivityCategory.Forwarders,
            Terminal = TerminalData.TerminalDTO,
            MinPaneWidth = 300,
            WebService = TerminalData.WebServiceDTO
        };
        protected override ActivityTemplateDTO MyTemplate => ActivityTemplateDTO;

        public override async Task Run()
        {
            if (!ActivityUI.UpstreamCrateChooser.CrateDescriptions.Any(x => x.Selected))
            {
                RaiseError($"Failed to run {ActivityTemplateDTO.Name} because upstream crate is not selected", ActivityErrorCode.DESIGN_TIME_DATA_MISSING);
            }
            if ((ActivityUI.UseNewSpreadsheetOption.Selected && string.IsNullOrWhiteSpace(ActivityUI.NewSpreadsheetName.Value))
                || (ActivityUI.UseExistingSpreadsheetOption.Selected && string.IsNullOrEmpty(ActivityUI.ExistingSpreadsheetsList.Value)))
            {
                RaiseError($"Failed to run {ActivityTemplateDTO.Name} because spreadsheet name is not specified", ActivityErrorCode.DESIGN_TIME_DATA_MISSING);
            }
            if ((ActivityUI.UseNewWorksheetOption.Selected && string.IsNullOrWhiteSpace(ActivityUI.NewWorksheetName.Value))
                || (ActivityUI.UseExistingWorksheetOption.Selected && string.IsNullOrEmpty(ActivityUI.ExistingWorksheetsList.Value)))
            {
                RaiseError($"Failed to run {ActivityTemplateDTO.Name} because worksheet name is not specified", ActivityErrorCode.DESIGN_TIME_DATA_MISSING);
            }
            var crateToProcess = FindCrateToProcess();
            if (crateToProcess == null)
            {
                RaiseError($"Failed to run {ActivityTemplateDTO.Name} because specified upstream crate was not found in payload");
            }

            var tableToSave = StandardTableDataCMTools
                .ExtractPayloadCrateDataToStandardTableData(crateToProcess);
            var url = await AppendOrCreateSpreadsheet(tableToSave);
            await PushLaunchURLNotification(url);
        }

        private async Task<string> AppendOrCreateSpreadsheet(StandardTableDataCM tableToSave)
        {
            byte[] fileData;
            string fileName;

            if (ActivityUI.UseNewSpreadsheetOption.Selected)
            {
                fileData = ExcelUtils.CreateExcelFile(
                    tableToSave,
                    ActivityUI.NewWorksheetName.Value
                );

                fileName = ActivityUI.NewSpreadsheetName.Value;
            }
            else
            {
                var existingFileStream = await HubCommunicator.DownloadFile(
                    Int32.Parse(ActivityUI.ExistingSpreadsheetsList.Value)
                );

                byte[] existingFileBytes;
                using (var memStream = new MemoryStream())
                {
                    await existingFileStream.CopyToAsync(memStream);
                    existingFileBytes = memStream.ToArray();
                }

                fileName = ActivityUI.ExistingSpreadsheetsList.selectedKey;

                var worksheetName = ActivityUI.UseNewWorksheetOption.Selected
                    ? ActivityUI.NewWorksheetName.Value
                    : ActivityUI.ExistingWorksheetsList.selectedKey;

                StandardTableDataCM dataToInsert;
                if (ActivityUI.UseExistingWorksheetOption.Selected
                    || ActivityUI.ExistingWorksheetsList.ListItems.Any(x => x.Key == ActivityUI.NewWorksheetName.Value))
                {
                    var existingData = ExcelUtils.GetExcelFile(existingFileBytes, fileName, true, worksheetName);

                    StandardTableDataCMTools.AppendToStandardTableData(existingData, tableToSave);
                    dataToInsert = existingData;
                }
                else
                {
                    dataToInsert = tableToSave;
                }

                fileData = ExcelUtils.RewriteSheetForFile(
                    existingFileBytes,
                    dataToInsert,
                    worksheetName
                );
            }

            using (var stream = new MemoryStream(fileData, false))
            {
                if (!fileName.ToUpper().EndsWith(".XLSX"))
                {
                    fileName += ".xlsx";
                }

                var file = await HubCommunicator.SaveFile(fileName, stream);
                Payload.Add(Crate.FromContent("StoredFile", new StandardFileDescriptionCM
                {
                    Filename = file.Id.ToString(), // dirty hack
                    TextRepresentation = file.OriginalFileName, // another hack
                    Filetype = ".xlsx"
                }));
				return file.CloudStorageUrl;
            }
        }

        private async Task<List<ListItem>> GetCurrentUsersFiles()
        {
            var curAccountFileList = await HubCommunicator.GetFiles();
            //TODO where tags == Docusign files
            return curAccountFileList.Select(c => new ListItem() { Key = c.OriginalFileName, Value = c.Id.ToString(CultureInfo.InvariantCulture) }).ToList();
        }

        private Crate FindCrateToProcess()
        {
            var desiredCrateDescription = ActivityUI.UpstreamCrateChooser.CrateDescriptions.Single(x => x.Selected);
            return Payload.FirstOrDefault(x => x.Label == desiredCrateDescription.Label && x.ManifestType.Type == desiredCrateDescription.ManifestType);
        }

        private async Task PushLaunchURLNotification(string url)
        {
            await PushUserNotification("Success", "Excel File", $"The Excel file can be downloaded by navigating to this URL: {new Uri(url).AbsoluteUri}");
        }
    }
}