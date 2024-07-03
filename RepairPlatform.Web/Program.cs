using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RepairPlatform.Data.Contexts;
using RepairPlatform.Entities;
using RepairPlatform.Services;
using RepairPlatform.Services.Mapping;
using System.Text.RegularExpressions;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<Repairguy20118046Context>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


//registers RepairguysService with the DI container as a scoped service.
//This means a new instance of RepairguysService will be created for each HTTP request.
builder.Services.AddScoped<RepairguysService>();

builder.Services.AddScoped<ClientsService>();

builder.Services.AddScoped<ReservationsService>();

builder.Services.AddScoped<ReviewsService>();

builder.Services.AddScoped<AdministratorsService>();

builder.Services.AddIdentity<AspNetUsers, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 5;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
//.AddRoles<IdentityRole>() // Enable roles
.AddEntityFrameworkStores<Repairguy20118046Context>()
.AddDefaultTokenProviders();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(options =>
{
    options.LoginPath = "/Login_Logout/Login";
    options.AccessDeniedPath = "/Views/Public/AccessDenied";
    options.LogoutPath = "/Login_Logout/Logout";
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthorization();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireRepairGuyRole", policy => policy.RequireRole("repairguy"));
    options.AddPolicy("RequireClientRole", policy => policy.RequireRole("client"));
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("admin"));
});

builder.Services.AddAutoMapper(typeof(MappingProfile));


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AspNetUsers>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    // Ensure the "client" role exists
    var roleExists = await roleManager.RoleExistsAsync("client");
    if (!roleExists)
    {
        await roleManager.CreateAsync(new IdentityRole("client"));
    }

    // Ensure the "admin" role exists
    roleExists = await roleManager.RoleExistsAsync("admin");
    if (!roleExists)
    {
        await roleManager.CreateAsync(new IdentityRole("admin"));
    }

    // Ensure the "repairguy" role exists
    roleExists = await roleManager.RoleExistsAsync("repairguy");
    if (!roleExists)
    {
        await roleManager.CreateAsync(new IdentityRole("repairguy"));
    }

    // Create a default user to assign the role to
    var adminUser = new AspNetUsers { UserName = "admin@example.com", Email = "admin@example.com" };
    var user = await userManager.FindByEmailAsync("admin@example.com");
    if (user == null)
    {
        var result = await userManager.CreateAsync(adminUser, "Admin123!");
        if (result.Succeeded)
        {
            user = adminUser;
        }
    }

    // Assign the role to the user
    if (user != null)
    {
        await userManager.AddToRoleAsync(user, "admin");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.Use(async (context, next) =>
{
    context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
    context.Response.Headers["Pragma"] = "no-cache";
    context.Response.Headers["Expires"] = "0";
    await next();
});


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapWhen(context =>
    Regex.IsMatch(context.Request.Path, "^/admin$", RegexOptions.IgnoreCase),
    appBuilder =>
    {
        appBuilder.Run(async context =>
        {
            context.Response.Redirect("/Login_Logout/AdminLogin");
            await Task.CompletedTask;
        });
    });

app.MapRazorPages();


app.Run();

