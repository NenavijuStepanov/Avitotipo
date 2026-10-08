using Microsoft.EntityFrameworkCore;
using webasp.Models;
namespace webasp.Data
{
    public class DB : DbContext    // класс для работы с бд, наследует все от DbContext, поэтому файл такой пустой
    {
        public DB(DbContextOptions<DB> options) : base(options)  // запускает конструктор от род класса, чтоб тот схавал строку подключения к бд
        {
        }
        public DbSet<User> Users { get; set; } // таблица Users в MySQL

        public DbSet<Ad> Ads { get; set; } // таблица Ads в MySQL

        // эти 2 строки это обертки таблиц для программы, чтоб программа сама формировала sql запросы при использовании функций

    }
}