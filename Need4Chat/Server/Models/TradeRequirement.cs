namespace Need4Chat.Server.Models
{
    public partial class TradeRequirement
    {
        public int Id { get; set; }
        public int TradeId { get; set; }
        public int ItemId { get; set; }
        public int? Offset { get; set; }
    }
}
