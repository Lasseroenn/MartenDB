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
        o.Connection(builder.Configuration.GetConnectionString("default"));
    }
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/create", async ([FromBody] CreateUserRequest request, IDocumentSession session) =>
{
    session.Store(new User(request.Name, request.Age));
    await session.SaveChangesAsync();
    return TypedResults.NoContent();
});

app.MapGet("/fetch/{id}", (IQuerySession query) =>
{
    return "user";
});

public record User(string Name, int Age)
{
    public Guid Id { get; set; }
    
};

public record CreateUserRequest
{
    public string Name;
    public int Age;
}
