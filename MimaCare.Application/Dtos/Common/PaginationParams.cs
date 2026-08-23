namespace MimaCare.Application.Dtos.Common
{
    public class PaginationParams
    {
        private const int MaxPageSize = 100;
        private const int DefaultPageSize = 10;

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = DefaultPageSize;

        public int EffectivePage
            => Page < 1 ? 1 : Page;

        public int EffectivePageSize
            => PageSize < 1
                ? DefaultPageSize
                : PageSize > MaxPageSize
                    ? MaxPageSize
                    : PageSize;
    }
}
