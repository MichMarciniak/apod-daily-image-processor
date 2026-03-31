using backend.Data;
using backend.Dtos;
using backend.Mappers;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class ConceptService
{
    private readonly AppDbContext _context;

    public ConceptService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ConceptResponse>> GetAllConcepts()
    {
        var res = await _context.Concepts
            .Select(x => x.ToDto())
            .ToListAsync();
        return res;
    }

    public async Task<ConceptResponse?> GetConcept(string name)
    {
        var res = await _context.Concepts
            .Select(x => x.ToDto())
            .Where(x => x.Name == name)
            .FirstOrDefaultAsync();

        return res;
    }

    public async Task<ConceptResponse> AddNewConcept(string name)
    {
        var concept = new Concept { Name = name };
        var res = _context.Concepts.Add(concept);
        await _context.SaveChangesAsync();
        
        return concept.ToDto();
    } 
}