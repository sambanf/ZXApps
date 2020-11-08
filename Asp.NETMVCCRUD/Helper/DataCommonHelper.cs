using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Web;
using System.ComponentModel;
using Newtonsoft.Json;
using System.Globalization;

namespace Asp.NETMVCCRUD.Class
{
    public class DataCommonHelper
    {
        public static string GetValueFromDataTable(DataTable dt, string columnName, string condition)
        {
            DataRow[] drList = null;
            if (!string.IsNullOrEmpty(condition))
            {
                drList = dt.Select(condition);
            }
            else
            {
                drList = dt.Select();
            }
            try
            {
                return StringHelper.SetEmptyStringIfNull(drList[0][columnName]);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static DataTable ConvertObjectToDataTable(Object item, string ExcludedColumns)
        {
            DataTable dataTable = new DataTable(item.GetType().AssemblyQualifiedName);
            string[] ArrColumnExcluded = ExcludedColumns.Split(';');
            Dictionary<string, string> ColumnsExcludedDict = new Dictionary<string, string>();
            if (ArrColumnExcluded.Length > 0)
                foreach (string column in ArrColumnExcluded)
                    ColumnsExcludedDict.Add(column, column);

            //Get all the properties
            PropertyInfo[] Props = item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                if (!ColumnsExcludedDict.ContainsKey(prop.Name))
                    dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            var values = new object[dataTable.Columns.Count];
            int j = 0;
            for (int i = 0; i < Props.Length; i++)
            {
                //inserting property values to datatable rows
                if (!ColumnsExcludedDict.ContainsKey(Props[i].Name))
                {
                    values[j++] = Props[i].GetValue(item, null);
                }
            }
            dataTable.Rows.Add(values);

            //put a breakpoint here and check datatable
            return dataTable;
        }

        public static List<T> DataTableToList<T>(DataTable table) where T : class, new()
        {
            try
            {
                List<T> list = new List<T>();

                foreach (var row in table.AsEnumerable())
                {
                    T obj = new T();
                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        try
                        {
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                            propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    list.Add(obj);
                }
                return list;
            }
            catch
            {
                return null;
            }
        }

        public static DataTable ConvertListToDataTable<T>(List<T> items)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            foreach (T item in items)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        public static DataTable ConvertListToDataTable<T>(List<T> items, string ExcludedColumns)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            string[] ArrColumnExcluded = ExcludedColumns.Split(';');
            Dictionary<string, string> ColumnsExcludedDict = new Dictionary<string, string>();
            if (ArrColumnExcluded.Length > 0)
                foreach (string column in ArrColumnExcluded)
                    ColumnsExcludedDict.Add(column, column);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                if (!ColumnsExcludedDict.ContainsKey(prop.Name))
                {
                    if (prop.PropertyType == typeof(Byte[]))
                    {
                        dataTable.Columns.Add(prop.Name, typeof(byte[]));
                    }
                    else
                    {
                        dataTable.Columns.Add(prop.Name);
                    }

                }
            }
            foreach (T item in items)
            {
                var values = new object[dataTable.Columns.Count];
                int j = 0;
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    if (!ColumnsExcludedDict.ContainsKey(Props[i].Name))
                    {
                        values[j++] = Props[i].GetValue(item, null);
                    }
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public static DataTable ConvertIEnumberableToDataTable<T>(IEnumerable<T> varlist)
        {
            DataTable dtReturn = new DataTable();

            // column names 
            PropertyInfo[] oProps = null;
            FieldInfo[] oField = null;
            if (varlist == null) return dtReturn;

            foreach (T rec in varlist)
            {
                // Use reflection to get property names, to create table, Only first time, others will follow 
                if (oProps == null)
                {
                    oProps = ((Type)rec.GetType()).GetProperties();
                    foreach (PropertyInfo pi in oProps)
                    {
                        Type colType = pi.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }

                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                    oField = ((Type)rec.GetType()).GetFields();
                    foreach (FieldInfo fieldInfo in oField)
                    {
                        Type colType = fieldInfo.FieldType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }

                        dtReturn.Columns.Add(new DataColumn(fieldInfo.Name, colType));
                    }
                }

                DataRow dr = dtReturn.NewRow();

                if (oProps != null)
                {
                    foreach (PropertyInfo pi in oProps)
                    {
                        dr[pi.Name] = pi.GetValue(rec, null) ?? DBNull.Value;
                    }
                }
                if (oField != null)
                {
                    foreach (FieldInfo fieldInfo in oField)
                    {
                        dr[fieldInfo.Name] = fieldInfo.GetValue(rec) ?? DBNull.Value;
                    }
                }
                dtReturn.Rows.Add(dr);
            }
            return dtReturn;
        }

        public static string DataTableToCSVString(DataTable table, string delimiter, bool includeHeader)
        {
            StringBuilder result = new StringBuilder();

            if (includeHeader)
            {
                foreach (DataColumn column in table.Columns)
                {
                    result.Append(column.ColumnName);
                    result.Append(delimiter);
                }
                result.Remove(--result.Length, 0);
                result.Append(Environment.NewLine);
            }

            foreach (DataRow row in table.Rows)
            {

                foreach (object item in row.ItemArray)
                {

                    if (item is System.DBNull)

                        result.Append(delimiter);

                    else
                    {

                        string itemAsString = item.ToString();

                        // Double up all embedded double quotes
                        itemAsString = itemAsString.Replace("\"", "\"\"");
                        // To keep things simple, always delimit with double-quotes

                        // so we don't have to determine in which cases they're necessary
                        // and which cases they're not.

                        itemAsString = "\"" + itemAsString + "\"";

                        result.Append(itemAsString + delimiter);

                    }

                }
                result.Remove(--result.Length, 0);
                result.Append(Environment.NewLine);
            }

            return result.ToString();

        }

        public static void DataTableToCSV(DataTable table, string delimiter, bool includeHeader, string args_path)
        {
            string csvstring = DataTableToCSVString(table, delimiter, includeHeader);

            using (StreamWriter writer = new StreamWriter(args_path, true))
            {
                writer.Write(csvstring);

            }
        }

        public static void DataTableToCSV_ExportToHTML(DataTable table, string delimiter, bool includeHeader, string filename)
        {

            string csvstring = DataTableToCSVString(table, delimiter, includeHeader);

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ClearHeaders();
            HttpContext.Current.Response.ContentType = "text/csv";
            HttpContext.Current.Response.AddHeader("content-disposition", string.Format("attachment; filename={0}.csv", filename));
            HttpContext.Current.Response.AddHeader("Pragma", "public");
            HttpContext.Current.Response.Write(csvstring);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();

        }
        public static string[][] deserializeObject(string o)
        {
            return JsonConvert.DeserializeObject<string[][]>(o);
        }

        public static void ResetCulture()
        {
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            customCulture.NumberFormat.NumberGroupSeparator = ",";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
        }

        public static DateTime ConvertDatetimeIDNtoEN(string datetime)
        {
            string[] s_DatetimeFormat = { "dd/MM/yyyy" };
            const string c_CultureInfo = "en-US";
            return DateTime.ParseExact(datetime, s_DatetimeFormat, new CultureInfo(c_CultureInfo), DateTimeStyles.None);
        }

        public static DateTime ConvertDatetimeENtoIDN(string datetime)
        {
            string[] s_DatetimeFormat = { "MM/dd/yyyy hh:mm:ss tt" };
            const string c_CultureInfo = "id";
            return DateTime.ParseExact(datetime, s_DatetimeFormat, new CultureInfo(c_CultureInfo), DateTimeStyles.None);
        }
    }
}
