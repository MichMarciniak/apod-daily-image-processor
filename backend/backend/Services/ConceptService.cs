using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class ConceptService
{
    private AppDbContext _context;

    public ConceptService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Concept>> GetAllConcepts()
    {
        var res = await _context.Concepts.ToListAsync();
        return res;
    }

    public async Task<Concept?> GetConcept(string name)
    {
        var res = await _context.Concepts
            .Where(x => x.Name == name)
            .FirstOrDefaultAsync();

        return res;
    }

    public async Task<Concept> AddNewConcept(string name)
    {
        var concept = new Concept { Name = name };
        var res = _context.Concepts.Add(concept);
        await _context.SaveChangesAsync();
        return concept;
    } 
}