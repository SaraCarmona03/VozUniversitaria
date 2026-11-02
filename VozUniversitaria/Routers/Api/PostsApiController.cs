using Microsoft.AspNetCore.Mvc;

namespace VozUniversitaria.Routers.Api;

public sealed record PostDto(
    int Id, string CategoryKey, string Category, string Time,
    string Title, string Description, string FooterLabel,
    string? CommentsLabel, string? Status);

[ApiController]
[Route("api/posts")]
public sealed class PostsApiController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<PostDto>> GetAll()
    {
        var posts = new List<PostDto>
        {
            new(1, "notice", "Notices", "Today", "Library schedule change",
                "During final exams, it will be open from 7:00 a.m. to 10:00 p.m.",
                "Coordinación", "3 comentarios", null),
            new(2, "event", "Events", "2 hours ago", "Feria de proyectos de software",
                "Viernes 10 de octubre, 10:00 h, auditorio principal.",
                "48 asistirán", "7 comentarios", null),
            new(3, "suggestion", "Suggestion", "Yesterday", "More outlets in the cafeteria",
                "There are not enough places to charge laptops while studying.",
                "32 supports", "5 comments", null),
            new(4, "complaint", "Complaint", "3 days ago", "Aire acondicionado sin funcionar",
                "El aula B-204 lleva una semana sin clima.",
                "21 supports", null, "In review"),
        };
        return Ok(posts);
    }
}