
using System;
using System.Collections.Generic;
using System.Linq;

namespace OneSDK
{
	/// <summary> Utility class with optimized methods for working with Types </summary>
	public static class TypeUtils
	{
		/// <summary> Cast object to the given type </summary>
		/// <param name="input"> Target object </param>
		/// <typeparam name="T"> Type to cast to </typeparam>
		/// <returns> Object casted to the given type </returns>
		public static T CastObject<T>(object input) =>
			(T) input;

		/// <summary> Convert object to the given type </summary>
		/// <param name="input"> target object </param>
		/// <typeparam name="T"> Type to convert to </typeparam>
		/// <returns> Object converted to the given type </returns>
		public static T ConvertObject<T>(object input) =>
			(T) Convert.ChangeType(input, typeof(T));

	}
}