using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using SoftwareTracker.Data;
using Hangfire;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = AkeylessHelper.RetrieveSecret("ConnectionString"); //builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddTransient<EmailSender>();
builder.Services.AddDefaultIdentity<IdentityUser>(options =>options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddLogging();
builder.Services.AddScoped<LicenseHelper>();
builder.Services.AddAuthentication().AddGoogle(options => 
{
    options.ClientId = AkeylessHelper.RetrieveSecret("Google-ClientID");
    options.ClientSecret = AkeylessHelper.RetrieveSecret("Google-ClientSecret");
});
builder.Services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(connectionString));
builder.Services.AddHangfireServer();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});
builder.WebHost.UseIIS();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseForwardedHeaders();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseForwardedHeaders(); 
    app.UseHsts();
}
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    db.Database.Migrate();
    await Seeder.CreateRoles(roleManager, userManager, logger);
    Seeder.SeedUsers(userManager, db, logger);
}
app.UseHangfireDashboard();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();


RecurringJob.AddOrUpdate<LicenseHelper>("DailyLicenseExpirationScan", x => x.LicenseScan(), Cron.Daily, new RecurringJobOptions {  TimeZone = TimeZoneInfo.Local});
app.Run();
