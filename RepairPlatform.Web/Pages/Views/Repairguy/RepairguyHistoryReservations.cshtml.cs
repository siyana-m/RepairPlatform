using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RepairPlatform.Services.DTO.Reservations;
using RepairPlatform.Services;
using RepairPlatform.Services.DTO.Repairguys;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using RepairPlatform.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using System.Drawing;

namespace RepairPlatform.Web.Pages.Views.Repairguy
{
    [Authorize(Policy = "RequireRepairGuyRole")]
    public class RepairguyHistoryReservationsModel : PageModel
    {
        private readonly ReservationsService _reservationsService;
        private readonly UserManager<AspNetUsers> _userManager;
        private readonly RepairguysService _repairguysService;
        private readonly Repairguy20118046Context _dbContext;

        public RepairguyHistoryReservationsModel(UserManager<AspNetUsers> userManager, ReservationsService reservationsService, RepairguysService repairguysService, Repairguy20118046Context dbContext)
        {
            _reservationsService = reservationsService;
            _userManager = userManager;
            _repairguysService = repairguysService;
            _dbContext = dbContext;
        }

        public List<ReservationDto>? Reservations { get; set; }
        public RepairguyDto? Repairguy { get; set; }

        public int EditReservationId { get; set; } = 0;

        public async Task<IActionResult> OnGetAsync(string? status, string? sortOption)
        {
            await LoadReservationsAsync(status, sortOption).ConfigureAwait(false);
            return Page();
        }

        private async Task LoadReservationsAsync(string? status, string? sortOption)
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user == null) return;

            var repairguyDto = await _repairguysService.GetByUserId(user.Id).ConfigureAwait(false);
            if (repairguyDto == null) return;

            var reservations = await _reservationsService.GetReservationsExportForRepairguyAsync(repairguyDto.RepairguyId, status, sortOption).ConfigureAwait(false);

            Reservations = reservations.Select(r => new ReservationDto
            {
                ReservationId = r.ReservationId,
                ResName = r.ResName,
                Client = r.Client,
                Group = r.Group,
                ResDateTime = r.ResDateTime,
                ResLocation = r.ResLocation,
                ResComment = r.ResComment,
                ResStatus = r.ResStatus ?? "В очакване"
            }).ToList();
        }

        public SelectList GetStatusOptions(string status)
        {
            return new SelectList(new[]
            {
                new SelectListItem { Value = "Приета", Text = "Приета" },
                new SelectListItem { Value = "Отхвърлена", Text = "Отхвърлена" },
                new SelectListItem { Value = "Завършена", Text = "Завършена" }
            }, "Value", "Text", status);
        }

        public string GetStatusClass(string status)
        {
            return status switch
            {
                "Приета" => "status-accepted",
                "Отхвърлена" => "status-declined",
                "Завършена" => "status-completed",
                _ => "status-pending",
            };
        }

        public async Task<IActionResult> OnPostChangeReservationStatusAsync(int id, string status)
        {
            await _reservationsService.ChangeReservationStatus(id, status).ConfigureAwait(false);
            await LoadReservationsAsync(null, null).ConfigureAwait(false);
            EditReservationId = 0;
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditReservationAsync(int id)
        {
            EditReservationId = id;
            await LoadReservationsAsync(null, null).ConfigureAwait(false);
            return Page();
        }

        public async Task<IActionResult> OnGetExportReservationsToExcel(string? status, string? sortOption)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            var repairguy = await _dbContext.Repairguys.FirstOrDefaultAsync(rg => rg.UserId == user.Id);

            if (repairguy == null)
            {
                return NotFound("Repairguy not found");
            }

            var reservations = await _reservationsService.GetReservationsExportForRepairguyAsync(repairguy.RepairguyId, status, sortOption).ConfigureAwait(false);

            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Reservations");

                worksheet.Cells[1, 1].Value = $"Експорт на данни за резервации на {repairguy.RfirstName} {repairguy.RlastName} към {DateTime.Now}: ";
                worksheet.Cells[1, 1, 1, 8].Merge = true;
                worksheet.Cells[1, 1, 1, 8].Style.Font.Bold = true;
                worksheet.Cells[1, 1, 1, 8].Style.Font.Size = 16;

                worksheet.Cells[3, 1].Value = "№ на Резервация";
                worksheet.Cells[3, 2].Value = "Име на Клиент";
                worksheet.Cells[3, 3].Value = "Категория";
                worksheet.Cells[3, 4].Value = "Име на Резервация";
                worksheet.Cells[3, 5].Value = "Дата и Час";
                worksheet.Cells[3, 6].Value = "Адрес";
                worksheet.Cells[3, 7].Value = "Коментар";
                worksheet.Cells[3, 8].Value = "Статус";

                using (var range = worksheet.Cells[3, 1, 3, 8])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Font.Size = 14;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    range.AutoFitColumns();
                }

                for (int i = 0; i < reservations.Count; i++)
                {
                    var reservation = reservations[i];
                    worksheet.Cells[i + 4, 1].Value = reservation.ReservationId;
                    worksheet.Cells[i + 4, 2].Value = $"{reservation.Client.CfirstName} {reservation.Client.ClastName}";
                    worksheet.Cells[i + 4, 3].Value = reservation.Group.CatName;
                    worksheet.Cells[i + 4, 4].Value = reservation.ResName;
                    worksheet.Cells[i + 4, 5].Value = reservation.ResDateTime.ToString("g");
                    worksheet.Cells[i + 4, 6].Value = reservation.ResLocation;
                    worksheet.Cells[i + 4, 7].Value = reservation.ResComment;
                    worksheet.Cells[i + 4, 8].Value = string.IsNullOrEmpty(reservation.ResStatus) ? "В очакване" : reservation.ResStatus;
                    worksheet.Cells[i + 4, 1, i + 4, 8].Style.Font.Size = 12;

                    var statusCell = worksheet.Cells[i + 4, 8];
                    statusCell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    switch (reservation.ResStatus ?? "В очакване")
                    {
                        case "Приета":
                            statusCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            statusCell.Style.Fill.BackgroundColor.SetColor(Color.Green);
                            statusCell.Style.Font.Color.SetColor(Color.White);
                            break;
                        case "Отхвърлена":
                            statusCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            statusCell.Style.Fill.BackgroundColor.SetColor(Color.Red);
                            statusCell.Style.Font.Color.SetColor(Color.White);
                            break;
                        case "Завършена":
                            statusCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            statusCell.Style.Fill.BackgroundColor.SetColor(Color.Blue);
                            statusCell.Style.Font.Color.SetColor(Color.White);
                            break;
                        case "В очакване":
                            statusCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            statusCell.Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                            statusCell.Style.Font.Color.SetColor(Color.Black);
                            break;
                    }
                }

                using (var range = worksheet.Cells[3, 1, 3 + reservations.Count, 8])
                {
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }

                for (int col = 1; col <= 8; col++)
                {
                    worksheet.Cells[3, col].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells[3, col].Style.Border.Left.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells[3, col].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells[3, col].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;

                    worksheet.Cells[3 + reservations.Count, col].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                    for (int row = 3; row <= 3 + reservations.Count; row++)
                    {
                        worksheet.Cells[row, col].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                    }
                }

                for (int row = 3; row <= 3 + reservations.Count; row++)
                {
                    worksheet.Cells[row, 1].Style.Border.Left.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells[row, 8].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                }

                worksheet.Cells[3, 1, 3, 8].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                worksheet.Cells[3 + reservations.Count, 1, 3 + reservations.Count, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                package.Save();
            }
            stream.Position = 0;
            var fileName = $"Reservations_{repairguy.RfirstName}_{repairguy.RlastName}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(stream, contentType, fileName);
        }

    }
}


