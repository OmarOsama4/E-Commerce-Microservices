namespace BusinessLogicLayer.RabbitMQ;
public class ProductNameUpdateMessage
{
    public Guid ProductID { get; set; }
    public string NewName { get; set; } = string.Empty;
}
