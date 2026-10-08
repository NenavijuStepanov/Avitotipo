using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;                    // этот файл - это точка входа в приложение
using Microsoft.Extensions.DependencyInjection;
using webasp.Data;
using webasp.Models;

var builder = WebApplication.CreateBuilder(args);    // создает главный строитель приложения. Он автоматически считывает настройки из appsettings.json,
                                                     // переменные окружения, настраивает встроенный веб-сервер Kestrel и систему логирования.
builder.Services.AddRazorPages();   // поддержка html-рендеринга
builder.Services.AddControllers();  // поддержка rest-api

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")               // достает строку подключения
    ?? throw new InvalidOperationException("Строка подключения 'DefaultConnection' не найдена.");  

builder.Services.AddDbContext<DB>(options =>                                             // штука, чтоб проверить работу бд и подстроиться под версию mysql
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();   // регистрация хешера паролей



builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) // Настройка cookie-аутентификации
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";            // на какую страницу бросать, если гость зайдет на страницу с [Authorize]
        options.Cookie.Name = "UserAuthCookie";          // имя браузерного файла cookie
        options.ExpireTimeSpan = TimeSpan.FromDays(7);    // время жизни сессии (кука будет действительна 7 дней)
    });



var app = builder.Build();     // фиксирует настройки сервисов и «собирает» готовый объект приложения app

using (var scope = app.Services.CreateScope())  // создаем временный объект, так как еще не запущено приложение, чтоб настроить бд до старта
                                                // мы вручную говорим .NET: "Создай мне временную 'песочницу' (Scope), как будто к нам только что пришел виртуальный HTTP-запрос."
{
    var db = scope.ServiceProvider.GetRequiredService<DB>();  // запрашивает контекст бд
    await db.Database.MigrateAsync();   // вызывает MigrateAsync(), который проверяет MySQL и асинхронно накатывает все недостающие Entity Framework миграции
                                        // база данных всегда будет иметь актуальную структуру таблиц без необходимости вручную выполнять команды в консоли
}


    if (!app.Environment.IsDevelopment())  // если проект запущен не в режиме разработки (Production), скрывать детальные ошибки за стандартной страницей /Error
                                           // и принудительно заставлять браузер использовать защищенное соединение HSTS
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();   // перенаправляет незащищенный трафик с http:// на https://
app.MapControllers();   // подключает маршруты API-контроллеров

app.UseRouting();   // анализирует URL и определяет, какой именно компонент должен его обрабатывать

app.UseAuthentication(); // читает куку UserAuthCookie, расшифровывает её и определяет, КТО этот пользователь (заполняет User.Identity)
app.UseAuthorization(); // проверяет, ЕСТЬ ЛИ ПРАВА у этого пользователя (проверяет атрибуты [Authorize])

// !!!Если поменять их местами, авторизация перестанет работать, так как система попытается проверить права еще до того, как узнает, кто совершает запрос!!!

app.MapStaticAssets();          // cовременный встроенный оптимизатор статических файлов (.NET 9): 
app.MapRazorPages()             // эффективно сжимает и отдаёт из папки wwwroot все CSS, JS, иконки и картинки,
   .WithStaticAssets();         // а также привязывает маршруты Razor Pages к этим ресурсам



app.Run(); // ну тут понятно
