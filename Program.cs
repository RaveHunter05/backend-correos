global using correos_backend.Helpers;
global using correos_backend.Decorators;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;

using dotenv.net;
using correos_backend.Middleware;

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
		options.UseSqlServer(Environment.GetEnvironmentVariable("CONNECTION_STRING"));

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

if (app.Environment.IsProduction())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHttpsRedirection();
	app.UseHsts();
}

app.UseCors();



app.UseAuthentication();
app.UseMiddleware<CheckUserIsActiveMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/security/getMessage", () => "Hello World!").RequireAuthorization();

using (var scope = app.Services.CreateScope())
{
	var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

	var users = userManager.Users.ToList();

	foreach (var user in users)
	{
		if (userManager.GetRolesAsync(user).Result.Count == 0)
		{
			userManager.AddToRoleAsync(user, "Manager").Wait();
		}
	}
}

app.Run();
