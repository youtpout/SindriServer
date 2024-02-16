using System.Text.Json.Serialization;

namespace SindriServer
{
    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public class ComputeTimes
    {
        [JsonPropertyName("total")]
        public double? Total { get; set; }

        [JsonPropertyName("queued")]
        public double? Queued { get; set; }

        [JsonPropertyName("clean_up")]
        public double? CleanUp { get; set; }

        [JsonPropertyName("file_setup")]
        public double? FileSetup { get; set; }

        [JsonPropertyName("create_proof")]
        public double? CreateProof { get; set; }

        [JsonPropertyName("save_results")]
        public double? SaveResults { get; set; }
    }

    public class Proof
    {
        [JsonPropertyName("proof")]
        public string? ProofResult { get; set; }
    }

    public class ProofInput
    {
        [JsonPropertyName("Prover.toml")]
        public string? ProverToml { get; set; }
    }

    public class Public
    {
        [JsonPropertyName("Verifier.toml")]
        public string? VerifierToml { get; set; }
    }

    public class ProofResultDto
    {
        [JsonPropertyName("proof_id")]
        public string? ProofId { get; set; }

        [JsonPropertyName("circuit_name")]
        public string? CircuitName { get; set; }

        [JsonPropertyName("circuit_id")]
        public string? CircuitId { get; set; }

        [JsonPropertyName("circuit_type")]
        public string? CircuitType { get; set; }

        [JsonPropertyName("date_created")]
        public DateTime? DateCreated { get; set; }

        [JsonPropertyName("perform_verify")]
        public bool? PerformVerify { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("team")]
        public string? Team { get; set; }

        [JsonPropertyName("compute_time")]
        public string? ComputeTime { get; set; }

        [JsonPropertyName("compute_time_sec")]
        public double? ComputeTimeSec { get; set; }

        [JsonPropertyName("compute_times")]
        public ComputeTimes? ComputeTimes { get; set; }

        [JsonPropertyName("file_size")]
        public int? FileSize { get; set; }

        [JsonPropertyName("proof_input")]
        public ProofInput? ProofInput { get; set; }

        [JsonPropertyName("proof")]
        public Proof? Proof { get; set; }

        [JsonPropertyName("public")]
        public Public? Public { get; set; }

        [JsonPropertyName("verification_key")]
        public VerificationKey? VerificationKey { get; set; }

        [JsonPropertyName("error")]
        public object? Error { get; set; }
    }

    public class VerificationKey
    {
    }



}
