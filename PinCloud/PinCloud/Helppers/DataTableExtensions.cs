using System.Data;
using System.Reflection;
using APinI.Models;

namespace APinI.Helppers
{
    public static class DataTableExtensions
    {
        /// <summary>
        /// Converts a List of IQOptionCandle to DataTable
        /// </summary>
        /// <param name="list">The list to convert</param>
        /// <returns>DataTable representation of the list</returns>
        public static DataTable ToDataTable(this List<IQOptionCandle> list)
        {
            var dataTable = new DataTable();
            
            if (list == null || !list.Any())
                return dataTable;

            // Get properties from the IQOptionCandle type
            var properties = typeof(IQOptionCandle).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            
            // Create columns based on properties
            foreach (var property in properties)
            {
                var columnType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                dataTable.Columns.Add(property.Name, columnType);
            }
            
            // Add rows
            foreach (var item in list)
            {
                var row = dataTable.NewRow();
                foreach (var property in properties)
                {
                    var value = property.GetValue(item);
                    row[property.Name] = value ?? DBNull.Value;
                }
                dataTable.Rows.Add(row);
            }
            
            return dataTable;
        }
        
        /// <summary>
        /// Generic method to convert any List<T> to DataTable
        /// </summary>
        /// <typeparam name="T">The type of objects in the list</typeparam>
        /// <param name="list">The list to convert</param>
        /// <returns>DataTable representation of the list</returns>
        public static DataTable ToDataTable<T>(this List<T> list)
        {
            var dataTable = new DataTable();
            
            if (list == null || !list.Any())
                return dataTable;

            // Get properties from the generic type T
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            
            // Create columns based on properties
            foreach (var property in properties)
            {
                var columnType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                dataTable.Columns.Add(property.Name, columnType);
            }
            
            // Add rows
            foreach (var item in list)
            {
                var row = dataTable.NewRow();
                foreach (var property in properties)
                {
                    var value = property.GetValue(item);
                    row[property.Name] = value ?? DBNull.Value;
                }
                dataTable.Rows.Add(row);
            }
            
            return dataTable;
        }
    }
}