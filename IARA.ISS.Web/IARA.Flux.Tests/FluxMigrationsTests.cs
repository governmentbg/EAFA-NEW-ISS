using System;
using System.Linq;
using System.Collections.Generic;
using IARA.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using TL.SysToSysSecCom.Abstractions.Utils;
using IARA.Flux.Models;
using IARA.FluxModels.Enums;
using Infragistics.Documents.Excel;
using System.Drawing;
using Microsoft.EntityFrameworkCore;

namespace IARA.Flux.Tests
{
    internal static class FluxMigrationsTests
    {
        public static void RunFromDb(IServiceProvider serviceProvider, DateTime dateFrom, DateTime dateTo, string filename)
        {
            IARADbContext db = serviceProvider.GetRequiredService<IARADbContext>();

            var requests = (from fvms in db.Fluxfvmsrequests
                            where fvms.RequestDateTime >= dateFrom
                              && fvms.RequestDateTime <= dateTo
                            orderby fvms.RequestDateTime
                            select new
                            {
                                fvms.RequestDateTime,
                                fvms.RequestContent
                            }).ToList();

            Dictionary<string, List<MigrationTrip>> trips = new();
            List<string> noTripUuids = new();

            foreach (var request in requests)
            {
                FLUXFAReportMessageType report = CommonUtils.JsonDeserialize<FLUXFAReportMessageType>(request.RequestContent);
                string requestUuid = report.FLUXReportDocument.ID.Single(x => x.schemeID == "UUID").Value;
                string tripId = report.FAReportDocument?[0]?.SpecifiedFishingActivity?[0]?.SpecifiedFishingTrip?.ID?.Single(x => x.schemeID == "EU_TRIP_ID")?.Value;

                if (string.IsNullOrEmpty(tripId))
                {
                    noTripUuids.Add(requestUuid);
                }
                else
                {
                    if (trips.TryGetValue(tripId, out List<MigrationTrip> values))
                    {
                        values.Add(new MigrationTrip(tripId, requestUuid, request.RequestDateTime, report));
                    }
                    else
                    {
                        trips[tripId] = new List<MigrationTrip> { new MigrationTrip(tripId, requestUuid, request.RequestDateTime, report) };
                    }
                }
            }

            var tripPages = (from far in db.FvmsfishingActivityReports
                                .AsSplitQuery()
                             join purpose in db.MdrFluxGpPurposes on far.MdrFluxGpPurposeId equals purpose.Id
                             join faType in db.MdrFluxFaTypes on far.MdrFluxFaTypeId equals faType.Id
                             join ship in db.ShipsRegister on far.VesselId equals ship.Id
                             where trips.Keys.Contains(far.TripIdentifier)
                             select new
                             {
                                 far.TripIdentifier,
                                 Data = new
                                 {
                                     UUID = far.ResponseUuid.ToString(),
                                     CFR = ship.Cfr,
                                     ShipName = ship.Name,
                                     ExtMark = ship.ExternalMark,
                                     Page = string.Join(Environment.NewLine, from farlbp in db.FvmsfishingActivityReportLogBookPages
                                                                             join page in db.ShipLogBookPages on farlbp.ShipLogBookPageId equals page.Id
                                                                             where farlbp.FishingActivityReportId == far.Id
                                                                             select $"{page.PageNum} ({page.Status})"),
                                     Errors = string.Join(Environment.NewLine, from error in db.ErrorLogs
                                                                               where error.Class.Contains("FluxIntegrations")
                                                                                   && error.LogDate >= far.CreatedOn.AddSeconds(-3)
                                                                                   && error.LogDate <= far.CreatedOn.AddSeconds(+3)
                                                                                   && !error.Message.Contains("failed to send message")
                                                                                   && !error.Message.Contains("response ended prematurely")
                                                                                   && !error.Message.Contains("reset by peer")
                                                                               select error.Id + " - " + error.Message)
                                 }
                             }).ToLookup(x => x.TripIdentifier, y => y.Data);

            foreach (KeyValuePair<string, List<MigrationTrip>> trip in trips)
            {
                var pages = tripPages[trip.Key].ToList();

                foreach (MigrationTrip migrationTrip in trip.Value)
                {
                    var page = pages.FirstOrDefault(x => x.UUID == migrationTrip.UUID);

                    if (page != null)
                    {
                        migrationTrip.CFR = page.CFR;
                        migrationTrip.ShipName = page.ShipName;
                        migrationTrip.ExternalMark = page.ExtMark;
                        migrationTrip.Page = page.Page;
                        migrationTrip.Errors = page.Errors;
                    }
                }

                trips[trip.Key] = trip.Value.OrderBy(x => x.RequestDate).ToList();
            }

            ExportToExcel(trips, noTripUuids, filename);
        }

        private static void ExportToExcel(Dictionary<string, List<MigrationTrip>> trips, List<string> noTripUuids, string filename)
        {
            Workbook workbook = new Workbook(WorkbookFormat.Excel2007);
            Worksheet worksheet = workbook.Worksheets.Add("Sheet1");

            Dictionary<int, int> maxCellsSize = new();

            int row = 0;

            foreach (KeyValuePair<string, List<MigrationTrip>> trip in trips)
            {
                foreach (MigrationTrip migrationTrip in trip.Value)
                {
                    migrationTrip.Page ??= "";
                    migrationTrip.Errors ??= "";

                    WorksheetCell c0 = worksheet.Rows[row].Cells[0];
                    c0.Value = $"{migrationTrip.ShipName} ({migrationTrip.CFR}|{migrationTrip.ExternalMark})";
                    AddOrUpdateCellSize(maxCellsSize, 0, c0.Value.ToString().Length);

                    WorksheetCell c1 = worksheet.Rows[row].Cells[1];
                    c1.Value = migrationTrip.TripIdentifier;
                    AddOrUpdateCellSize(maxCellsSize, 1, c1.Value.ToString().Length);

                    WorksheetCell c2 = worksheet.Rows[row].Cells[2];
                    c2.Value = migrationTrip.RequestDate.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    AddOrUpdateCellSize(maxCellsSize, 2, c2.Value.ToString().Length);

                    WorksheetCell c3 = worksheet.Rows[row].Cells[3];
                    c3.Value = migrationTrip.UUID;
                    AddOrUpdateCellSize(maxCellsSize, 3, c3.Value.ToString().Length);

                    WorksheetCell c4 = worksheet.Rows[row].Cells[4];
                    c4.Value = $"{string.Join("|", migrationTrip.FaTypes.Select(x => x.ToString()))} ({(int?)migrationTrip.Purpose}|{(int?)migrationTrip.RelatedPurpose})";
                    AddOrUpdateCellSize(maxCellsSize, 4, c4.Value.ToString().Length);

                    WorksheetCell c5 = worksheet.Rows[row].Cells[5];
                    c5.Value = string.Join("|", migrationTrip.SubFaTypes.Select(x => x.ToString()));
                    AddOrUpdateCellSize(maxCellsSize, 5, c5.Value.ToString().Length);

                    WorksheetCell c6 = worksheet.Rows[row].Cells[6];
                    c6.Value = migrationTrip.Page;

                    if (migrationTrip.Page.Contains(Environment.NewLine))
                    {
                        AddOrUpdateCellSize(maxCellsSize, 6, c6.Value.ToString().Length / 2);
                    }
                    else
                    {
                        AddOrUpdateCellSize(maxCellsSize, 6, c6.Value.ToString().Length);
                    }

                    WorksheetCell c7 = worksheet.Rows[row].Cells[7];
                    c7.Value = migrationTrip.Errors;

                    if (migrationTrip.Page.Contains(Environment.NewLine))
                    {
                        AddOrUpdateCellSize(maxCellsSize, 7, c7.Value.ToString().Length / 2);
                    }
                    else
                    {
                        AddOrUpdateCellSize(maxCellsSize, 7, c7.Value.ToString().Length);
                    }

                    ++row;
                }

                bool error = false;

                if (trip.Value.Last().FaTypes.Any(x => x == FaTypes.LANDING) && string.IsNullOrEmpty(trip.Value.Last().Page))
                {
                    error = true;
                }

                List<WorksheetCell> cells = worksheet.Rows[row - 1].Cells.Where(x => x.ColumnIndex >= 0 && x.ColumnIndex <= 7).ToList();
                foreach (WorksheetCell cell in cells)
                {
                    cell.CellFormat.BottomBorderStyle = CellBorderLineStyle.Thick;

                    if (error)
                    {
                        cell.CellFormat.Fill = CellFill.CreateSolidFill(Color.Red);
                    }
                }
            }

            foreach (KeyValuePair<int, int> cellDefinition in maxCellsSize)
            {
                int cellWidth = cellDefinition.Value + 3;

                if (cellDefinition.Key == 6)
                {
                    worksheet.Columns[cellDefinition.Key].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                    worksheet.Columns[cellDefinition.Key].SetWidth(cellWidth, WorksheetColumnWidthUnit.Character);
                    worksheet.Columns[cellDefinition.Key].AutoFitWidth();
                }
                else
                {
                    worksheet.Columns[cellDefinition.Key].CellFormat.WrapText = ExcelDefaultableBoolean.False;
                    worksheet.Columns[cellDefinition.Key].SetWidth(cellWidth, WorksheetColumnWidthUnit.Character);
                }
            }

            workbook.Save(filename);
        }

        private static void AddOrUpdateCellSize(Dictionary<int, int> maxCellsSize, int cell, int size)
        {
            if (maxCellsSize.ContainsKey(cell))
            {
                maxCellsSize[cell] = Math.Max(maxCellsSize[cell], size);
            }
            else
            {
                maxCellsSize.Add(cell, size);
            }
        }

        private class MigrationTrip
        {
            public DateTime RequestDate { get; set; }

            public string UUID { get; set; }

            public string TripIdentifier { get; set; }

            public FluxPurposes? Purpose { get; set; }

            public FluxPurposes? RelatedPurpose { get; set; }

            public FaReportTypes FaReportType { get; set; }

            public List<FaTypes> FaTypes { get; set; } = new();

            public List<FaTypes> SubFaTypes { get; set; } = new();

            public string CFR { get; set; }

            public string ShipName { get; set; }

            public string ExternalMark { get; set; }

            public string Page { get; set; }

            public string Errors { get; set; }

            public MigrationTrip(string tripId, string requestUuid, DateTime requestDate, FLUXFAReportMessageType report)
            {
                RequestDate = requestDate;
                UUID = requestUuid;
                TripIdentifier = tripId;

                if (!string.IsNullOrEmpty(report.FLUXReportDocument.PurposeCode?.Value))
                {
                    Purpose = (FluxPurposes)int.Parse(report.FLUXReportDocument.PurposeCode.Value);
                }

                if (!string.IsNullOrEmpty(report.FAReportDocument[0].RelatedFLUXReportDocument?.PurposeCode?.Value))
                {
                    RelatedPurpose = (FluxPurposes)int.Parse(report.FAReportDocument[0].RelatedFLUXReportDocument.PurposeCode.Value);
                }

                FaReportType = Enum.Parse<FaReportTypes>(report.FAReportDocument[0].TypeCode.Value);

                foreach (FishingActivityType fishingActivity in report.FAReportDocument[0].SpecifiedFishingActivity)
                {
                    FaTypes.Add(Enum.Parse<FaTypes>(fishingActivity.TypeCode.Value));

                    if (fishingActivity.RelatedFishingActivity != null)
                    {
                        foreach (FishingActivityType relatedFishingActivity in fishingActivity.RelatedFishingActivity)
                        {
                            SubFaTypes.Add(Enum.Parse<FaTypes>(relatedFishingActivity.TypeCode.Value));
                        }
                    }
                }
            }
        }
    }
}
