using System;

namespace bit.shared.numerics
{
    public class Map
    {
       public static T[] Transform<T,S>(S[] source, Func<int,S,T> func)
        {
            var result = new T[source.Length];
            for(int i=0;i<result.Length;++i) {
                result[i] = func(i,source[i]);
            }
            return result;            
        }
    }
}

