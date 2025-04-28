using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SPACE_UTIL
{
	public static class U
	{
		public static T minMax<T>(T[] T_1D, Func<T, T, float> cmp_func)
		{
			T min = T_1D[0];
			for (int i0 = 1; i0 < T_1D.Length; i0 += 1)
				if (cmp_func(T_1D[i0], min) < 0f) // if ( b - a ) < 0f, than a < b, so swap
					min = T_1D[i0];
			return min;
		}
		public static T minMax<T>(List<T> T_1D, Func<T, T, float> cmp_func)
		{
			return minMax(T_1D.ToArray(), cmp_func);
		}


		// Equivalent to Array.prototype.find()
		public static T find<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
		{
			if (collection == null) throw new ArgumentNullException(nameof(collection));
			if (predicate == null) throw new ArgumentNullException(nameof(predicate));

			// collection.MoveNext(), or a foreach loop
			foreach (var item in collection)
				if (predicate(item))
					return item;
			Debug.Log("found none with collection name provided");
			return default(T); // Returns null for reference types, default value for value types
		}

		// Equivalent to Array.prototype.findIndex()
		public static int findIndex<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
		{
			if (collection == null) throw new ArgumentNullException(nameof(collection));
			if (predicate == null) throw new ArgumentNullException(nameof(predicate));

			int index = 0;
			// collection.MoveNext(), or a foreach loop
			foreach (var item in collection)
			{
				if (predicate(item))
					return index;
				index += 1;
			}
			return -1; // Returns -1 if found none
		}
	}
}
