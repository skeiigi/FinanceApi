using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FinanceApi.Data;

var builder = WebApplication.CreateBuilder(args);

// --- НОВОЕ: Настройка порта для Railway ---
// Railway передает порт через переменную окружения PORT. Если её нет, используем 8080.
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");

// 1. Подключение PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseNpgsql(connectionString));

// 2. Настройка Авторизации (Identity)
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<ApiDbContext>();

// 3. Настройка CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp", policy =>
    {
        // Добавь сюда адрес своего фронтенда на Vercel после деплоя!
        policy.WithOrigins("http://localhost:5173", "https://твой-фронтенд.vercel.app")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Настройка пайплайна
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowVueApp");

// --- НОВОЕ: Проверка пульса ---
// Теперь при заходе на главную страницу API ты увидишь этот текст вместо 404
app.MapGet("/", () => "AI-Financist API is running!");

app.MapIdentityApi<IdentityUser>();
app.MapControllers();

app.Run();