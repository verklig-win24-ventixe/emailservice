using System.Security.Cryptography;
using Azure.Communication.Email;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Presentation.Interfaces;
using Presentation.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var keyVaultUrl = "https://verklig-ventixe-keyvault.vault.azure.net/";
builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUrl), new DefaultAzureCredential());

var client = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
KeyVaultSecret acsSecret = await client.GetSecretAsync("ACSConnectionString-Ventixe");
KeyVaultSecret jwtKeySecret = await client.GetSecretAsync("JwtPublicKey");
KeyVaultSecret issuerSecret = await client.GetSecretAsync("JwtIssuer");
KeyVaultSecret audienceSecret = await client.GetSecretAsync("JwtAudience");

var rsa = RSA.Create();
rsa.ImportFromPem(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(jwtKeySecret.Value)));

var signingKey = new RsaSecurityKey(rsa);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
  options.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = issuerSecret.Value,
    ValidAudience = audienceSecret.Value,
    IssuerSigningKey = signingKey
  };
});

builder.Services.AddSingleton(x => new EmailClient(acsSecret.Value));
builder.Services.AddTransient<IVerificationService, VerificationService>();

var app = builder.Build();
app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
  c.SwaggerEndpoint("/swagger/v1/swagger.json", "Event Service API");
  c.RoutePrefix = string.Empty;
});
app.UseHttpsRedirection();
app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();