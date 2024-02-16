using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;

namespace SindriServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TwisterController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly ILogger<TwisterController> _logger;
        public TwisterController(IConfiguration configuration, ILogger<TwisterController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
        public async Task<string> GetProof([FromBody] PublicInputDto publicInput)
        {
            try
            {
                HttpClient client = new HttpClient();

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"https://sindri.app/api/v1/circuit/{_configuration["CircuitId"]}/prove");

                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("Authorization", $"Bearer {_configuration["ApiKey"]}");

                string data = $"proof_input=secret= \"{publicInput.Secret}\"\noldAmount= \"{publicInput.OldAmount}\"\nwitnesses= [{string.Join(",", publicInput.Witnesses)}]\nleafIndex= \"{publicInput.LeafIndex}\"\nleaf= \"{publicInput.Leaf}\"\nmerkleRoot= \"{publicInput.MerkleRoot}\"\nnullifier= \"{publicInput.Nullifier}\"\namount= \"{publicInput.Amount}\"\nreceiver= \"{publicInput.Receiver}\"\nrelayer= \"{publicInput.Relayer}\"\ndeposit= \"{publicInput.Deposit}\"";

                request.Content = new StringContent(data);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                HttpResponseMessage response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Generation successfull");
                return responseBody;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }


        }
    }
}
