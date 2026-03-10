namespace OrderManagementSystem.Lab.Lab3.Builder;

public interface ICustomOrderBuilder
{
    ICustomOrderBuilder ForUser(int userId);
    ICustomOrderBuilder AddItem(int productId, int quantity);
    ICustomOrderBuilder ShipTo(string address);
    ICustomOrderBuilder WithDiscountCode(string code);
    ICustomOrderBuilder AsPriority();
    ICustomOrderBuilder WithNote(string note);
    ICustomOrderBuilder WithDeliveryOption(string option);
    CustomOrderDto Build();
}
