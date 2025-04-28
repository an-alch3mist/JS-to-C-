using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

public static class console
{
	public static string loc_file = Application.dataPath + "/LOG/LOG.txt";
	public static void log(params object[] args)
	{
		string str = string.Join("\n\n", args);

		// Unity console
		Debug.Log(str);
	}
	public static void log_txt(params object[] args)
	{
		string str = string.Join("\n\n", args);
		//string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
		//string logEntry = $"[{timestamp}] {str}";
		Debug.Log($"logged into file: {str}");
		// File logging

		try { File.AppendAllText(loc_file, str + Environment.NewLine); }
		catch (Exception e) { Debug.LogError($"Failed to write to log file: {e.Message}"); }
	}

	// does to ToString() for each of attribute, with thier name in column
	/// <summary>
	/// Renders a list of T into a plain-text table. 
	/// Columns are sized to fit the widest cell in each column.
	/// </summary>
	public static string toTable<T>(this IEnumerable<T> list, string name = "LIST<>")
	{
		if (list == null)
			return "list is null";
		var items = list.ToList();
		if (items.Count == 0)
			return "list got no elem";

		var sb = new StringBuilder();
		var type = typeof(T);
		var fields = type.GetFields(
			BindingFlags.Public | BindingFlags.Instance
		);

		// Calculate column widths
		var columnWidths = new int[fields.Length];
		for (int i = 0; i < fields.Length; i++)
		{
			columnWidths[i] = fields[i].Name.Length;
			foreach (var item in items)
			{
				var val = fields[i].GetValue(item);
				columnWidths[i] = Math.Max(columnWidths[i], (val?.ToString() ?? "null").Length);
			}
			columnWidths[i] += 2; // Add a little padding
		}

		// Header
		sb.AppendLine(string.Join(" | ", fields.Select((f, i) => f.Name.PadRight(columnWidths[i]))));
		sb.AppendLine(new string('-', columnWidths.Sum() + (fields.Length - 1) * 3));

		// Rows
		foreach (var item in items)
		{
			var values = fields.Select((f, i) =>
			{
				var val = f.GetValue(item);
				return (val?.ToString() ?? "null").PadRight(columnWidths[i]);
			});
			sb.AppendLine(string.Join(" | ", values));
		}

		return $"{name}:\n" + sb.ToString();
	}

}

public static class list__to__table
{
	public static string incorrect_spacing_tab_toTable<T>(this IEnumerable<T> collection, string name = "LIST<>")
	{
		if (collection == null) return $"{name}: null";
		if (!collection.Any()) return $"{name}: empty";

		Type type = typeof(T);
		bool isSimpleType = IsSimpleType(type);

		var sb = new StringBuilder();
		sb.AppendLine($"{name}:");

		if (isSimpleType)	BuildSimpleTable(sb, collection);
		else				BuildComplexTable(sb, collection, type);

		return sb.ToString();
	}

	static bool IsSimpleType(Type type)
	{
		return type.IsPrimitive
			|| type.IsEnum
			|| type == typeof(string)
			|| type == typeof(decimal)
			|| type == typeof(DateTime);
	}
	static void BuildSimpleTable<T>(StringBuilder sb, IEnumerable<T> collection)
	{
		sb.AppendLine("Index | Value");
		sb.AppendLine("------|------");

		int index = 0;
		foreach (var item in collection)
		{
			sb.AppendLine($"{index++,5} | {item?.ToString() ?? "null"}");
		}
	}
	static void BuildComplexTable<T>(StringBuilder sb, IEnumerable<T> collection, Type type)
	{
		var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
		var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
		var columns = new List<MemberInfo>();
		columns.AddRange(properties);
		columns.AddRange(fields);

		if (columns.Count == 0)
		{
			sb.AppendLine("No public properties/fields");
			return;
		}

		// Build header
		sb.AppendLine(string.Join(" | ", columns.Select(c => c.Name.PadRight(15))));
		sb.AppendLine(new string('-', columns.Count * 18));

		// Build rows
		int rowIndex = 0;
		foreach (var item in collection)
		{
			if (item == null)
			{
				sb.AppendLine($"{rowIndex, 5} | {"null".PadRight(15)}");
				rowIndex += 1;
				continue;
			}

			var values = columns.Select(c =>
			{
				object value = c is PropertyInfo pi
					? pi.GetValue(item)
					: ((FieldInfo)c).GetValue(item);

				// value.ToString()
				return (value?.ToString() ?? "null").PadRight(15);
			});

			sb.AppendLine($"{rowIndex,5} | {string.Join(" | ", values)}");
			rowIndex += 1;
		}
	}
}

#region prev_extension__to_table
public static class prev_extension
{
	/// <summary>
	/// get elements inside a LIST to log 
	/// </summary>
	public static string toPrimitiveTable<T>(this List<T> LIST, string name = "LIST<>")
	{
		if (LIST == null || LIST.Count == 0) return "LIST.count = 0";
		var sb = new StringBuilder();
		sb.Append($"{name}:\n");
		for (int i = 0; i < LIST.Count; i += 1)
		{
			T element = LIST[i];
			sb.Append($"\t{i}__");

			if (element == null) { sb.AppendLine("~ "); continue; }

			// Handle primitives(bool, int, long, string, float), enums types
			sb.AppendLine(element.ToString());
		}

		return sb.ToString();
	}
	public static string toPrimitiveTable<T>(this T[] LIST, string name = "LIST[]")
	{
		if (LIST == null || LIST.Length == 0) return "LIST.count = 0";
		var sb = new StringBuilder();
		sb.Append($"{name}:\n");
		for (int i = 0; i < LIST.Length; i += 1)
		{
			T element = LIST[i];
			sb.Append($"\t{i}__");

			if (element == null) { sb.AppendLine("~ "); continue; }
		
			// Handle primitives(bool, int, long, string, float), enums types
			sb.AppendLine(element.ToString());
		}

		return sb.ToString();
	}
}

/*
str:
	LIST<>:
		0__ id: [0, 1], dist: 2, ansc: ~ 
		1__ id: c, dist: 3, ansc: dist: 0, ansc_ref: null 
		2__ id: d, dist: 4, ansc: dist: 0, ansc_ref: null 
		3__ id: e, dist: 4, ansc: dist: 3, ansc_ref: c 
		4__ id: [1, 2], dist: 6, ansc: dist: 3, ansc_ref: c 
*/
#endregion