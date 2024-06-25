using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using System.Web;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System;
using Microsoft.AspNetCore.Authorization;
using WebApplication1.Controllers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.FileSystemGlobbing.Internal;

var builder = WebApplication.CreateBuilder(args);

// ������� ����� ����������
string? connection = builder.Configuration.GetConnectionString("DefaultConnection");
// ����� �������� � ������
//builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlite(connection));
// Add services to the container.
builder.Services.AddControllersWithViews();
// �������������� � ��������� ����
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => options.LoginPath = "/login");
builder.Services.AddAuthorization();
var app = builder.Build();
app.UseAuthentication();   // ��������� middleware �������������� 
app.UseAuthorization();   // ��������� middleware ����������� 
app.MapGet("/login", async (HttpContext context) =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    // html-����� ��� ����� ������/������
    string loginForm = @"<!DOCTYPE html>
    <html>
    <head>
        <meta charset='utf-8' />
<link rel='stylesheet' href='/css/site.css' asp-append-version='true' />
        <title>�����������</title>
    </head>
    <body>
        <h2></h2>
<form method='post' class='styled-form'>
    <p>
        <label>����</label><br />
        <input name='login' class='input-field' />
    </p>
    <p>
        <label>������</label><br />
        <input type='password' name='password' class='input-field' />
    </p>
    <input type='submit' value='�����������' class='submit-btn' />
</form>
    </body>
    </html>";
    await context.Response.WriteAsync(loginForm);
});

app.MapPost("/login", async (string? returnUrl, HttpContext context) =>
{
    // �������� ���� � ������
    var form = context.Request.Form;
    // ���� ���� �� ������ �� ����������, ����������� ��������� ��� ������� 400
    if (!form.ContainsKey("login") || !form.ContainsKey("password"))
        return Results.BadRequest("������ �� ���� �� ������");
    string? login = form["login"];
    string? password = form["password"];
    if (login != "Manager" || password != "pass") return Results.Unauthorized();
    var claims = new List<Claim> { new Claim(ClaimTypes.Name, login) };
    // �������� ClaimsIdentity
    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
    // ��������� ����������������� ���
    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
    return Results.Redirect(returnUrl ?? "/");
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapGet("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
});
//app.Map("/manager", [Authorize] () => $"³���� ���! �� �����������");
app.MapControllerRoute(name: "default",pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
