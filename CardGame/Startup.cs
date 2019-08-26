using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;
using CardGame.API.Hubs;
using CardGame.Entities;
using CardGame.Repositories;
using CardGame.Repositories.Interfaces;
using CardGame.Services;
using CardGame.Services.Interfaces;

namespace CardGame
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			 .AddJwtBearer(options =>
			 {
				 options.TokenValidationParameters = new TokenValidationParameters
				 {
					 ValidateIssuer = true,
					 ValidateAudience = true,
					 ValidateLifetime = true,
					 ValidateIssuerSigningKey = true,
					 ValidIssuer = Configuration["Jwt:Issuer"],
					 ValidAudience = Configuration["Jwt:Issuer"],
					 IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
				 };
				 options.Events = new JwtBearerEvents
				 {
					 OnMessageReceived = context =>
					 {
						 var accessToken = context.Request.Query["access_token"];
						 var path = context.HttpContext.Request.Path;
						 if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/hub")))
							 context.Token = accessToken;

						 return Task.CompletedTask;
					 }
				 };
			 });
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

			//var connection = @"Server=(localdb)\mssqllocaldb;Database=EFGetStarted.AspNetCore.NewDb;Trusted_Connection=True;ConnectRetryCount=0";
			services.AddDbContext<CardsContext>(options => options.UseInMemoryDatabase("CardGameDb"));

			services.AddScoped<ICardsRepository, CardsRepository>();
			services.AddScoped<IGameService, GameService>();
			services.AddScoped<IPlayerService, PlayerService>();
			services.AddScoped<IAuthService, AuthService>();
			services.AddSignalR();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseHsts();
			}
			app.UseStaticFiles();
			app.UseHttpsRedirection();
			app.UseAuthentication();
			app.UseSignalR(routes =>
			{
				routes.MapHub<GameHub>("/hub");
			});
			app.UseMvc();
		}
	}
}