using Mapster;
using MimaCare.Domain.Common;

namespace MimaCare.Application.Common
{
    public static class PagedResultExtensions
    {
        public static PagedResult<TTarget> AdaptTo<TSource, TTarget>(
            this PagedResult<TSource> source)
            where TSource : class
            where TTarget : class
        {
            return new PagedResult<TTarget>
            {
                Items = source.Items.Adapt<IEnumerable<TTarget>>(),
                Page = source.Page,
                PageSize = source.PageSize,
                TotalCount = source.TotalCount
            };
        }
    }
}
