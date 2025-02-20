using System;
using System.Collections.Generic;

namespace PizzaShop.Models;

public partial class User
{
    public long Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Username { get; set; } = null!;

    public long RoleId { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string ProfileImg { get; set; } = null!;

    public long Phone { get; set; }

    public bool? IsActive { get; set; }

    public long CountryId { get; set; }

    public long StateId { get; set; }

    public long CityId { get; set; }

    public string Address { get; set; } = null!;

    public int ZipCode { get; set; }

    public DateTime CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public string? Resettoken { get; set; }

    public DateTime? Resettokenexpiry { get; set; }

    public virtual ICollection<Category> CategoryCreatedByNavigations { get; set; } = new List<Category>();

    public virtual ICollection<Category> CategoryUpdatedByNavigations { get; set; } = new List<Category>();

    public virtual City City { get; set; } = null!;

    public virtual Country Country { get; set; } = null!;

    public virtual ICollection<Customer> CustomerCreatedByNavigations { get; set; } = new List<Customer>();

    public virtual ICollection<Customer> CustomerUpdatedByNavigations { get; set; } = new List<Customer>();

    public virtual ICollection<CustomersReview> CustomersReviewCreatedByNavigations { get; set; } = new List<CustomersReview>();

    public virtual ICollection<CustomersReview> CustomersReviewUpdatedByNavigations { get; set; } = new List<CustomersReview>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Item> ItemCreatedByNavigations { get; set; } = new List<Item>();

    public virtual ICollection<Item> ItemUpdatedByNavigations { get; set; } = new List<Item>();

    public virtual ICollection<Modifier> ModifierCreatedByNavigations { get; set; } = new List<Modifier>();

    public virtual ICollection<Modifier> ModifierUpdatedByNavigations { get; set; } = new List<Modifier>();

    public virtual ICollection<Order> OrderCreatedByNavigations { get; set; } = new List<Order>();

    public virtual ICollection<OrderItem> OrderItemCreatedByNavigations { get; set; } = new List<OrderItem>();

    public virtual ICollection<OrderItem> OrderItemUpdatedByNavigations { get; set; } = new List<OrderItem>();

    public virtual ICollection<OrderItemsModifier> OrderItemsModifierCreatedByNavigations { get; set; } = new List<OrderItemsModifier>();

    public virtual ICollection<OrderItemsModifier> OrderItemsModifierUpdatedByNavigations { get; set; } = new List<OrderItemsModifier>();

    public virtual ICollection<Order> OrderUpdatedByNavigations { get; set; } = new List<Order>();

    public virtual ICollection<PaymentMethod> PaymentMethodCreatedByNavigations { get; set; } = new List<PaymentMethod>();

    public virtual ICollection<PaymentMethod> PaymentMethodUpdatedByNavigations { get; set; } = new List<PaymentMethod>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<RolePermission> RolePermissionCreatedByNavigations { get; set; } = new List<RolePermission>();

    public virtual ICollection<RolePermission> RolePermissionUpdatedByNavigations { get; set; } = new List<RolePermission>();

    public virtual ICollection<Section> SectionCreatedByNavigations { get; set; } = new List<Section>();

    public virtual ICollection<Section> SectionUpdatedByNavigations { get; set; } = new List<Section>();

    public virtual State State { get; set; } = null!;

    public virtual ICollection<Table> TableCreatedByNavigations { get; set; } = new List<Table>();

    public virtual ICollection<Table> TableUpdatedByNavigations { get; set; } = new List<Table>();

    public virtual ICollection<Taxis> TaxisCreatedByNavigations { get; set; } = new List<Taxis>();

    public virtual ICollection<Taxis> TaxisUpdatedByNavigations { get; set; } = new List<Taxis>();

    public virtual ICollection<WaitingToken> WaitingTokenCreatedByNavigations { get; set; } = new List<WaitingToken>();

    public virtual ICollection<WaitingToken> WaitingTokenUpdatedByNavigations { get; set; } = new List<WaitingToken>();
}
