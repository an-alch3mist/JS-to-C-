![`C#`](https://github.com/user-attachments/assets/a17172e4-6f6d-401e-9be8-bce3e6324f49)


![txt](https://github.com/user-attachments/assets/192f001c-92d6-4fd3-9541-411ce8ea5edf)


```txt
column width initially => FIELD[i].length
the column width is determined by the element with max toString().length + padding(2)
```

```cs
/// <summary>
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

	// Separator line(dashes + +-separators)
	//before: sb.AppendLine(new string('-', columnWidths.Sum() + (fields.Length - 1) * 3));
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
```
