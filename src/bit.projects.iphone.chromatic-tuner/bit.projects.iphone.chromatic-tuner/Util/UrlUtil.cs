using System;
using System.Text;
using System.Globalization;

namespace bit.projects.iphone.chromatictuner
{
    public class UrlUtil
    {
        public static string UrlEncode (string text)
        {
            if (text == null) {
                return null;
            }

            StringBuilder sb = new StringBuilder (text.Length);
            int len = text.Length;
            for (int i=0; i<len; ++i) {
                var c = text[i];
                if((c >= 'A' && c <= 'Z') ||
                   (c >= 'a' && c <= 'z') ||
                   c == '-' || c == '_' || c == '.' || c =='~') {
                    sb.Append(c);
                } else {
                    sb.Append(string.Format("%{0:x2}",(int)c));                   
                }
            }
            return sb.ToString();
       
        }
    }
}

