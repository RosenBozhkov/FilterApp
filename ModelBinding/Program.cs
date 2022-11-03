using Microsoft.Extensions.DependencyInjection;

namespace ModelBinding;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers()
        .AddNewtonsoftJson(
      //      jsonOptions =>
      //  {
      //      jsonOptions.SerializerSettings.Converters.Add(new StringEnumConverter());
      //  }
        );


        //builder.Services.AddControllers().AddNewtonsoftJson(IMvcBuilder);

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }



        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}