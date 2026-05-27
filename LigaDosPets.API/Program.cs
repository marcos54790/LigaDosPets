using LigaDosPets.Application.Abstractions.Identity;
using LigaDosPets.Application.Abstractions.Services;
using LigaDosPets.Application.Services;
using LigaDosPets.Infrastructure;
using LigaDosPets.Infrastructure.Identity;
using LigaDosPets.Infrastructure.Persistence;
using LigaDosPets.Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ==========================================================================
// 1. Infraestrutura (DbContext, UnitOfWork, Repositórios)
// ==========================================================================
builder.Services.AddInfrastructure(builder.Configuration);

// ==========================================================================
// 2. Identity
// ==========================================================================
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Login";
    options.AccessDeniedPath = "/Auth/AccessDenied";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
});

// ==========================================================================
// 3. Serviço de Identidade (Abstração do Identity)
//    ⚠️ ESSENCIAL para UsuarioService!
// ==========================================================================
builder.Services.AddScoped<IIdentityService, IdentityService>();

// ==========================================================================
// 4. Serviços da Camada de Aplicação
// ==========================================================================
builder.Services.AddScoped<IAnimalService, AnimalService>();
builder.Services.AddScoped<IAdocaoService, AdocaoService>();
builder.Services.AddScoped<IAgendamentoService, AgendamentoService>();
builder.Services.AddScoped<IConversaService, ConversaService>();
builder.Services.AddScoped<INotificacaoService, NotificacaoService>();
builder.Services.AddScoped<ISolicitacaoEntregaAppService, SolicitacaoEntregaService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IPorteService, PorteService>();
builder.Services.AddScoped<IEspecieService, EspecieService>();
builder.Services.AddScoped<IRacaService, RacaService>();
builder.Services.AddScoped<ICaracteristicaService, CaracteristicaService>();
builder.Services.AddScoped<IFotoService, FotoService>();

// ==========================================================================
// 5. MVC
// ==========================================================================
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ==========================================================================
// 6. Pipeline de Middlewares
// ==========================================================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ==========================================================================
// 7. Seed do Banco de Dados (com Migração automática)
// ==========================================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await context.Database.MigrateAsync();
        await DataBaseSeeder.SeedAsync(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Erro ao executar o seed do banco de dados.");
    }
}

app.Run();