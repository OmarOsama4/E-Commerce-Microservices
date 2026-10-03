namespace BusinessLogicLayer.DTO;
public class OrderItemUpdateRequestDTO
{
    public Guid ProductID { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}
