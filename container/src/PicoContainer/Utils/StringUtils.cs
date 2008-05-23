using System;

namespace PicoContainer.Utils
{
    /// <summary>
    /// Summary description for StringUtils.
    /// </summary>
    public class StringUtils
    {
        private StringUtils()
        {
        }

        public static string ArrayToString(object[] array)
        {
            String retval = "";

            for (int i = 0; i < array.Length; i++)
            {
                retval = retval + array[i];
                if (i + 1 < array.Length)
                {
                    retval += ",";
                }
            }
            return retval;
        }
    }
}