global using correos_backend.Helpers;
global using correos_backend.Decorators;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using dotenv.net;

var builder = WebApplication.CreateBuilder(args);

DotEnv.Load();

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson(options =>
		options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
		);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddAuthentication(options =>
		{
		options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
		options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		}).AddJwtBearer(options =>
			{
			options.RequireHttpsMetadata = false;
			options.SaveToken = true;
			options.TokenValidationParameters = new TokenValidationParameters
			{
			ValidateIssuerSigningKey = true,
			ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
			ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET"))),
			ClockSkew = TimeSpan.Zero
			};
			});

builder.Services.AddAuthorization(options =>
		{
		options.DefaultPolicy = new AuthorizationPolicyBuilder().
		AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme).
		RequireAuthenticatedUser().
		Build();
		}
		);

builder.Services.AddDbContext<CorreosContext>(options =>
		{
		options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

		});

	builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
	.AddEntityFrameworkStores<CorreosContext>()
	.AddDefaultTokenProviders();

	builder.Services.AddScoped<JwtSecurityTokenHandlerWrapper>();

	builder.Services.AddControllersWithViews(options => {
			options.Filters.Add(typeof(JwtAuthorizeFilter));});

	var app = builder.Build();


	// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/security/getMessage", () => "Hello World!").RequireAuthorization();

app.Run();
