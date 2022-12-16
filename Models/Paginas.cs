using Microsoft.EntityFrameworkCore;

namespace GroupSpace2022.Models
{
    public class Paginas<T> : List<T>
    {
        public int PaginaIndex { get; private set; }
        public int TotalPages { get; private set; }

        public Paginas(List<T> items, int count, int pageIndex, int pageSize)
        {
            PaginaIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PaginaIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PaginaIndex < TotalPages);
            }
        }

        public static async Task<Paginas<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new Paginas<T>(items, count, pageIndex, pageSize);
        }

        public static Paginas<T> Create(List<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count;
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new Paginas<T>(items, count, pageIndex, pageSize);
        }
    }
}
