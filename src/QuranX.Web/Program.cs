using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Trace;
using QuranX.Web.McpTools;
using QuranX.Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);

QuranX.Persistence.Services.Registration.Register(builder.Services);
QuranX.Web.Services.Registration.Register(builder.Environment, builder.Services);

if (!builder.Environment.IsDevelopment())
{
    builder
    .Services
    .AddOpenTelemetry()
    .WithTracing(builder => builder
        //.AddSource("Experimental.ModelContextProtocol")
        //.AddSource("QuranX.Mcp")
        .AddSource("*")
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter()
        .SetSampler<AlwaysOnSampler>()
     );
    }

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services
  .AddMcpServer(McpServerRegistration.Register)
  .WithHttpTransport(options => options.Stateless = true)
  .WithToolsFromAssembly();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.WebRootPath, ".well-known")),
    RequestPath = "/.well-known",
    ServeUnknownFileTypes = true
});

app.UseRouting();
app.UseMiddleware<OpenTelemetryEnrichmentMiddleware>();
app.UseWhen(
    context => context.Request.Path.StartsWithSegments("/mcp"),
    branch => branch.UseMiddleware<McpTracingMiddleware>());

app.MapMcp("/mcp");

app.MapControllerRoute(
    name: "About",
    pattern: "About",
    defaults: new { controller = "About", action = "Index" });

app.MapControllerRoute(
    name: "Help",
    pattern: "Help",
    defaults: new { controller = "Help", action = "Index" });

app.MapControllerRoute(
    name: "SiteSearch",
    pattern: "Search",
    defaults: new { controller = "SiteSearch", action = "Index" });

app.MapControllerRoute(
    name: "Commentators",
    pattern: "Tafsirs",
    defaults: new { controller = "Commentators", action = "Index" });

app.MapControllerRoute(
    name: "CommentariesForVerse",
    pattern: "Tafsirs/{ChapterNumber:int}.{VerseNumber:int}",
    defaults: new { controller = "CommentariesForVerse", action = "Index" });

app.MapControllerRoute(
    name: "VerseAnalysis",
    pattern: "Analysis/{ChapterNumber:int}.{VerseNumber:int}",
    defaults: new { controller = "VerseAnalysis", action = "Index" });

app.MapControllerRoute(
    name: "RootAnalysis",
    pattern: "Analysis/Root/{rootLetterNames}",
    defaults: new { controller = "RootAnalysis", action = "Index" });

app.MapControllerRoute(
    name: "VerseCommentary",
    pattern: "Tafsir/{CommentatorCode}/{ChapterNumber:int}.{VerseNumber:int}",
    defaults: new { controller = "VerseCommentary", action = "Index" });

app.MapControllerRoute(
    name: "HadithCollections",
    pattern: "Hadiths",
    defaults: new { controller = "HadithCollections", action = "Index" });

app.MapControllerRoute(
    name: "VerseHadiths",
    pattern: "Hadiths/{ChapterNumber:int}.{VerseNumber:int}",
    defaults: new { controller = "VerseHadiths", action = "Index" });

app.MapControllerRoute(
    name: "HadithIndex3",
    pattern: "Hadith/{CollectionCode}/{ReferenceCode}/{ReferenceValue1}/{ReferenceValue2}/{ReferenceValue3}",
    defaults: new { controller = "HadithIndex", action = "Index" },
    constraints: new
    {
        ReferenceValue1 = new RegexRouteConstraint("^[a-z]+-\\d+[a-z]*$"),
        ReferenceValue2 = new RegexRouteConstraint("^[a-z]+-\\d+[a-z]*$"),
        ReferenceValue3 = new RegexRouteConstraint("^[a-z]+-\\d+[a-z]*$")
    });

app.MapControllerRoute(
    name: "HadithIndex2",
    pattern: "Hadith/{CollectionCode}/{ReferenceCode}/{ReferenceValue1}/{ReferenceValue2}",
    defaults: new { controller = "HadithIndex", action = "Index" },
    constraints: new
    {
        ReferenceValue1 = new RegexRouteConstraint("^[a-z]+-\\d+[a-z]*$"),
        ReferenceValue2 = new RegexRouteConstraint("^[a-z]+-\\d+[a-z]*$")
    });

app.MapControllerRoute(
    name: "HadithIndex1",
    pattern: "Hadith/{CollectionCode}/{ReferenceCode}/{ReferenceValue1}",
    defaults: new { controller = "HadithIndex", action = "Index" },
    constraints: new
    {
        ReferenceValue1 = new RegexRouteConstraint("^[a-z]+-\\d+[a-z]*$")
    });

app.MapControllerRoute(
    name: "HadithIndex",
    pattern: "Hadith/{CollectionCode}/{ReferenceCode}",
    defaults: new { controller = "HadithIndex", action = "Index" });

app.MapControllerRoute(
    name: "DictionaryEntry",
    pattern: "Dictionary/{DictionaryCode}/{Word}",
    defaults: new { controller = "DictionaryEntry", action = "Index" });

app.MapControllerRoute(
    name: "QuranVerses",
    pattern: "{*Verses}",
    defaults: new { controller = "QuranVerses", action = "Index" },
    constraints: new
    {
        Verses = new RegexRouteConstraint(@"^(\d+\.\d+(-\d+)?)(,(\d+\.\d+(-\d+)?))*$")
    });

app.MapControllerRoute(
    name: "Dictionaries",
    pattern: "Dictionaries/{Root?}",
    defaults: new { controller = "Dictionaries", action = "Index" });

app.MapControllerRoute(
    name: "Home",
    pattern: "",
    defaults: new { controller = "QuranVerses", action = "Index", Verses = "1.1" });

// Optional: Default route at the end
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
