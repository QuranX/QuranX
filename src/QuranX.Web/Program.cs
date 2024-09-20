using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

QuranX.Persistence.Services.Registration.Register(builder.Services);
QuranX.Web.Services.Registration.Register(builder.Environment, builder.Services);

if (!builder.Environment.IsDevelopment())
{
	builder.Services.AddOpenTelemetry().WithTracing(builder => builder
	.AddAspNetCoreInstrumentation()
	.AddHttpClientInstrumentation()
	.AddOtlpExporter());
}

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

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

app.UseRouting();

app.UseAuthorization();

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
	name: "Home",
	pattern: "",
	defaults: new { controller = "RedirectToUrl", action = "Index", url = "/1.1" });

// Optional: Default route at the end
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
