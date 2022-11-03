namespace ConsoleApp1_ConnectionString
{
    public class Wallet
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal? Balance { get; set; }

        public override string ToString()
        {
            return $"[{Id}] {Name} ({Balance:N0})";
        }
    }
}
