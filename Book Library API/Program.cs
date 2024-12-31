using Bill_system_API.IRepositories;
using Bill_system_API.Models;
using Bill_system_API.Repositories;
using Microsoft.EntityFrameworkCore;
using Bill_system_API.MappinigProfiles;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Bill_system_API.Seeds;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Bill_system_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //Inject Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<LibraryDbContext>().AddDefaultTokenProviders();
            // Add default Schema to validate on Token
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "mySchema"; // Note: Removed space in schema name
                options.DefaultChallengeScheme = "mySchema"; // Added DefaultChallengeScheme
            })
            .AddJwtBearer("mySchema", options =>
            {
                string key = "Welcome to my secret key in Bill System"; // Ensure this key is stored securely, e.g., in app settings
                var secretKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true, // Validate the token expiration
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = secretKey
                };
            });


            // Add services to the container.
            builder.Services.AddDbContext<LibraryDbContext>(op =>
                op.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("book_library_db")));

        builder.Services.AddControllers().AddNewtonsoftJson(op =>
                op.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // Register the EmailService with the necessary configuration
            builder.Services.AddScoped<IEmailService, EmailService>(provider =>
                new EmailService(
                    builder.Configuration["Smtp:Server"],
                    int.Parse(builder.Configuration["Smtp:Port"]),
                    builder.Configuration["Smtp:Username"],
                    builder.Configuration["Smtp:Password"]
                ));

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>(); // IUnitOfWork

            // AutoMapper configuration
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile(new BookProfile());
                cfg.AddProfile(new BorrowedBookProfile());
            });

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            // Add Swagger services
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Book Library API",
                    Description = "API Documentation for Book Library API Project"
                });

                // Include XML comments if available
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            // Register repositories
            var types = new[] { typeof(Book), typeof(BorrowedBook) };

            foreach (var type in types)
            {
                var interfaceType = typeof(IGenericRepository<>).MakeGenericType(type);
                var implementationType = typeof(GenericRepository<>).MakeGenericType(type);
                builder.Services.AddScoped(interfaceType, implementationType);
            }

            var app = builder.Build();

            // Seed roles
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                SeedRoles.Initialize(services, userManager).Wait();
            }
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Book Library V1");
                    c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // Use CORS
            app.UseCors("AllowAllOrigins");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
