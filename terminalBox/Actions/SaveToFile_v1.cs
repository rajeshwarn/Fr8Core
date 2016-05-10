﻿using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Data.Control;
using Data.Crates;
using Data.Interfaces.DataTransferObjects;
using Data.Interfaces.Manifests;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;
using terminalBox.Infrastructure;
using TerminalBase.BaseClasses;

namespace terminalBox.Actions
{
    public class SaveToFile_v1 : EnhancedTerminalActivity<SaveToFile_v1.ActivityUi>
    {
        public class ActivityUi : StandardConfigurationControlsCM
        {
            public CrateChooser FileChooser { get; set; }
            public TextBox Filename { get; set; }

            public ActivityUi(UiBuilder uiBuilder)
            {
                Controls.Add(FileChooser = uiBuilder.CreateCrateChooser("FileChooser", "Select file to store:", true, true));
                /*Controls.Add(Filename = new DropDownList()
                {
                    Label = "Select where to store file",
                    Name = "Filename"
                });*/

                Controls.Add(Filename = new TextBox()
                { Name = "Filename", Label = "Enter the file name: " });
            }
        }

        public SaveToFile_v1()
            : base(true)
        {
        }

        /*private async Task FillAvailableFolders()
        {
            var token = JsonConvert.DeserializeObject<BoxAuthTokenDO>(AuthorizationToken.Token);
            var folders = await new BoxService().ListFolders(token);

            ConfigurationControls.Filename.ListItems = folders.Select(x => new ListItem()
            {
                Key = x.Value,
                Value = x.Key
            }).ToList();
        }*/

        protected override async Task Initialize(CrateSignaller crateSignaller)
        {
            //await FillAvailableFolders();
        }

        protected override async Task Configure(CrateSignaller crateSignaller)
        {
            //await FillAvailableFolders();
        }

        protected override async Task RunCurrentActivity()
        {
            var token = JsonConvert.DeserializeObject<BoxAuthTokenDO>(AuthorizationToken.Token);
            var desiredCrateDescription = ConfigurationControls.FileChooser.CrateDescriptions.Single(x => x.Selected);
            var tableCrate = CurrentPayloadStorage.CratesOfType<StandardTableDataCM>().FirstOrDefault(x => x.Label == desiredCrateDescription.Label && x.ManifestType.Type == desiredCrateDescription.ManifestType);

            if (tableCrate == null)
            {
                Error($"Selected crate {desiredCrateDescription.Label} doesn't contains table data");
                return;
            }
            var fileName = ConfigurationControls.Filename.Value;
            var service = new BoxService(token);
            string fileId;
            using (var stream = new MemoryStream())
            {
                CreateSpreadsheetWorkbook(stream);
                // Need to reset stream before saving it to box to prevent errors
                stream.Seek(0, SeekOrigin.Begin);
                fileId = service.SaveFile(fileName + ".xlsx", stream).Result;
            }
            var downloadLink = service.GetFileLink(fileId).Result;
            
            await HubCommunicator.NotifyUser(new TerminalNotificationDTO
            {
                Type = "Success",
                ActivityName = "SaveToFile",
                ActivityVersion = "1",
                TerminalName = "terminalBox",
                TerminalVersion = "1",
                Message = "File was upload to Box. You can download it using this url: " + downloadLink,
                Subject = "File download URL"
            }, CurrentFr8UserId);
        }

        public void CreateSpreadsheetWorkbook(MemoryStream stream)
        {
            // Create a spreadsheet document 
            // By default, AutoSave = true, Editable = true, and Type = xlsx.
            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(stream,
                SpreadsheetDocumentType.Workbook);

            // Add a WorkbookPart to the document.
            WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();

            // Add a WorksheetPart to the WorkbookPart.
            WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            // Add Sheets to the Workbook.
            Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.
                AppendChild<Sheets>(new Sheets());

            // Append a new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet()
            {
                Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "mySheet"
            };
            sheets.Append(sheet);

            workbookpart.Workbook.Save();

            // Close the document.
            spreadsheetDocument.Close();
        }
    }
}