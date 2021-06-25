using Eklee.PasswordManager.Core;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

namespace Eklee.PasswordManager
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddScoped<IKeyVaultClient, KeyVaultClient>();
			services.AddScoped<IManagementClient, ManagementClient>();
			services.AddScoped<ClipboardService>();

			string signalRConnection = Configuration["SignalRConnection"];
			if (!string.IsNullOrEmpty(signalRConnection))
			{
				services.AddSignalR().AddAzureSignalR(signalRConnection);
			}

			services.AddApplicationInsightsTelemetry();

			services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
				.AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAd"))
				.EnableTokenAcquisitionToCallDownstreamApi(new string[]
				{
					MyConstants.UserRead,
					MyConstants.ManagementUserImpersonationScope
				})
				.AddDownstreamWebApi("KeyVault", Configuration.GetSection("KeyVaultApi"))
				.AddDownstreamWebApi("Management", Configuration.GetSection("ManagementApi"))
				.AddInMemoryTokenCaches();

			services.AddControllersWithViews()
				.AddMicrosoftIdentityUI();

			services.AddAuthorization(options =>
			{
				// By default, all incoming requests will be authorized according to the default policy
				options.FallbackPolicy = options.DefaultPolicy;
			});

			services.AddRazorPages();
			services.AddServerSideBlazor()
				.AddMicrosoftIdentityConsentHandler();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapBlazorHub();
				endpoints.MapFallbackToPage("/_Host");
			});
		}
	}
}
