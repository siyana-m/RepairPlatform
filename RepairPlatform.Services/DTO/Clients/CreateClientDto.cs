using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairPlatform.Services.DTO.Clients
{
    public class CreateClientDto
    {
        [Required]
        [Display(Name = "Име")]
        public string? CFirstName { get; set; }

        [Required]
        [Display(Name = "Фамилия")]
        public string? CLastName { get; set; }

        [Required]
        [Display(Name = "Телефон")]
        public string? CTelephone { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Имейл")]
        public string? CEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Парола")]
        public string? CPassword { get; set; }

        [Display(Name = "Потвърди парола")]
        [Compare("CPassword", ErrorMessage = "Паролите не съвпадат.")]
        public string? ConfirmPassword { get; set; }
    }
}
