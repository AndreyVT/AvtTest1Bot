using AvtTest1Bot.Model;
using AvtTest1Bot.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AvtTest1Bot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // получаем строку подключения из файла конфигурации
            string connection = Configuration.GetConnectionString("DefaultConnection");
            // добавляем контекст MobileContext в качестве сервиса в приложение
            services.AddDbContext<BotDbContext>(options =>
                options.UseSqlServer(connection));

            services.AddMvc();
            // добавим в контейнер сервисы
            services.AddScoped<IUpdateService, UpdateService>();

            services.AddSingleton<IBotService, BotService>();
            services.AddSingleton<ICommandManager, CommandManager>();

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IMapper, Mapper>();

            IConfigurationSection confSection = Configuration.GetSection("BotConfiguration");
            services.Configure<BotConfiguration>(confSection);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // создадим БД если нет
            using (IServiceScope scope = app.ApplicationServices.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<BotDbContext>();
                bool isDBCreated = dbContext.Database.EnsureCreated();
            }

            app.UseMvc();
        }
    }
}
