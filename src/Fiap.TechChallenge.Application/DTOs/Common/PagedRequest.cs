using Fiap.TechChallenge.Contracts.Common;

namespace Fiap.TechChallenge.Application.DTOs.Common
{
    public class PagedRequest
    {
        public const int MaxPageSize = PagedRequestContrato.MaxPageSize;
        public const int DefaultPageSize = PagedRequestContrato.DefaultPageSize;

        private int _page = 1;
        private int _pageSize = DefaultPageSize;

        public int Page
        {
            get => _page;
            set => _page = value < 1 ? 1 : value;
        }

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value < 1 ? DefaultPageSize : (value > MaxPageSize ? MaxPageSize : value);
        }

        public int Skip => (Page - 1) * PageSize;

        public PagedRequestContrato ToContract() => new(Page, PageSize);

        public static implicit operator PagedRequestContrato(PagedRequest request) =>
            new(request.Page, request.PageSize);

        public static implicit operator PagedRequest(PagedRequestContrato contract) =>
            new() { Page = contract.Page, PageSize = contract.PageSize };
    }
}
