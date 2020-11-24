namespace Stratis.Features.AzureIndexer
{
    using System;
    using System.Collections.Generic;

    class AnonymousEqualityComparer<T, TComparer> : IEqualityComparer<T>
    {
        Func<T, TComparer> comparer;

        public AnonymousEqualityComparer(Func<T, TComparer> comparer)
        {
            this.comparer = comparer;
        }

        public bool Equals(T x, T y)
        {
            return this.comparer(x).Equals(this.comparer(y));
        }

        public int GetHashCode(T obj)
        {
            return this.comparer(obj).GetHashCode();
        }
    }
}
