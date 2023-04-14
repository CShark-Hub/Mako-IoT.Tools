namespace Mako.IoT.NFVersionInspector.Extensions
{
    public static class ListExtensions
    {
        public static IList<T> AddIfNotNull<T>(this IList<T> list, T? item)
        {
            if (item != null)
                list.Add(item);
            return list;
        }

        public static IEnumerable<Package> Flatten(this IEnumerable<Package> list)
        {
            var set = new HashSet<Package>();
            foreach (var package in list)
            {
                set.Add(package);
                foreach (var dependency in package.Dependencies)
                {
                    set.Add(dependency);
                }
            }

            return set.OrderBy(i => i.IsNative);
        }
    }
}
