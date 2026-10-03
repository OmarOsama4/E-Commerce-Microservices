namespace BusinessLogicLayer.DTO;
public class OrderResponseDTO
{
    public Guid OrderID { get; set; }
    public Guid UserID { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalBill { get; set; }
    public List<OrderItemResponseDTO> OrderItems { get; set; } = new List<OrderItemResponseDTO>();
}
