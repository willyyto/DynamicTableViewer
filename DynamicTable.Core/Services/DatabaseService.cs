using DynamicTable.Core.Models;
using Dapper;
using System.Data;
using System.Data.SqlClient;


namespace DynamicTable.Core.Services
{
    public class DatabaseService
    {
        public async Task<IEnumerable<string>> GetTableNamesAsync(string connectionString)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                const string query = @"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
                var tableNames = await db.QueryAsync<string>(query);
                return tableNames;
            }
        }

        public async Task<IEnumerable<string>> GetColumnNamesAsync(string connectionString, string tableName)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var query = $@"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'{tableName}'";
                var columnNames = await db.QueryAsync<string>(query);
                return columnNames;
            }
        }

        public async Task<TableData> GetTableDataAsync(string connectionString, string tableName, int page, int pageSize)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                // Calculate the starting row number
                int offset = (page - 1) * pageSize;

                // Get the total number of rows
                var countQuery = $@"SELECT COUNT(*) FROM {tableName}";
                var totalRows = await db.ExecuteScalarAsync<int>(countQuery);

                // Get the paginated data
                var dataQuery = $@"SELECT * FROM {tableName} ORDER BY (SELECT NULL) OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
                var data = await db.QueryAsync<dynamic>(dataQuery, new { Offset = offset, PageSize = pageSize });

                // Get the column names
                var columnQuery = $@"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'{tableName}'";
                var columns = await db.QueryAsync<string>(columnQuery);

                return new TableData
                {
                    Columns = columns,
                    Rows = data,
                    TotalRows = totalRows
                };
            }
        }

        public async Task<TableData> GetTableDataWithColumnsAsync(string connectionString, string tableName, int page, int pageSize)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                // Calculate the starting row number
                int offset = (page - 1) * pageSize;

                // Get the total number of rows
                var countQuery = $@"SELECT COUNT(*) FROM {tableName}";
                var totalRows = await db.ExecuteScalarAsync<int>(countQuery);

                // Get the paginated data
                var dataQuery = $@"SELECT * FROM {tableName} ORDER BY (SELECT NULL) OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
                var data = await db.QueryAsync<dynamic>(dataQuery, new { Offset = offset, PageSize = pageSize });

                // Get the column names
                var columnQuery = $@"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'{tableName}'";
                var columns = await db.QueryAsync<string>(columnQuery);

                return new TableData
                {
                    Columns = columns,
                    Rows = data,
                    TotalRows = totalRows
                };
            }
        }
    }
}
