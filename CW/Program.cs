using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews()
    .AddViewOptions(options =>
    {
        options.HtmlHelperOptions.ClientValidationEnabled = true; 
    });

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.Use(async (context, next) =>
{
    var userIdCookie = context.Request.Cookies["UserId"];
    var usernameCookie = context.Request.Cookies["Username"];
    var isAdminCookie = context.Request.Cookies["IsAdmin"];

    
    if (!string.IsNullOrEmpty(userIdCookie) && !string.IsNullOrEmpty(usernameCookie) && context.Session.GetInt32("UserId") == null)
    {
        context.Session.SetInt32("UserId", int.Parse(userIdCookie));
        context.Session.SetString("Username", usernameCookie);
        context.Session.SetString("IsAdmin", isAdminCookie);
    }

    await next();
});


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
