using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var applications = new ConcurrentDictionary<Guid, JobApplication>();
Seed();

app.MapGet("/", () => Results.Ok(new
{
    service = "CareerTrack API",
    status = "running",
    documentation = "See README for endpoint examples"
}));

app.MapGet("/api/applications", (string? status) =>
{
    var result = applications.Values
        .Where(item => string.IsNullOrWhiteSpace(status) || item.Status.Equals(status, StringComparison.OrdinalIgnoreCase))
        .OrderByDescending(item => item.UpdatedAt)
        .ToList();
    return Results.Ok(result);
});

app.MapGet("/api/applications/{id:guid}", (Guid id) =>
    applications.TryGetValue(id, out var item) ? Results.Ok(item) : Results.NotFound());

app.MapPost("/api/applications", (CreateApplicationRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Company) || string.IsNullOrWhiteSpace(request.Role))
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["company"] = ["Company is required."],
            ["role"] = ["Role is required."]
        });

    var item = new JobApplication(
        Guid.NewGuid(), request.Company.Trim(), request.Role.Trim(),
        NormalizeStatus(request.Status), request.Notes?.Trim() ?? string.Empty, DateTimeOffset.UtcNow);
    applications[item.Id] = item;
    return Results.Created($"/api/applications/{item.Id}", item);
});

app.MapPut("/api/applications/{id:guid}/status", (Guid id, UpdateStatusRequest request) =>
{
    if (!applications.TryGetValue(id, out var existing)) return Results.NotFound();
    var updated = existing with { Status = NormalizeStatus(request.Status), UpdatedAt = DateTimeOffset.UtcNow };
    applications[id] = updated;
    return Results.Ok(updated);
});

app.MapDelete("/api/applications/{id:guid}", (Guid id) =>
    applications.TryRemove(id, out _) ? Results.NoContent() : Results.NotFound());

app.Run();

string NormalizeStatus(string? value)
{
    var allowed = new[] { "Saved", "Applied", "Interview", "Offer", "Rejected" };
    return allowed.FirstOrDefault(status => status.Equals(value, StringComparison.OrdinalIgnoreCase)) ?? "Saved";
}

void Seed()
{
    var seed = new JobApplication(Guid.NewGuid(), "Northstar Labs", "Junior Software Developer", "Applied", "Portfolio shared", DateTimeOffset.UtcNow.AddDays(-2));
    applications[seed.Id] = seed;
}

public record JobApplication(Guid Id, string Company, string Role, string Status, string Notes, DateTimeOffset UpdatedAt);
public record CreateApplicationRequest(string Company, string Role, string? Status, string? Notes);
public record UpdateStatusRequest(string? Status);
