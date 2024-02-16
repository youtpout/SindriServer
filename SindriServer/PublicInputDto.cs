namespace SindriServer
{
    public class PublicInputDto
    {
        public string Secret { get; set; }
        public string OldAmount { get; set; }
        public List<string> Witnesses { get; set; }
        public string LeafIndex { get; set; }
        public string Leaf { get; set; }
        public string MerkleRoot { get; set; }
        public string Nullifier { get; set; }
        public string Amount { get; set; }
        public string Receiver { get; set; }
        public string Relayer { get; set; }
        public string Deposit { get; set; }
    }
}
