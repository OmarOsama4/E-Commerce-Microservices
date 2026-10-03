namespace BusinessLogicLayer.DTO;
public class OrderAddRequestDTO
{
    public Guid UserID { get; set; }
    public DateTime OrderDate { get; set; }
    public List<OrderItemAddRequestDTO> OrderItems { get; set; } = new List<OrderItemAddRequestDTO>();
}
