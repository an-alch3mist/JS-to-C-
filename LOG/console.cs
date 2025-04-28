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


	/// <summary>
	/// make sure element class got ovverriden ToString() method
	/// does to ToString() for each of attribute, with thier name in column
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
			BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic
		);

		// Calculate column widths
		var columnWidths = new int[fields.Length];
		for (int i = 0; i < fields.Length; i++)
		{
			columnWidths[i] = fields[i].Name.Length;
			foreach (var item in items)
			{
				object val = fields[i].GetValue(item);
				columnWidths[i] = Math.Max(columnWidths[i], (val?.ToString() ?? "null").Length);
			}
			columnWidths[i] += 2; // Add a little padding
		}

		// Header
		sb.AppendLine(string.Join(" | ", fields.Select((f, i) =>
		{
			string fieldName = f.Name;
			if (!f.IsPublic)
				fieldName = "-" + fieldName; // prefix - for private field
			return fieldName.PadRight(columnWidths[i]);
		})));

		// Separator line(dashes + +-separators),before: sb.AppendLine(new string('-', columnWidths.Sum() + (fields.Length - 1) * 3));
		for (int i0 = 0; i0 < fields.Length; i0 += 1)
		{
			sb.Append(new string('-', columnWidths[i0]));
			if (i0 < fields.Length - 1)
				sb.Append("-+-"); // seperator
		}
		sb.AppendLine();

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