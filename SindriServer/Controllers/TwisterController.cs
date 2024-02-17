using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        public async Task<IActionResult> PostProof([FromBody] PublicInputDto publicInput)
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
                ProofResultDto proofGeneration = JsonSerializer.Deserialize<ProofResultDto>(responseBody);

                string proofId = proofGeneration.ProofId;
                _logger.LogInformation("Generation successfull");

                if (proofGeneration.Proof?.ProofResult != null)
                {
                    return Ok(proofGeneration.Proof);
                }

                int restart = 0;
                do
                {
                    try
                    {
                        string responseBodyProof = await GetProof(proofId);
                        ProofResultDto proofGenerationProof = JsonSerializer.Deserialize<ProofResultDto>(responseBodyProof);
                        if (proofGenerationProof.Proof?.ProofResult != null)
                        {
                            return Ok(proofGenerationProof.Proof);
                        }
                    }
                    catch (Exception)
                    {

                    }
                    finally { restart++; }
                    await Task.Delay(2000);

                } while (!(restart > 10));

                _logger.LogInformation("Get result successfull");

                return StatusCode(408, "Proof generation take more time than expected, try generate proof on web browser.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }


        }


        [HttpGet]
        public async Task<string> Get(string proofId)
        {
            return await GetProof(proofId);
        }

        private async Task<string> GetProof(string proofId)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://sindri.app/api/v1/proof/{proofId}/detail?time={DateTime.Now.Ticks}");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer {_configuration["ApiKey"]}");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();
            return result;
        }
    }
}
