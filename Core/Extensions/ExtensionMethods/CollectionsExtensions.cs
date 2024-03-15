
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ExtensionMethods
{
	public static class Collections
	{
		public static List<T> Zipper<T>(List<T> a, List<T> b)
		{
			int maxCount = Mathf.Min(a.Count, b.Count);
			List<T> returnVal = new List<T>();
			for (int i = 0; i < maxCount; i++)
			{
				returnVal.Add(a[i]);
				returnVal.Add(b[i]);
			}
			return returnVal;
		}
		public static bool TryGet<T>(this List<T> list, Predicate<T> match, out T result)
		{
			result = list.Find(match);
			return !EqualityComparer<T>.Default.Equals(result, default(T));
		}

		public static bool TryGet<T>(this List<T> list, int index, out T result)
		{
			result = default;

			if (index >= 0 && index < list.Count)
			{
				result = list[index];
				return true;
			}

			return false;
		}

		public static T First<T>(this List<T> collection)
		{
			return collection[0];
		}

		public static T Last<T>(this List<T> collection)
		{
			return collection[collection.Count - 1];
		}

		public static T First<T>(this T[] collection)
		{
			return collection[0];
		}

		public static T Last<T>(this T[] collection)
		{
			return collection[collection.Length - 1];
		}
		public static void RemoveLast<T>(ref List<T> list)
		{
			list.RemoveAt(list.Count - 1);
		}

		public static T Random<T>(this T[] collection)
		{
			if (collection.Length == 1)
				return collection[0];

			return collection[UnityEngine.Random.Range(0, collection.Length)];
		}

		public static T Random<T>(this List<T> collection)
		{
			if (collection.Count == 1)
				return collection[0];

			return collection[UnityEngine.Random.Range(0, collection.Count)];
		}

		public static Vector3 GetAverageValue(this List<Vector3> list)
		{
			if (list == null)
				return new Vector3();
			Vector3 avg = new Vector3();
			foreach (var item in list)
			{
				avg += item;
			}
			if (list.Count > 0)
				avg /= list.Count;
			return avg;
		}

		public static float GetAverageValue(this List<float> list)
		{
			if (list == null)
				return 0;
			float avg = new float();
			foreach (var item in list)
			{
				avg += item;
			}
			if (list.Count > 0)
				avg /= list.Count;
			return avg;
		}
		public static List<List<T>> CondenseLists<T>(List<List<T>> input, int iterations = 1)
		{
			if (iterations <= 0)
			{
				throw new ArgumentException("Iterations must be a positive integer.");
			}

			var condensedLists = new List<List<T>>();

			foreach (var sublist in input)
			{
				List<T> foundList = null;

				// Check if any element in sublist matches any element in the existing lists
				foreach (var condensedList in condensedLists)
				{
					if (sublist.Any(item => condensedList.Any(existingItem => existingItem.Equals(item))))
					{
						foundList = condensedList;
						break;
					}
				}

				if (foundList != null)
				{
					// Add all unique items from sublist to the found list
					foreach (var item in sublist)
					{
						if (!foundList.Any(existingItem => existingItem.Equals(item)))
						{
							foundList.Add(item);
						}
					}
				}
				else
				{
					// Add the sublist as a new list in condensedLists
					condensedLists.Add(sublist.Distinct().ToList());
				}
			}

			// Recurse if more iterations are needed
			if (iterations > 1)
			{
				return CondenseLists(condensedLists, iterations - 1);
			}

			return condensedLists;
		}
		public static T[] MoveToArray<T>(this T _in)
		{
			return new T[] { _in };
		}

		public static List<T> MoveToList<T>(this T _in)
		{
			return new List<T> { _in };
		}

		public static void SetActiveGO(this List<GameObject> gos, bool active)
		{
			foreach (var item in gos)
			{
				item.gameObject.SetActive(active);
			}
		}

		public static void SetActiveGO(this List<Transform> gos, bool active)
		{
			foreach (var item in gos)
			{
				item.gameObject.SetActive(active);
			}
		}
	}
}