using System;
using System.Linq;

namespace DefaultNamespace.Tools
{
	public static class EnumExtensions
	{
		public static T NextValue<T>(this T currentValue) where T : Enum
		{
			var enumValues = Enum.GetValues(typeof(T)).Cast<T>().ToArray();

			var nextTypeIndex = Array.IndexOf(enumValues, currentValue) + 1;

			return nextTypeIndex < enumValues.Length ? enumValues[nextTypeIndex] : enumValues[0];
		}
	}
}