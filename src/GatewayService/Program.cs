using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["IdentityServerUrl"];
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters.ValidateAudience = false;
    options.TokenValidationParameters.NameClaimType = "username";
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("CORSPolicy", configure =>
    {
        configure.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins(builder.Configuration["ClientUrl"]!);
    });
});

var app = builder.Build();

app.UseCors();
app.MapReverseProxy();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
