using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Neocare.Application.DTOs;
using Neocare.Application.Services;
using System.ComponentModel.DataAnnotations;

namespace Neocare.Pages
{
    public class CreateStressEntryModel : PageModel
    {
        private readonly StressEntryService _stressEntryService;

        public CreateStressEntryModel(StressEntryService stressEntryService)
        {
            _stressEntryService = stressEntryService;
        }

        [BindProperty]
        public StressEntryViewModel Entry { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Log dos erros para debug
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                }
                return Page();
            }

            var createDto = new CreateStressEntryDto
            {
                StressLevel = Entry.StressLevel,
                Description = Entry.Description,
                Symptoms = Entry.Symptoms?.Split(',').Select(s => s.Trim()).ToList() ?? new List<string>(),
                UserId = "default-user"
            };

            await _stressEntryService.CreateAsync(createDto);
            return RedirectToPage("./StressEntries");
        }
    }

    public class StressEntryViewModel
    {
        [Required(ErrorMessage = "O nível de estresse é obrigatório")]
        [Range(1, 10, ErrorMessage = "O nível de estresse deve estar entre 1 e 10")]
        [Display(Name = "Nível de Estresse")]
        public int StressLevel { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "A descrição deve ter entre 10 e 500 caracteres")]
        [Display(Name = "Descrição")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Sintomas (separados por vírgula)")]
        public string? Symptoms { get; set; }
    }
}