using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<string> GetProof([FromBody] PublicInputDto publicInput)
        {
            try
            {
                HttpClient client = new HttpClient();

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"https://sindri.app/api/v1/circuit/{_configuration["CircuitId"]}/prove");

                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("Authorization", $"Bearer {_configuration["ApiKey"]}");

                string data = $"proof_input=secret= \"{publicInput.Secret}\"\noldAmount= \"{publicInput.OldAmount}\"\nwitnesses= [{string.Join(",", publicInput.Witnesses)}]\nleafIndex= \"{publicInput.LeafIndex}\"\nleaf= \"{publicInput.Leaf}\"\nmerkleRoot= \"{publicInput.MerkleRoot}\"\nnullifier= \"{publicInput.Nullifier}\"\namount= \"{publicInput.Amount}\"\nreceiver= \"{publicInput.Receiver}\"\nrelayer= \"{publicInput.Relayer}\"\ndeposit= \"{publicInput.Deposit}\"";

     
                var collection = new List<KeyValuePair<string, string>>();
                collection.Add(new("perform_verify", "true"));
                collection.Add(new("proof_input", $"secret= \"{publicInput.Secret}\"\noldAmount= \"{publicInput.OldAmount}\"\nwitnesses= [{string.Join(", ", publicInput.Witnesses)}]\nleafIndex= \"{publicInput.LeafIndex}\"\nleaf= \"{publicInput.Leaf}\"\nmerkleRoot= \"{publicInput.MerkleRoot}\"\nnullifier= \"{publicInput.Nullifier}\"\namount= \"{publicInput.Amount}\"\nreceiver= \"{publicInput.Receiver}\"\nrelayer= \"{publicInput.Relayer}\"\ndeposit= \"{publicInput.Deposit}\""));
                request.Content = new FormUrlEncodedContent(collection);

                HttpResponseMessage response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                ProofResultDto proofGeneration = JsonSerializer.Deserialize<ProofResultDto>(responseBody);

                string proofId = proofGeneration.ProofId;
                _logger.LogInformation("Generation successfull");

                if (proofGeneration.Proof?.ProofResult != null)
                {
                    return proofGeneration.Proof?.ProofResult;
                }

                int restart = 0;
                do
                {
                    try
                    {
                        var clientProof = new HttpClient();
                        var requestProof = new HttpRequestMessage(HttpMethod.Get, $"https://sindri.app/api/v1/proof/{proofId}/detail");
                        requestProof.Headers.Add("Accept", "application/json");
                        requestProof.Headers.Add("Authorization", "Bearer sindri-Y1qkOKoN734PWkUCxwrJ2a1WhnZtIwLG-Stsk");
                        var responseProof = await client.SendAsync(requestProof);
                        responseProof.EnsureSuccessStatusCode();
                        string responseBodyProof = await response.Content.ReadAsStringAsync();
                        ProofResultDto proofGenerationProof = JsonSerializer.Deserialize<ProofResultDto>(responseBodyProof);
                        if (proofGenerationProof.Proof?.ProofResult != null)
                        {
                            return proofGenerationProof.Proof?.ProofResult;
                        }
                    }
                    catch (Exception)
                    {

                    }
                    finally { restart++; }
                    await Task.Delay(2000);

                } while (!(restart > 4));

                _logger.LogInformation("Get result successfull");

                return proofGeneration.Proof?.ProofResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }


        }
    }
}
