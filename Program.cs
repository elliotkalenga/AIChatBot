using ChatBot.Data;
using ChatBot.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ===============================
// DATABASE
// ===============================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// ===============================
// IDENTITY
// ===============================
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// ===============================
// MVC + SESSION
// ===============================
builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ===============================
// APPLICATION SERVICES
// ===============================
builder.Services.AddScoped<KnowledgeSearchService>();
builder.Services.AddScoped<IDocumentParserService, DocumentParserService>();
builder.Services.AddScoped<IChunkingService, ChunkingService>();
builder.Services.AddScoped<KnowledgeIngestionService>();
builder.Services.AddScoped<KnowledgeFullTextSearchService>();


// (Optional background services — keep only if you use them)
builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
builder.Services.AddHostedService<QueuedHostedService>();

var app = builder.Build();

// ===============================
// PIPELINE
// ===============================
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ✅ MUST be before Authorization
app.UseSession();

app.UseAuthorization();

// ===============================
// ROUTING
// ===============================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
