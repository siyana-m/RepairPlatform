//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using OfficeOpenXml;
//using OfficeOpenXml.Style;
//using RepairPlatform.Services;
//using RepairPlatform.Services.DTO.Repairguys;

//namespace RepairPlatform.Web.Pages.Views.Admin
//{
//    public class AdminUsersModel : PageModel
//    {
//        private readonly AdministratorsService _adminService;

//        public AdminUsersModel(AdministratorsService adminService)
//        {
//            _adminService = adminService;
//        }

//        public AdminUsersViewModel? AdminUsers { get; set; }
//        public class AdminUsersViewModel
//        {
//            public List<Entities.Repairguy>? Repairguys { get; set; }
//            public List<Entities.Client>? Clients { get; set; }

//            [BindProperty]
//            public int EditRepairguyId { get; set; }

//            [BindProperty]
//            public int EditClientId { get; set; }
//        }

//        public void OnGet(string? status, string? sortOption)
//        {
//            AdminUsers = new AdminUsersViewModel
//            {
//                Repairguys = _adminService.GetAllRepairguys(),
//                Clients = _adminService.GetAllClients()
//            };
//        }

//        public SelectList GetStatusOptions(string status)
//        {
//            return new SelectList(new[]
//            {
//                new SelectListItem { Value = "Active", Text = "Active" },
//                new SelectListItem { Value = "Inactive", Text = "Inactive" },
//            }, "Value", "Text", status);
//        }


//        public IActionResult OnPostChangeRepairguyStatus(int id, string status)
//        {
//            _adminService.ChangeRepairguyStatus(id, status);
//            return RedirectToPage();
//        }


//        public IActionResult OnPostChangeClientStatus(int id, string status)
//        {
//            _adminService.ChangeClientStatus(id, status);
//            return RedirectToPage();
//        }

//        public IActionResult OnPostSaveRepairguyStatus(int id, string status)
//        {
//            _adminService.ChangeRepairguyStatus(id, status);
//            AdminUsers!.EditRepairguyId = 0; // Reset edit mode
//            return RedirectToPage();
//        }

//        public IActionResult OnPostSaveClientStatus(int id, string status)
//        {
//            _adminService.ChangeClientStatus(id, status);
//            AdminUsers!.EditClientId = 0; // Reset edit mode
//            return RedirectToPage();
//        }


//        public IActionResult OnPostEditRepairguy(int id)
//        {
//            AdminUsers = new AdminUsersViewModel
//            {
//                EditRepairguyId = id,
//                Repairguys = _adminService.GetAllRepairguys(),
//                Clients = _adminService.GetAllClients()
//            };

//            return Page();
//        }

//        public IActionResult OnPostEditClient(int id)
//        {
//            AdminUsers = new AdminUsersViewModel
//            {
//                EditClientId = id,
//                Repairguys = _adminService.GetAllRepairguys(),
//                Clients = _adminService.GetAllClients()
//            };

//            return Page();
//        }

//        //public async Task<IActionResult> OnPostDeleteRepairguyAsync(int id)
//        //{
//        //    _adminService.DeleteRepairguy(id);

//        //    AdminUsers = new AdminUsersViewModel
//        //    {
//        //        EditRepairguyId = id,
//        //        Repairguys = _adminService.GetAllRepairguys(),
//        //        Clients = _adminService.GetAllClients()
//        //    };

//        //    return Page();
//        //}

//        //public async Task<IActionResult> OnPostDeleteClientAsync(int id)
//        //{
//        //    _adminService.DeleteClient(id);

//        //    AdminUsers = new AdminUsersViewModel
//        //    {
//        //        EditClientId = id,
//        //        Repairguys = _adminService.GetAllRepairguys(),
//        //        Clients = _adminService.GetAllClients()
//        //    };

//        //    return Page();
//        //}

//        public async Task<IActionResult> OnGetExportRepairguyPerformanceReport()
//        {
//            var repairguys = await Task.Run(() => _adminService.GetAllRepairguysWithPerformance());
//            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
//            var stream = new MemoryStream();
//            using (var package = new ExcelPackage(stream))
//            {
//                var worksheet = package.Workbook.Worksheets.Add("Repairguys Performance");

//                worksheet.Cells[1, 1].Value = $"Отчети за изпълнението на майсторите към {DateTime.Now}: ";
//                worksheet.Cells[1, 1, 1, 5].Merge = true;
//                worksheet.Cells[1, 1, 1, 5].Style.Font.Bold = true;
//                worksheet.Cells[1, 1, 1, 5].Style.Font.Size = 16;

//                worksheet.Cells[3, 1].Value = "Име";
//                worksheet.Cells[3, 2].Value = "Имейл";
//                worksheet.Cells[3, 3].Value = "Средна Оценка";
//                worksheet.Cells[3, 4].Value = "Брой Резервации";
//                worksheet.Cells[3, 5].Value = "Брой Завършени Резервации";

//                // Style headers
//                using (var range = worksheet.Cells[3, 1, 3, 5])
//                {
//                    range.Style.Font.Bold = true;
//                    range.Style.Font.Size = 14;
//                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
//                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
//                    range.AutoFitColumns();
//                }

//                for (int i = 0; i < repairguys.Count; i++)
//                {
//                    var repairguy = repairguys[i];
//                    worksheet.Cells[i + 4, 1].Value = $"{repairguy.RfirstName} {repairguy.RlastName}";
//                    worksheet.Cells[i + 4, 2].Value = repairguy.Remail;
//                    worksheet.Cells[i + 4, 3].Value = repairguy.AverageRating;
//                    worksheet.Cells[i + 4, 4].Value = repairguy.TotalReservations;
//                    worksheet.Cells[i + 4, 5].Value = repairguy.CompletedReservations;
//                    worksheet.Cells[i + 4, 1, i + 4, 5].Style.Font.Size = 12;
//                }

//                // Add borders to all cells
//                using (var range = worksheet.Cells[3, 1, 3 + repairguys.Count, 5])
//                {
//                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
//                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
//                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
//                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
//                }

//                // Add thick borders between columns and around column names
//                for (int col = 1; col <= 5; col++)
//                {
//                    // Apply thick borders to the header cells
//                    worksheet.Cells[3, col].Style.Border.Top.Style = ExcelBorderStyle.Thick;
//                    worksheet.Cells[3, col].Style.Border.Left.Style = ExcelBorderStyle.Thick;
//                    worksheet.Cells[3, col].Style.Border.Right.Style = ExcelBorderStyle.Thick;
//                    worksheet.Cells[3, col].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;

//                    worksheet.Cells[3 + repairguys.Count, col].Style.Border.Right.Style = ExcelBorderStyle.Thick;
//                    for (int row = 3; row <= 3 + repairguys.Count; row++)
//                    {
//                        worksheet.Cells[row, col].Style.Border.Right.Style = ExcelBorderStyle.Thick;
//                    }
//                }

//                // Make the top, bottom, and left sides of the table thick
//                for (int row = 3; row <= 3 + repairguys.Count; row++)
//                {
//                    worksheet.Cells[row, 1].Style.Border.Left.Style = ExcelBorderStyle.Thick;
//                    worksheet.Cells[row, 5].Style.Border.Right.Style = ExcelBorderStyle.Thick; // Ensure the right border is also thick
//                }

//                worksheet.Cells[3, 1, 3, 5].Style.Border.Top.Style = ExcelBorderStyle.Thick; // Ensure the top border of the table is thick
//                worksheet.Cells[3 + repairguys.Count, 1, 3 + repairguys.Count, 5].Style.Border.Bottom.Style = ExcelBorderStyle.Thick; // Ensure the bottom border of the table is thick

//                // Auto fit columns
//                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

//                package.Save();
//            }
//            stream.Position = 0;
//            var fileName = $"Repairguys_Performance_Report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
//            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
//            return File(stream, contentType, fileName);
//        }

//    }
//}


using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using RepairPlatform.Services;
using RepairPlatform.Services.DTO.Admins;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairPlatform.Web.Pages.Views.Admin
{
    public class AdminUsersModel : PageModel
    {
        private readonly AdministratorsService _adminService;

        public AdminUsersModel(AdministratorsService adminService)
        {
            _adminService = adminService;
        }

        public AdminUsersViewModel AdminUsers { get; set; }

        public class AdminUsersViewModel
        {
            public List<RepairguyPerformanceDto> Repairguys { get; set; }
            public List<Entities.Client> Clients { get; set; }
            public int EditRepairguyId { get; set; }
            public int EditClientId { get; set; }
        }

        public void OnGet(string? status, string? sortOption)
        {
            AdminUsers = new AdminUsersViewModel
            {
                Repairguys = _adminService.GetAllRepairguysWithPerformance(status, sortOption),
                Clients = _adminService.GetAllClients()
            };
        }

        public SelectList GetStatusOptions(string status)
        {
            return new SelectList(new[]
            {
                new SelectListItem { Value = "Active", Text = "Active" },
                new SelectListItem { Value = "Inactive", Text = "Inactive" },
            }, "Value", "Text", status);
        }

        public IActionResult OnPostChangeRepairguyStatus(int id, string status)
        {
            _adminService.ChangeRepairguyStatus(id, status);
            return RedirectToPage();
        }

        public IActionResult OnPostChangeClientStatus(int id, string status)
        {
            _adminService.ChangeClientStatus(id, status);
            return RedirectToPage();
        }

        public IActionResult OnPostEditRepairguy(int id)
        {
            AdminUsers = new AdminUsersViewModel
            {
                EditRepairguyId = id,
                Repairguys = _adminService.GetAllRepairguysWithPerformance(null, null),
                Clients = _adminService.GetAllClients()
            };

            return Page();
        }

        public IActionResult OnPostEditClient(int id)
        {
            AdminUsers = new AdminUsersViewModel
            {
                EditClientId = id,
                Repairguys = _adminService.GetAllRepairguysWithPerformance(null, null), 
                Clients = _adminService.GetAllClients()
            };

            return Page();
        }

        public async Task<IActionResult> OnGetExportRepairguyPerformanceReport(string? status, string? sortOption)
        {
            var repairguys = await Task.Run(() => _adminService.GetAllRepairguysWithPerformance(status, sortOption));
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Repairguys Performance");

                worksheet.Cells[1, 1].Value = $"Отчети за изпълнението на майсторите към {DateTime.Now}: ";
                worksheet.Cells[1, 1, 1, 5].Merge = true;
                worksheet.Cells[1, 1, 1, 5].Style.Font.Bold = true;
                worksheet.Cells[1, 1, 1, 5].Style.Font.Size = 16;

                worksheet.Cells[3, 1].Value = "Име";
                worksheet.Cells[3, 2].Value = "Имейл";
                worksheet.Cells[3, 3].Value = "Средна Оценка";
                worksheet.Cells[3, 4].Value = "Брой Резервации";
                worksheet.Cells[3, 5].Value = "Брой Завършени Резервации";


                using (var range = worksheet.Cells[3, 1, 3, 5])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Font.Size = 14;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    range.AutoFitColumns();
                }

                for (int i = 0; i < repairguys.Count; i++)
                {
                    var repairguy = repairguys[i];
                    worksheet.Cells[i + 4, 1].Value = $"{repairguy.RfirstName} {repairguy.RlastName}";
                    worksheet.Cells[i + 4, 2].Value = repairguy.Remail;
                    worksheet.Cells[i + 4, 3].Value = repairguy.AverageRating;
                    worksheet.Cells[i + 4, 4].Value = repairguy.TotalReservations;
                    worksheet.Cells[i + 4, 5].Value = repairguy.CompletedReservations;
                    worksheet.Cells[i + 4, 1, i + 4, 5].Style.Font.Size = 12;
                }


                using (var range = worksheet.Cells[3, 1, 3 + repairguys.Count, 5])
                {
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }


                for (int col = 1; col <= 5; col++)
                {
 
                    worksheet.Cells[3, col].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells[3, col].Style.Border.Left.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells[3, col].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells[3, col].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;

                    worksheet.Cells[3 + repairguys.Count, col].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                    for (int row = 3; row <= 3 + repairguys.Count; row++)
                    {
                        worksheet.Cells[row, col].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                    }
                }


                for (int row = 3; row <= 3 + repairguys.Count; row++)
                {
                    worksheet.Cells[row, 1].Style.Border.Left.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells[row, 5].Style.Border.Right.Style = ExcelBorderStyle.Thick; 
                }

                worksheet.Cells[3, 1, 3, 5].Style.Border.Top.Style = ExcelBorderStyle.Thick; 
                worksheet.Cells[3 + repairguys.Count, 1, 3 + repairguys.Count, 5].Style.Border.Bottom.Style = ExcelBorderStyle.Thick; 


                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                package.Save();
            }
            stream.Position = 0;
            var fileName = $"Repairguys_Performance_Report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(stream, contentType, fileName);
        }
    }
}

