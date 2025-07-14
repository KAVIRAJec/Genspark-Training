using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Freelance_Project.Services
{
    public class KeyVaultService
    {
        private readonly IConfiguration _configuration;
        private readonly SecretClient _secretClient;

        public KeyVaultService(IConfiguration configuration)
        {
            _configuration = configuration;
            var keyVaultUrl = _configuration["Azure:KeyVaultUrl"];
            
            if (string.IsNullOrEmpty(keyVaultUrl))
                throw new InvalidOperationException("Azure Key Vault URL is not configured");
                
            _secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
        }

        public async Task<string> GetSecretAsync(string secretName)
        {
            try
            {
                var secret = await _secretClient.GetSecretAsync(secretName);
                return secret.Value.Value;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to retrieve secret '{secretName}' from Key Vault: {ex.Message}", ex);
            }
        }
    }
}