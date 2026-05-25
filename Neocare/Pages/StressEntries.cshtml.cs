using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Neocare.Application.DTOs;
using Neocare.Application.Services;

namespace Neocare.Pages
{
    public class StressEntriesModel : PageModel
    {
        private readonly StressEntryService _stressEntryService;

        public StressEntriesModel(StressEntryService stressEntryService)
        {
            _stressEntryService = stressEntryService;
        }

        public SearchResult<StressEntryDto> SearchResult { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        [BindProperty(SupportsGet = true)]
        public string SortBy { get; set; } = "date";

        [BindProperty(SupportsGet = true)]
        public string SortDirection { get; set; } = "desc";

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? MinStressLevel { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? MaxStressLevel { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FromDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? ToDate { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                var searchParams = new SearchParams
                {
                    Page = CurrentPage,
                    PageSize = PageSize,
                    SortBy = SortBy,
                    SortDirection = SortDirection,
                    SearchTerm = SearchTerm,
                    MinStressLevel = MinStressLevel,
                    MaxStressLevel = MaxStressLevel,
                    FromDate = FromDate,
                    ToDate = ToDate
                };

                SearchResult = await _stressEntryService.SearchStressEntries(searchParams);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in OnGetAsync: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                SearchResult = new SearchResult<StressEntryDto> { Items = new List<StressEntryDto>(), TotalCount = 0 };
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            var result = await _stressEntryService.DeleteStressEntry(id);
            if (!result)
            {
                TempData["Error"] = "Registro não encontrado ou não pode ser excluído.";
            }
            return RedirectToPage(new { CurrentPage, PageSize, SortBy, SortDirection, SearchTerm, MinStressLevel, MaxStressLevel, FromDate, ToDate });
        }

        public string GetPageUrl(int page)
        {
            return Url.Page("./StressEntries", new
            {
                CurrentPage = page,
                PageSize,
                SortBy,
                SortDirection,
                SearchTerm,
                MinStressLevel,
                MaxStressLevel,
                FromDate,
                ToDate
            });
        }
    }
}