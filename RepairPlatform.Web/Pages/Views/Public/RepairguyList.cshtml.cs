using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RepairPlatform.Services.DTO.Repairguys;
using RepairPlatform.Services;
using Microsoft.EntityFrameworkCore;
using RepairPlatform.Entities;

namespace RepairPlatform.Web.Pages.Views.Public
{
    public class RepairguyListModel : PageModel
    {

        private readonly RepairguysService _repairguysService;
        private readonly Repairguy20118046Context _dbContext;
        public RepairguyListModel(RepairguysService repairguysService, Repairguy20118046Context dbContext)
        {
            _repairguysService = repairguysService;
            _dbContext = dbContext;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SortOption { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? GroupOption { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? TownOption { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        public int TotalPages { get; set; }



        public IList<RepairguyDto>? Repairguys { get; set; }
        public IList<string>? Groups { get; set; }
        public IList<string?>? Towns { get; set; }
        public IList<(RepairguyDto Repairguy, double AverageRating)>? RepairguysWithRating { get; set; }

        public async Task<IActionResult> OnGetAsync(int id, string searchTerm, string sortOption, string groupOption, string townOption, int pageNumber = 1)
        {
            Id = id;
            PageNumber = pageNumber;

            var query = _dbContext.Repairguys
                .Include(r => r.Town) 
                                      //.Include(r => r.Groups) // Include Groups
                        .Include(r => r.Repairs) 
                        .ThenInclude(repair => repair.Groups)
                .AsQueryable();


            var initialRepairguys = await query.ToListAsync();

            foreach (var repairguy in initialRepairguys)
            {
                var groups = repairguy.Repairs.SelectMany(r => r.Groups.Select(g => g.CatName)).Distinct().ToList();
                repairguy.Groups = groups;
            }

            //if (string.IsNullOrEmpty(searchTerm))
            //{
            //    Repairguys = await _repairguysService.GetActiveRepairguysAsync();
            //}
            //else
            //{
            //    Repairguys = await _repairguysService.Search(searchTerm);
            //}

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(r => r.RfirstName.Contains(searchTerm) || r.RlastName.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(groupOption))
            {
                initialRepairguys = initialRepairguys.Where(r => r.Groups.Contains(groupOption)).ToList();
            }

            if (!string.IsNullOrEmpty(townOption))
            {
                initialRepairguys = initialRepairguys.Where(r => r.Town.Name == townOption).ToList();
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                initialRepairguys = initialRepairguys.Where(r => r.RfirstName.Contains(searchTerm) || r.RlastName.Contains(searchTerm)).ToList();
            }


            Repairguys = initialRepairguys.Select(r => new RepairguyDto
            {
                RepairguyId = r.RepairguyId,
                RfirstName = r.RfirstName,
                RlastName = r.RlastName,
                Remail = r.Remail,
                Rdescription = r.Rdescription,
                Rphoto = r.Rphoto,
                Rstatus = r.Rstatus,
                Groups = r.Groups,
                TownId = r.TownId,
            }).ToList();

            RepairguysWithRating = new List<(RepairguyDto, double)>();
            foreach (var repairguy in Repairguys)
            {
                var reviews = await _dbContext.Reviews
                    .Where(r => r.RepairguyId == repairguy.RepairguyId)
                    .ToListAsync();

                double averageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;
                RepairguysWithRating.Add((repairguy, averageRating));
            }

            if (!string.IsNullOrEmpty(sortOption))
            {
                switch (sortOption)
                {
                    case "firstNameAsc":
                        RepairguysWithRating = RepairguysWithRating.OrderBy(rg => rg.Repairguy.RfirstName).ToList();
                        break;
                    case "firstNameDesc":
                        RepairguysWithRating = RepairguysWithRating.OrderByDescending(rg => rg.Repairguy.RfirstName).ToList();
                        break;
                    case "ratingAsc":
                        RepairguysWithRating = RepairguysWithRating.OrderBy(rg => rg.AverageRating).ToList();
                        break;
                    case "ratingDesc":
                        RepairguysWithRating = RepairguysWithRating.OrderByDescending(rg => rg.AverageRating).ToList();
                        break;
                }
            }

            int pageSize = 10; 
            TotalPages = (int)Math.Ceiling(RepairguysWithRating.Count / (double)pageSize);

            Repairguys = RepairguysWithRating
                .Skip((PageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(rg => rg.Repairguy)
                .ToList();

            Groups = await _dbContext.Groups.Select(g => g.CatName).ToListAsync();
            Towns = await _dbContext.Towns
                .Where(t => t.Name != null)
                .Select(t => t.Name!)
                .ToListAsync();


            return Page();
        }
    }
}
