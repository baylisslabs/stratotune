using System;

namespace bit.shared.numerics
{
    public class Reduce
    {
        public static int ArgMax(int begin,int end,Func<int,double> func) 
        {
            var max_i = begin;
            var max_f_i = func(begin);          
            for(int i=begin+1;i<end;++i)
            {
                var f_i = func(i);
                if(f_i>max_f_i) {
                    max_f_i = f_i;
                    max_i = i;
                }
            }
            return max_i;
        }
    }
}

