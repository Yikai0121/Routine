using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Routine.APi.Data;
using Routine.APi.Services;

namespace Routine
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
            services.AddControllers(setup =>
            {
                setup.ReturnHttpNotAcceptable = true; //��^�Ȥ��OJson�榡��^406
                //setup.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());//�[�Jxml��^�榡
                //setup.OutputFormatters.Insert(0, new XmlDataContractSerializerOutputFormatter()); �w�]�]�w��XML
            }).AddXmlDataContractSerializerFormatters();//.net core �s�W��kinput�Bouput���|�s�W
            services.AddScoped<ICompanyRepository, CompanyRepository>();

            services.AddDbContext<RoutineDbContext>(option =>
            {
                option.UseSqlite("Data Source=routine.db");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}