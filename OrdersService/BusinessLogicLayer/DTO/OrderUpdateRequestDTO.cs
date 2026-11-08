namespace BusinessLogicLayer.DTO;
public class OrderUpdateRequestDTO
{
    public Guid OrderID { get; set; }
    public Guid UserID { get; set; }
    public DateTime OrderDate { get; set; }
    public List<OrderItemUpdateRequestDTO> OrderItems { get; set; } = new List<OrderItemUpdateRequestDTO>();
}
