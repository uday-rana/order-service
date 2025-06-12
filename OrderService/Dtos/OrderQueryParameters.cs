using System.ComponentModel.DataAnnotations;

using OrderService.Models;

namespace OrderService.Dtos;

public record OrderQueryParameters(
    [Range(1, int.MaxValue)]
    int Page = 1,

    [Range(1, 100)]
    int PageSize = 10,

    int? CustomerId = null,

    OrderStatus? Status = null,

    DateTime? CreatedFrom = null,

    DateTime? CreatedTo = null,

    DateTime? FulfilledFrom = null,

    DateTime? FulfilledTo = null,

    int? ProductId = null,

    string? SortBy = null,

    bool SortDescending = false
);
