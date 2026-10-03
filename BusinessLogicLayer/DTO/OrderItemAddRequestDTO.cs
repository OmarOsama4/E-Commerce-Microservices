namespace BusinessLogicLayer.DTO;
public class OrderItemAddRequestDTO
{
    public Guid ProductID { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}
