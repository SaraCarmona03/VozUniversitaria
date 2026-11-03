using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VozUniversitaria.Data;
using VozUniversitaria.Models;

namespace VozUniversitaria.Routers.Api;

[ApiController]
[Authorize]
[Route("api/publications")]
[AutoValidateAntiforgeryToken]
public sealed class PublicationsApiController : ControllerBase
{
    private static readonly Dictionary<string, string> Categories = new()
    {
        ["complaint"] = "Queja",
        ["suggestion"] = "Sugerencia",
        ["event"] = "Evento",
        ["notice"] = "Aviso"
    };

    private readonly AppDbContext _dbContext;

    public PublicationsApiController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetPublications(CancellationToken cancellationToken)
    {
        var publications = await _dbContext.Publications
            .AsNoTracking()
            .OrderByDescending(publication => publication.CreatedAt)
            .Select(publication => new
            {
                publication.Id,
                publication.AuthorId,
                publication.CategoryKey,
                publication.Title,
                publication.Description,
                publication.CreatedAt,
                Author = publication.Author.Name + " " + publication.Author.LastName
            })
            .ToListAsync(cancellationToken);

        return Ok(publications.Select(publication => new
        {
            publication.Id,
            publication.AuthorId,
            publication.CategoryKey,
            Category = Categories[publication.CategoryKey],
            publication.Title,
            publication.Description,
            publication.CreatedAt,
            publication.Author
        }));
    }

    [HttpPost]
    public async Task<IActionResult> CreatePublication(
        CreatePublicationRequest request,
        CancellationToken cancellationToken)
    {
        var categoryKey = request.CategoryKey.Trim().ToLowerInvariant();
        if (!Categories.TryGetValue(categoryKey, out var category))
        {
            return BadRequest(new
            {
                error = "Selecciona una categoría válida.",
                errors = new { categoryKey = new[] { "Selecciona una categoría válida." } }
            });
        }

        var title = request.Title.Trim();
        var description = request.Description.Trim();
        if (title.Length == 0 || description.Length == 0)
        {
            return BadRequest(new { error = "El título y la descripción son obligatorios." });
        }

        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authorId))
        {
            return Unauthorized(new { error = "Inicia sesión para crear publicaciones." });
        }

        var author = await _dbContext.Users
            .Where(user => user.Id == authorId)
            .Select(user => user.Name + " " + user.LastName)
            .SingleOrDefaultAsync(cancellationToken);
        if (author is null)
        {
            return Unauthorized(new { error = "Inicia sesión para crear publicaciones." });
        }

        var publication = new Publication
        {
            CategoryKey = categoryKey,
            Title = title,
            Description = description,
            CreatedAt = DateTime.UtcNow,
            AuthorId = authorId
        };

        _dbContext.Publications.Add(publication);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Created("/api/publications", new
        {
            publication.Id,
            publication.AuthorId,
            publication.CategoryKey,
            Category = category,
            publication.Title,
            publication.Description,
            publication.CreatedAt,
            Author = author
        });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdatePublication(
        int id,
        UpdatePublicationRequest request,
        CancellationToken cancellationToken)
    {
        var authorId = GetCurrentUserId();
        if (authorId is null)
        {
            return Unauthorized(new { error = "Inicia sesión para editar publicaciones." });
        }

        var publication = await _dbContext.Publications
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (publication is null)
        {
            return NotFound(new { error = "No se encontró la publicación." });
        }

        if (publication.AuthorId != authorId)
        {
            return Forbid();
        }

        var categoryKey = request.CategoryKey.Trim().ToLowerInvariant();
        if (!Categories.TryGetValue(categoryKey, out var category))
        {
            return BadRequest(new { error = "Selecciona una categoría válida." });
        }

        var title = request.Title.Trim();
        var description = request.Description.Trim();
        if (title.Length == 0 || description.Length == 0)
        {
            return BadRequest(new { error = "El título y la descripción son obligatorios." });
        }

        publication.CategoryKey = categoryKey;
        publication.Title = title;
        publication.Description = description;
        await _dbContext.SaveChangesAsync(cancellationToken);

        var author = await _dbContext.Users
            .Where(user => user.Id == publication.AuthorId)
            .Select(user => user.Name + " " + user.LastName)
            .SingleAsync(cancellationToken);

        return Ok(new
        {
            publication.Id,
            publication.AuthorId,
            publication.CategoryKey,
            Category = category,
            publication.Title,
            publication.Description,
            publication.CreatedAt,
            Author = author
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePublication(int id, CancellationToken cancellationToken)
    {
        var authorId = GetCurrentUserId();
        if (authorId is null)
        {
            return Unauthorized(new { error = "Inicia sesión para eliminar publicaciones." });
        }

        var publication = await _dbContext.Publications
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (publication is null)
        {
            return NotFound(new { error = "No se encontró la publicación." });
        }

        if (publication.AuthorId != authorId)
        {
            return Forbid();
        }

        _dbContext.Publications.Remove(publication);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private int? GetCurrentUserId() =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
            ? userId
            : null;
}

public sealed class CreatePublicationRequest
{
    [Required]
    [StringLength(20)]
    public string CategoryKey { get; init; } = string.Empty;

    [Required]
    [StringLength(120)]
    public string Title { get; init; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; init; } = string.Empty;
}

public sealed class UpdatePublicationRequest
{
    [Required]
    [StringLength(20)]
    public string CategoryKey { get; init; } = string.Empty;

    [Required]
    [StringLength(120)]
    public string Title { get; init; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; init; } = string.Empty;
}
