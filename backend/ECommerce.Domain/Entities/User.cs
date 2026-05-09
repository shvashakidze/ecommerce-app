using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities;


public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Customer;
    public ICollection<Order> Orders { get; set; } = [];
}
