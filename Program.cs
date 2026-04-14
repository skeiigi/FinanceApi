using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FinanceApi.Data; // �������, ��� ������ ����� Data � ��������

var builder = WebApplication.CreateBuilder(args);

// 1. ����������� PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseNpgsql(connectionString));

// 2. ��������� ����������� (Identity)
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<ApiDbContext>();

// 3. ──────── CORS (позволить Vue на localhost:5173 и других адресах)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000", "http://localhost:8080") // Указываем конкретные origins
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Теперь можно использовать с конкретными origins
    });
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// ��������� ���������
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowVueApp");
//app.UseHttpsRedirection();

// ��������� ��� ����������� � ������ (��������� ������������� ����� �������)
app.MapIdentityApi<IdentityUser>();

app.MapControllers(); // ����� �������� ���� FinanceController � ������

app.Run();