using System;
using System.Reflection;
using System.Text;

namespace PicoContainer.Utils
{
	/// <summary>
	/// Summary description for TypeUtls.
	/// </summary>
	public class TypeUtils
	{
		private TypeUtils()
		{
		}

		public static Type[] GetParameterTypes(ParameterInfo[] pis)
		{
			Type[] types = new Type[pis.Length];
			int x = 0;
			foreach (System.Reflection.ParameterInfo pi in pis)
			{
				types[x++] = pi.ParameterType;
			}

			return types;
		}

		public static Type[] GetParameterTypes(ConstructorInfo ci)
		{
			return GetParameterTypes(ci.GetParameters());
		}

		public static Type[] GetParameterTypes(MethodInfo ci)
		{
			return GetParameterTypes(ci.GetParameters());
		}

		public static string ConstructorAsString(ConstructorInfo ci)
		{
			StringBuilder sb = new StringBuilder(ci.DeclaringType.FullName).Append("(");
			ParameterInfo[] pi = ci.GetParameters();
			for (int x = 0; x < pi.Length; x++)
			{
				sb.Append(pi[x].ParameterType);
				if (x + 1 < pi.Length)
				{
					sb.Append(" ,");
				}
			}

			sb.Append(")");
			return sb.ToString();
		}
	}
}