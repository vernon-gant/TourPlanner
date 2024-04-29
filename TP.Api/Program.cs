using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using TP.Api.Utils;
using TP.DataAccess;
using TP.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddRouting();
builder.Services.AddControllers().AddOData(options =>
    {
        options.TimeZone = TimeZoneInfo.Utc;
        options.EnableQueryFeatures().AddRouteComponents(ApplicationConstants.OdataDefaultPrefix, EdmModelBuilder.GetEdmModel());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policyBuilder => policyBuilder.WithOrigins("http://localhost"));
});
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseODataRouteDebug();
}
else
{
    app.UseExceptionHandler();
}

app.UseStatusCodePages(async context =>
{
    context.HttpContext.Response.ContentType = "application/problem+json";

    var problemDetails = new ProblemDetails
    {
        Status = context.HttpContext.Response.StatusCode,
        Title = "A problem occurred with your request.",
        Detail = $"The requested endpoint {context.HttpContext.Request.Path} returned status code {context.HttpContext.Response.StatusCode}."
    };

    await context.HttpContext.Response.WriteAsJsonAsync(problemDetails);
});

app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();

app.MapControllers();

app.Run();