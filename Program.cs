global using correos_backend.Helpers;
global using correos_backend.Constants;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;

using dotenv.net;

using correos_backend.Services;

using correos_backend.Decorators;

var builder = WebApplication.CreateBuilder(args);

DotEnv.Load();

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson(options =>
		options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
		);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Services
builder.Services.AddScoped<CsvService>();
builder.Services.AddScoped<XLSXService>();
builder.Services.AddScoped<CurrentTimeService>();

builder.Services.AddScoped<S3Service>();

builder.Services.AddAuthentication(options =>
		{
		options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
		options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		}).AddJwtBearer(options =>
			{
			options.SaveToken = true;
			options.RequireHttpsMetadata = false;
			options.TokenValidationParameters = new TokenValidationParameters
			{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateIssuerSigningKey = true,
			ValidateLifetime = true,
			ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
			ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
			IssuerSigningKey = new SymmetricSecurityKey(
					Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")!)
					)
			};
			});


// configure the default authorization policy
builder.Services.AddAuthorization(options =>
		{
		options.AddPolicy("RequireAdmin", policy =>
				{
				policy.RequireRole("Admin");
				});
		options.AddPolicy("RequireBoss", policy =>
				{
				policy.RequireRole("Boss");
				});
		options.AddPolicy("Manager", policy =>
				{
				policy.RequireRole("Manager");
				});
		}
		);

builder.Services.AddDbContext<CorreosContext>(options =>
		{
		options.UseSqlServer(Environment.GetEnvironmentVariable("CONNECTION_STRING"));

		});

	builder.Services.AddIdentity<IdentityUser, IdentityRole>()
.AddEntityFrameworkStores<CorreosContext>()
	.AddDefaultTokenProviders();

	// esto puede ser el error
	builder.Services.AddScoped<JwtSecurityTokenHandlerWrapper>();

	// maybe esto
	builder.Services.AddControllers(options => {
			options.Filters.Add(typeof(JwtAuthorizeFilter));});

	var app = builder.Build();


	app.UseHttpsRedirection();


	// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

if (app.Environment.IsProduction())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHttpsRedirection();
	app.UseHsts();
}

	app.UseCors(policy => policy
			.AllowAnyOrigin()
			.AllowAnyMethod()
			.AllowAnyHeader());


	app.UseAuthentication();
	app.UseAuthorization();

	app.MapControllers();

	app.MapGet("/security/getMessage", () => "Hello World!").RequireAuthorization();

	app.Run();
