using Fiap.TechChallenge.Contracts.Common;

namespace Fiap.TechChallenge.Application.DTOs.Common
{
    public class PagedResult<T>
    {
        public IReadOnlyCollection<T> Items { get; }
        public int Page { get; }
        public int PageSize { get; }
        public int TotalItems { get; }
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalItems / PageSize) : 0;
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;

        public PagedResult(IReadOnlyCollection<T> items, int page, int pageSize, int totalItems)
        {
            Items = items;
            Page = page;
            PageSize = pageSize;
            TotalItems = totalItems;
        }

        public PagedResult(PagedResultContrato<T> contract)
            : this(contract.Items, contract.Page, contract.PageSize, contract.TotalItems) { }

        public PagedResultContrato<T> ToContract() => new(Items, Page, PageSize, TotalItems);

        public static implicit operator PagedResultContrato<T>(PagedResult<T> result) => result.ToContract();
        public static implicit operator PagedResult<T>(PagedResultContrato<T> contract) => new(contract);
    }
}
