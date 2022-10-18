var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<HotelDb>(options => {
    options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"));
});
builder.Services.AddScoped<IHotelRepository, Hotelrepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<HotelDb>();
    db.Database.EnsureCreated();
}

app.MapGet("/hotels", async (IHotelRepository repository) => 
    Results.Ok(await repository.GetHotelsAsync()))
    .Produces<List<Hotel>>(StatusCodes.Status200OK)
    .WithName("GetAllHotels")
    .WithTags("Getters");

app.MapGet("/hotels/search/name/{query}", async (string query, IHotelRepository repository) => 
    await repository.GetHotelsAsync(query) is IEnumerable<Hotel> hotels 
    ? Results.Ok(hotels)
    : Results.NotFound())
    .Produces<List<Hotel>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("SearchHotels")
    .WithTags("Getters")
    .ExcludeFromDescription();

app.MapGet("/hotels/{id}", async (int id, IHotelRepository repository) =>  
    await repository.GetHotelAsync(id) is Hotel hotel 
    ? Results.Ok(hotel) 
    : Results.NotFound())
    .Produces<Hotel>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("GetHotel")
    .WithTags("Getters"); 

app.MapPost("/hotels", async ([FromBody] Hotel hotel, IHotelRepository repository) => {
        await repository.InsertHotelAsync(hotel);
        await repository.SaveAsync();
        return Results.Created($"/hotels/{hotel.Id}", hotel);
    })
    .Accepts<Hotel>("application/json")
    .Produces<Hotel>(StatusCodes.Status201Created)
    .WithName("CreateHotel")
    .WithTags("Creators");

app.MapPut("/hotels", async ([FromBody] Hotel hotel, IHotelRepository repository) => {
        await repository.UpdateHotelAsync(hotel);
        await repository.SaveAsync();
        return Results.NoContent();
    })
    .Accepts<Hotel>("application/json")
    .Produces<Hotel>(StatusCodes.Status204NoContent)
    .WithName("UpdateHotel")
    .WithTags("Updaters");

app.MapDelete("/hotels/{id}", async (int id, IHotelRepository repository) => {
        await repository.DeleteHotelAsync(id);
        await repository.SaveAsync();
        return Results.NoContent();
    })
    .Produces<Hotel>(StatusCodes.Status204NoContent)
    .WithName("DeleteHotel")
    .WithTags("Deleters");

// app.UseHttpsRedirection(); // The SSL connection could not be established, see inner exception.

app.Run();
