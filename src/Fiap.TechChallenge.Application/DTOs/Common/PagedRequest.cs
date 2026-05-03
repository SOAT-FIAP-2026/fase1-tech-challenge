namespace Fiap.TechChallenge.Application.DTOs.Common
{
    public class PagedRequest
    {
        private const int MaxPageSize = 100;
        private const int DefaultPageSize = 10;

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
    }
}
