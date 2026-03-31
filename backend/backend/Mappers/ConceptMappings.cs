using backend.Dtos;
using backend.Models;

namespace backend.Mappers;

public static class ConceptMappings
{

    public static ConceptResponse ToDto(this Concept concept)
    {
        return new ConceptResponse(
            concept.Name
        );
    }
}