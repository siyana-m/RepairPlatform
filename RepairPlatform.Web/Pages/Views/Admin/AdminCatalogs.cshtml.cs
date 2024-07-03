using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RepairPlatform.Entities;
using RepairPlatform.Services;
using RepairPlatform.Services.DTO.Reservations;
using System.Text.RegularExpressions;
using static RepairPlatform.Web.Pages.Views.Admin.AdminCatalogsModel;

namespace RepairPlatform.Web.Pages.Views.Admin
{
    public class AdminCatalogsModel : PageModel
    {
        private readonly AdministratorsService _adminService;

        public AdminCatalogsModel(AdministratorsService adminService)
        {
            _adminService = adminService;
        }

        [BindProperty]
        public AdminCatalogsViewModel? AdminCatalogs { get; set; }
        public class AdminCatalogsViewModel
        {
            public List<Entities.Group> Groups { get; set; } = new List<RepairPlatform.Entities.Group>();
            public List<Repair> Repairs { get; set; } = new List<Repair>();
            public List<Town> Towns { get; set; } = new List<Town>();

            [BindProperty]
            public int EditGroupId { get; set; }

            [BindProperty]
            public int EditRepairId { get; set; }

            [BindProperty]
            public int EditTownId { get; set; }

            [BindProperty]
            public Services.DTO.Groups.GroupDto? NewGroup { get; set; }

            [BindProperty]
            public Services.DTO.Repairs.RepairDto? NewRepair { get; set; }

            [BindProperty]
            public Services.DTO.Towns.TownDto? NewTown { get; set; }
        }

        public void OnGet()
        {
            AdminCatalogs = new AdminCatalogsViewModel
            {
                Groups = _adminService.GetAllGroupsWithRepairs(),
                Towns = _adminService.GetAllTowns(),
                NewGroup = new Services.DTO.Groups.GroupDto(),
                NewRepair = new Services.DTO.Repairs.RepairDto(),
                NewTown = new Services.DTO.Towns.TownDto()
            };
        }

        public IActionResult OnPostCreateGroup()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (AdminCatalogs == null)
            {
                AdminCatalogs = new AdminCatalogsViewModel
                {
                    Groups = _adminService.GetAllGroupsWithRepairs(),
                    Towns = _adminService.GetAllTowns(),
                    NewGroup = new Services.DTO.Groups.GroupDto(),
                    NewRepair = new Services.DTO.Repairs.RepairDto(),
                    NewTown = new Services.DTO.Towns.TownDto()
                };
            }

            _adminService.CreateGroup(AdminCatalogs!.NewGroup);
            return RedirectToPage();
        }

        public IActionResult OnPostCreateRepair(int groupId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (AdminCatalogs == null)
            {
                AdminCatalogs = new AdminCatalogsViewModel
                {
                    Groups = _adminService.GetAllGroupsWithRepairs(),
                    Towns = _adminService.GetAllTowns(),
                    NewGroup = new Services.DTO.Groups.GroupDto(),
                    NewRepair = new Services.DTO.Repairs.RepairDto(),
                    NewTown = new Services.DTO.Towns.TownDto()
                };
            }

            
            var dbGroup = _adminService.GetGroupbyId(groupId);
            AdminCatalogs!.NewRepair.Groups.Add(dbGroup);

            _adminService.CreateRepair(AdminCatalogs!.NewRepair);
            return RedirectToPage();
        }

        public IActionResult OnPostCreateTown()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (AdminCatalogs == null)
            {
                AdminCatalogs = new AdminCatalogsViewModel
                {
                    Groups = _adminService.GetAllGroupsWithRepairs(),
                    Towns = _adminService.GetAllTowns(),
                    NewGroup = new Services.DTO.Groups.GroupDto(),
                    NewRepair = new Services.DTO.Repairs.RepairDto(),
                    NewTown = new Services.DTO.Towns.TownDto()
                };
            }

            _adminService.CreateTown(AdminCatalogs!.NewTown);
            return RedirectToPage();
        }

        public IActionResult OnPostEditGroup(int id)
        {
            AdminCatalogs = new AdminCatalogsViewModel
            {
                EditGroupId = id,
                Groups = _adminService.GetAllGroups(),
                Repairs = _adminService.GetAllRepairs(),
                Towns = _adminService.GetAllTowns()
            };

            return Page();
        }

        public IActionResult OnPostEditRepair(int id)
        {
            AdminCatalogs = new AdminCatalogsViewModel
            {
                EditRepairId = id,
                Groups = _adminService.GetAllGroups(),
                Repairs = _adminService.GetAllRepairs(),
                Towns = _adminService.GetAllTowns()
            };

            return Page();
        }

        public IActionResult OnPostEditTown(int id)
        {
            AdminCatalogs = new AdminCatalogsViewModel
            {
                EditTownId = id,
                Groups = _adminService.GetAllGroups(),
                Repairs = _adminService.GetAllRepairs(),
                Towns = _adminService.GetAllTowns()
            };

            return Page();
        }

        public Task<IActionResult> OnPostDeleteGroupAsync(int id)
        {
            _adminService.DeleteGroup(id);

            AdminCatalogs = new AdminCatalogsViewModel
            {
                EditGroupId = id,
                Groups = _adminService.GetAllGroups(),
                Repairs = _adminService.GetAllRepairs(),
                Towns = _adminService.GetAllTowns()
            };

            return Task.FromResult<IActionResult>(Page());
        }

        public Task<IActionResult> OnPostDeleteRepairAsync(int id)
        {
            _adminService.DeleteRepair(id);

            AdminCatalogs = new AdminCatalogsViewModel
            {
                EditRepairId = id,
                Groups = _adminService.GetAllGroups(),
                Repairs = _adminService.GetAllRepairs(),
                Towns = _adminService.GetAllTowns()
            };

            return Task.FromResult<IActionResult>(Page());
        }

        public Task<IActionResult> OnPostDeleteTownAsync(int id)
        {
            _adminService.DeleteTown(id);

            AdminCatalogs = new AdminCatalogsViewModel
            {
                EditTownId = id,
                Groups = _adminService.GetAllGroups(),
                Repairs = _adminService.GetAllRepairs(),
                Towns = _adminService.GetAllTowns()
            };

            return Task.FromResult<IActionResult>(Page());
        }

        //public async Task<IActionResult> OnPostEditTownAsync(int id)
        //{
        //    AdminCatalogs.EditTownId = id;
        //    await LoadAdminCatalogsAsync().ConfigureAwait(false);
        //    return Page();
        //}

        //private async Task LoadAdminCatalogsAsync()
        //{
        //    if (AdminCatalogs == null)
        //    {
        //        AdminCatalogs = new AdminCatalogsViewModel
        //        {
        //            Groups = _adminService.GetAllGroupsWithRepairs(),
        //            Towns = _adminService.GetAllTowns(),
        //            NewGroup = new Services.DTO.Groups.GroupDto(),
        //            NewRepair = new Services.DTO.Repairs.RepairDto(),
        //            NewTown = new Services.DTO.Towns.TownDto()
        //        };
        //    }
        //}
    }

 
}
