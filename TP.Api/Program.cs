using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using TP.Api.Configuration;
using TP.Api.Http;
using TP.Api.Utils;
using TP.DataAccess;
using TP.Database;
using TP.Service.Tour;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.Configure<OpenRouteConfiguration>(builder.Configuration.GetSection("OpenRouteConfiguration"));

builder.Services.AddProblemDetails();
builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
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
builder.Services.AddHttpClient<OpenRouteClient, HttpOpenRouteClient>()
    .ConfigureHttpClient((serviceProvider, client) =>
    {
        OpenRouteConfiguration openRouteConfig = serviceProvider.GetRequiredService<IOptions<OpenRouteConfiguration>>().Value;
        client.BaseAddress = new Uri("https://api.openrouteservice.org/v2/directions");
        client.DefaultRequestHeaders.Add("Authorization", openRouteConfig.ApiKey);
    });
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDataAccess();
builder.Services.AddTour();

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

app.UseSerilogRequestLogging();
app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();
app.MapControllers();

await app.SetupDatabaseAsync();

app.Run();