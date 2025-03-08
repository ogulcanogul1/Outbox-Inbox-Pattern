namespace Stock.API.Models;

public class OrderInbox
{
    public Guid IdempotentToken { get; set; }
    public bool Process { get; set; }
    public string Payload { get; set; }
}
