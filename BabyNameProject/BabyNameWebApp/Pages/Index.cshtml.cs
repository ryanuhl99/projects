using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Collections.Generic;
using BabyNameWebApp.Models;
using BabyNameWebApp.Data;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;


namespace BabyNameWebApp.Pages;

public class IndexModel : PageModel
{

    public IndexModel(BabyNamesContext context, ILogger<IndexModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    private readonly ILogger<IndexModel> _logger;
    private readonly BabyNamesContext _context;


    public List<BabyName> FilteredNames { get; set; } = new List<BabyName>();
  
    [BindProperty]
    public string? Gender { get; set;}

    [BindProperty]
    public int Syllables { get; set; }

    [BindProperty]
    [StringLength(1, ErrorMessage="Please Enter Exactly One Character.")]
    public string? StartsWith { get; set; }

    public void OnGet()
    {
    }

    public void OnPost()
    {
        if (!ModelState.IsValid)
        {
            return;
        }

        IQueryable<BabyName> query = _context.BabyNames;


        if (!string.IsNullOrEmpty(Gender))
        {
            query = query.Where(g => g.Gender == Gender);
        }

        if (Syllables > 0)
        {
            query = query.AsEnumerable().Where(s => CountSyllables(s.Name) == Syllables).AsQueryable();
        }

        if (!string.IsNullOrEmpty(StartsWith))
        {
           string startsWithLower = StartsWith.ToLower();
           query = query.Where(x => x.Name.ToLower().StartsWith(startsWithLower));
        }

        FilteredNames = query.ToList();

    }

    private int CountSyllables(string name)
    {
        return name.Count(n => "AEIOUaeiou".Contains(n));
    }
}