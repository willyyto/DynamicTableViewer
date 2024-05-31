using DynamicTable.Core.Models;
using DynamicTable.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DynamicTable.Core.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DatabaseViewerController : ControllerBase
    {
        private readonly DatabaseService _databaseService;

        public DatabaseViewerController()
        {
            _databaseService = new DatabaseService();
        }

        [HttpGet("tables")]
        public async Task<ActionResult<IEnumerable<string>>> GetTables([FromQuery] string connectionString)
        {
            try
            {
                var tables = await _databaseService.GetTableNamesAsync(connectionString);
                return Ok(tables);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving tables: {ex.Message}");
            }
        }

        [HttpGet("{tableName}/columns")]
        public async Task<ActionResult<IEnumerable<string>>> GetColumns([FromQuery] string connectionString, string tableName)
        {
            // Security: Validate and sanitize tableName and connectionString
            try
            {
                var columns = await _databaseService.GetColumnNamesAsync(connectionString, tableName);
                return Ok(columns);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving columns: {ex.Message}");
            }
        }

        [HttpGet("{tableName}/data")]
        public async Task<ActionResult<TableData>> GetTableData(
            [FromQuery] string connectionString,
            string tableName,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            // Security: Validate and sanitize tableName and connectionString
            try
            {
                var data = await _databaseService.GetTableDataAsync(connectionString, tableName, page, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving table data: {ex.Message}");
            }
        }

        [HttpGet("{tableName}/full")]
        public async Task<ActionResult<TableData>> GetFullTableData(
            [FromQuery] string connectionString,
            string tableName,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            // Implement security checks here: validate and sanitize inputs
            try
            {
                var tableData = await _databaseService.GetTableDataWithColumnsAsync(connectionString, tableName, page, pageSize);
                return Ok(tableData);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving full table data: {ex.Message}");
            }
        }
        
        [HttpGet("{tableName}/export")]
        public async Task<IActionResult> ExportTableData([FromQuery] string connectionString, string tableName)
        {
            var databaseService = new DatabaseService();
            try
            {
                var data = await databaseService.GetTableDataWithColumnsAsync(connectionString, tableName, 1, int.MaxValue);
        
                var csv = new StringBuilder();
        
                // Add headers
                var headers = data.Columns;
                if (headers != null)
                {
                    csv.AppendLine(string.Join(",", headers));
                }

                // Add rows
                foreach (var row in data.Rows)
                {
                    var values = headers.Select(header => (row as IDictionary<string, object>)[header]?.ToString());
                    csv.AppendLine(string.Join(",", values));
                }

                return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", $"{tableName}.csv");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error exporting table data: {ex.Message}");
            }
        }
        
         [HttpPost("execute-query")]
        public async Task<IActionResult> ExecuteQuery([FromQuery] string connectionString, [FromBody] string query)
        {
            try
            {
                var result = await _databaseService.ExecuteQueryAsync(connectionString, query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error executing query: {ex.Message}");
            }
        }
    }
}
