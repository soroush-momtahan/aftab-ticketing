using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Aftab.App.Infrastructure.Data.ReadDb;

public class AftabReadDbContextFactory : IDesignTimeDbContextFactory<AftabReadDbContext>
{
    public AftabReadDbContext CreateDbContext(string[] args)
    {
        // مسیر پایه همان مسیری است که دستور CLI از آن اجرا می‌شود (Root فولدر)
        string basePath = Directory.GetCurrentDirectory();
        
        // مسیر فایل تنظیمات در پروژه Api را مشخص می‌کنیم
        string apiProjectPath = Path.Combine(basePath);

        // ساخت دستی Configuration برای خواندن appsettings.json
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(apiProjectPath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var builder = new DbContextOptionsBuilder<AftabReadDbContext>();
        
        // ⚠️ مهم: نام کلید کانکشن استرینگ خود را بنویسید (مثلا "WriteDbConnection")
        string? connectionString = configuration.GetConnectionString("aftab-read-db"); 

        builder.UseNpgsql(connectionString);

        // نکته طلایی: برای مایگریشن زدن نیازی به اضافه کردن Interceptor مربوط به Outbox نداریم!
        // این کار باعث می‌شود از شر خطاهای وابستگی در زمان دیزاین خلاص شویم.

        return new AftabReadDbContext(builder.Options);
    }
}
