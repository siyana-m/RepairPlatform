using RepairPlatform.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairPlatform.Services.DTO.Repairguys
{
    public class CreateRepairguyDto
    {
        [Required]
        [Display(Name = "Име")]
        public string RfirstName { get; set; } = null!;

        [Required]
        [Display(Name = "Фамилия")]
        public string RlastName { get; set; } = null!;

        [Required]
        [Display(Name = "Телефон")]
        public string Rtelephone { get; set; } = null!;

        [Required]
        [EmailAddress]
        [Display(Name = "Имейл")]
        public string Remail { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Парола")]
        public string Rpassword { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Потвърди парола")]
        [Compare("Rpassword", ErrorMessage = "Паролите не съвпадат.")]
        public string ConfirmPassword { get; set; } = null!;

        [Display(Name = "Описание")]
        public string? Rdescription { get; set; }

        [Display(Name = "Снимка")]
        public byte[]? Rphoto { get; set; }

        [Display(Name = "Статус")]
        public string? Rstatus { get; set; }

        [Display(Name = "Резервации")]
        public List<int>? Reservations { get; set; }

        [Display(Name = "Отзиви")]
        public List<int>? Reviews { get; set; }

        [Display(Name = "Ремонти")]
        public List<int>? Repairs { get; set; }

        [Required]
        [Display(Name = "Град")]
        public int TownId { get; set; }

    }
}
