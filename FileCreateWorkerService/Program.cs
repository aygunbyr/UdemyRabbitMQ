using FileCreateWorkerService.Models;
using FileCreateWorkerService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileCreateWorkerService
{
    public class Program
    {
        // 404 Hatas�n�n ��z�m�
        // �nce ExcelCreate uygulamas�n� tek ba��na aya�a kald�r�p excel dosyas� olu�tur
        // B�ylece Queue olu�ur ve art�k FileCreateWorkerService hata vermez
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration Configuration = hostContext.Configuration;

                    services.AddDbContext<AdventureWorks2019Context>(options =>
                    {
                        options.UseSqlServer(Configuration.GetConnectionString("SqlServer"));
                    });

                    services.AddSingleton<RabbitMQClientService>();
                    services.AddSingleton(sp => new ConnectionFactory()
                    {
                        Uri = new Uri(Configuration.GetConnectionString("RabbitMQ")),
                        DispatchConsumersAsync = true
                    });
                    services.AddHostedService<Worker>();
                });
    }
}
