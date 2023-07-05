using Azure.Identity;
using Blog_App_Dev.Data;
using Blog_App_Dev.Models;
using Blog_App_Dev.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Defualt connection string for production DB
// var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Dev connection string for local DB
// var connectionString = builder.Configuration.GetConnectionString("_DevConnectionString");
var azureOptions = new DefaultAzureCredentialOptions
{
    ManagedIdentityClientId = builder.Configuration["AzureKeyVault:AzureADManagedIdentityClientId"],
    TenantId = builder.Configuration["AzureKeyVault:TennantId"],
    AdditionallyAllowedTenants = { "*" }
};
var credential = new DefaultAzureCredential();

builder.Configuration.AddAzureKeyVault(new Uri(builder.Configuration["AzureKeyVault:VaultURL"]), credential);
try
{
    // Remove "Dev" from connection string name to get production DB
    string dbName = builder.Configuration["AzureKeyVault:ConnectionStringName"];
    var connectionString = builder.Configuration[dbName];

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
}
catch (Exception ex)
{
    Console.WriteLine($"Failed to get connection string: {ex.Message}");
}

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

/*
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
*/

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    
    options.SignIn.RequireConfirmedEmail = true;
    options.SignIn.RequireConfirmedAccount = true;
    
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);
builder.Services.AddRazorPages();
builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roleNames = new[] { "Admin", "User", "Guest" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseDeveloperExceptionPage();
    // app.UseExceptionHandler("/Error");
    // app.UseStatusCodePagesWithReExecute("/Error/{0}");
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseStatusCodePagesWithReExecute("/Error/{0}");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();


app.Run();
