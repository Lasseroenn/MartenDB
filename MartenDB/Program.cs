using Marten;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMarten(o =>
    {
        o.Connection(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(config =>
    {
        config.ConfigObject.AdditionalItems["syntaxHighlight"] = new Dictionary<string, object>
        {
            ["activated"] = false
        };
    });
}

app.UseHttpsRedirection();

app.MapPost("/create", async ([FromBody] CreateUserRequest request, IDocumentSession session) =>
{
    var user = new User(request.Name, request.Age);
    session.Store(user);
    await session.SaveChangesAsync();
    return TypedResults.Created($"/fetch/{user.Id}", user.Id);
});

app.MapGet("/{id}", async ([FromRoute] Guid id, IQuerySession query) =>
{
    var user = await query.LoadAsync<User>(id);
    return TypedResults.Ok(user);
});

// app.MapPut("/{id}", async ([FromRoute] Guid id, [FromBody] CreateUserRequest request, IDocumentSession session) =>
// {
//     var existingUser = await session.LoadAsync<User>(id);
//     if (existingUser is null)
//     {
//         return TypedResults.NotFound();
//     }
//     session.Store(existingUser);
//     await session.SaveChangesAsync();
//     return TypedResults.Ok(existingUser);
// });
app.Run();

public record User(string Name, int Age)
{
    public Guid Id { get; set; }
};

public record CreateUserRequest
{
    public string Name { get; set; }
    public int Age { get; set; }
}
