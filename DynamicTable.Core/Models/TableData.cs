namespace DynamicTable.Core.Models;

public class TableData
{
    public IEnumerable<string> Columns { get; set; }
    public IEnumerable<dynamic> Rows { get; set; }
    public int TotalRows { get; set; }
}
