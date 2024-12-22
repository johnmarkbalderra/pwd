using FluentEmail.Core;
using FluentEmail.Razor;
using FluentEmail.Smtp;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PWD_DSWD_SPC;
using PWD_DSWD_SPC.Data;
using System.Net.Mail;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register your DbContext with SQL Server configuration
builder.Services.AddDbContext<RegisterDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PWDConnectionString")));

// Configure FluentEmail with SMTP settings
builder.Services.AddFluentEmail("dswdpwdspc@gmail.com")
    .AddRazorRenderer()
    .AddSmtpSender(new SmtpClient
    {
        Host = builder.Configuration["Smtp:SmtpServer"],
        Port = int.Parse(builder.Configuration["Smtp:SmtpPort"]),
        EnableSsl = true,
        DeliveryMethod = SmtpDeliveryMethod.Network,
        UseDefaultCredentials = false,
        Credentials = new System.Net.NetworkCredential(
            builder.Configuration["Smtp:SmtpUser"],
            builder.Configuration["Smtp:SmtpPass"]
        )
    });

// Register the email service interface and its implementation
builder.Services.AddTransient<IEmailService, EmailService>();

// Add session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // HTTP Strict Transport Security
}

app.UseHttpsRedirection(); // Redirect HTTP to HTTPS
app.UseStaticFiles(); // Serve static files

app.UseRouting(); // Add routing middleware

// Use session before authorization
app.UseSession(); // Add session middleware

app.UseAuthorization(); // Add authorization middleware

// Map default controller routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Landing}/{action=Landing}/{id?}");

app.Run(); // Run the application
