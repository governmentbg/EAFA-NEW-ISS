using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IARA.Excel.Tools.Models;
using Infragistics.Documents.Excel;

namespace IARA.Excel.Tools
{
    public class ExcelExporter
    {
        private readonly ExcelExporterConfiguration config;
        private readonly Stream outputStream;

        private readonly Dictionary<int, int> maxCellsSize = new Dictionary<int, int>();
        private readonly Workbook workbook = new Workbook(WorkbookFormat.Excel2007);
        private readonly Worksheet worksheet;

        private readonly MethodInfo exportXlsxMethod;

        private bool isHierarchicalData = false;
        private int rowCount = 0;

        public ExcelExporter(Stream outputStream, string filename, ExcelExporterConfiguration config = null)
        {
            this.outputStream = outputStream;
            this.config = config ?? new ExcelExporterConfiguration();

            this.exportXlsxMethod = typeof(ExcelExporter).GetMethod(nameof(ExportXlsxHelper), BindingFlags.Instance | BindingFlags.NonPublic);

            this.worksheet = workbook.Worksheets.Add("Sheet1");
        }

        public Stream ExportXlsx(IEnumerable<IDictionary<string, object>> results)
        {
            WorksheetCell cell;

            using (var enumerator = results.GetEnumerator())
            {
                var row = enumerator.Current;

                if (enumerator.MoveNext())
                {
                    int columnCount = 0;

                    if (config.AddHeaders)
                    {
                        row = enumerator.Current;

                        columnCount = 0;
                        foreach (var cellValue in row)
                        {
                            cell = worksheet.Rows[rowCount].Cells[columnCount++];
                            cell.Value = cellValue.Key.Replace("_", " ");
                            cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                            AddOrUpdateCellSize(cell.ColumnIndex, cellValue.Key.Length + 3);
                        }
                        rowCount++;
                    }

                    do
                    {
                        row = enumerator.Current;

                        columnCount = 0;
                        foreach (var cellValue in row)
                        {
                            cell = worksheet.Rows[rowCount].Cells[columnCount++];
                            UpdateCellValue(cell, cellValue.Value);

                            if (cellValue.Value != null)
                            {
                                ApplyCellFormat(cell, cellValue.Value.GetType());
                            }
                        }

                        rowCount++;
                    } while (enumerator.MoveNext());

                    AddTableStyle(columnCount);
                }
            }

            return Finalize();
        }

        public Stream ParallelExportXlsx(IEnumerable<IDictionary<string, object>> results, int taskParallelCount = 4)
        {
            ColumnModel columns = new ColumnModel();

            using (var enumerator = results.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    if (config.AddHeaders)
                    {
                        WriteHeaderRowToExcel(enumerator.Current, columns);
                        columns.ResetColumns();
                        Interlocked.Increment(ref rowCount);

                        WriteRowToExcel(enumerator.Current, columns);
                    }
                    else
                    {
                        WriteRowToExcel(enumerator.Current, columns);
                        object padlock = new object();

                        var parallelOptions = new ParallelOptions
                        {
                            CancellationToken = CancellationToken.None,
                            MaxDegreeOfParallelism = taskParallelCount,
                            TaskScheduler = TaskScheduler.Current
                        };

                        Parallel.For(0, taskParallelCount, parallelOptions, i =>
                        {
                            var localColumns = columns.Clone() as ColumnModel;
                            bool hasNext = false;
                            IDictionary<string, object> row = null;

                        beginLoop:
                            lock (padlock)
                            {
                                hasNext = enumerator.MoveNext();

                                if (hasNext)
                                {
                                    row = enumerator.Current;
                                }
                            }

                            if (hasNext)
                            {
                                WriteRowToExcel(row, localColumns);
                                goto beginLoop;
                            }
                        });
                    }

                    AddTableStyle(columns);
                }
            }

            return Finalize();
        }

        private void WriteHeaderRowToExcel(IDictionary<string, object> row, ColumnModel columns)
        {
            foreach (var cellValue in row)
            {
                string cellAddress = $"{columns.GetNextColumnName()}{rowCount + 1}";
                WorksheetCell cell = worksheet.GetCell(cellAddress, CellReferenceMode.A1);
                cell.Value = cellValue.Key.Replace("_", " ");
                cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                AddOrUpdateCellSize(cell.ColumnIndex, cellValue.Key.Length + 3);
            }
        }

        private void WriteRowToExcel(IDictionary<string, object> row, ColumnModel columns)
        {
            foreach (var cellValue in row)
            {
                WorksheetCell cell = worksheet.GetCell($"{columns.GetNextColumnName()}{rowCount + 1}", CellReferenceMode.A1);

                UpdateCellValue(cell, cellValue.Value);

                if (cellValue.Value != null)
                {
                    ApplyCellFormat(cell, cellValue.Value.GetType());
                }
            }

            Interlocked.Increment(ref rowCount);
        }

        public static Stream ExportCsv(Stream outputStream, IEnumerable<Dictionary<string, object>> results)
        {
            string separator = Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator;
            separator = ";";
            string replaceString = ",";

            StreamWriter writer = new StreamWriter(outputStream, new UTF8Encoding(true));

            using (var enumerator = results.GetEnumerator())
            {
                var record = enumerator.Current;

                if (enumerator.MoveNext())
                {
                    record = enumerator.Current;
                    writer.WriteLine(string.Join(separator, record.Keys).Replace("_", " "));
                }

                while (enumerator.MoveNext())
                {
                    record = enumerator.Current;
                    var values = record.Values.ToList();

                    foreach (var value in values)
                    {
                        string strValue = "";
                        if (value != null)
                        {
                            strValue = value.ToString();
                        }

                        strValue = strValue.Replace(separator, replaceString);
                        strValue = strValue.Replace("\r\n", "\t");
                        strValue = strValue.Replace("\r", "\t");
                        strValue = strValue.Replace("\n", "\t");
                        writer.Write(strValue);
                        writer.Write(separator);
                    }

                    writer.WriteLine();
                }

                writer.Flush();
            }

            if (outputStream.CanSeek)
            {
                outputStream.Position = 0;
            }

            return outputStream;

        }

        public Stream ExportXlsx<TModel>(ExcelExporterData<TModel> data)
            where TModel : class
        {
            isHierarchicalData = data.ChildData != null && data.ChildData.Any();

            ExportXlsxHelper(data, 0);
            return Finalize();
        }

        private void ExportXlsxHelper<TModel>(ExcelExporterData<TModel> data, int indent, object parentKey = null)
            where TModel : class
        {
            IQueryable<TModel> query = config.PaginateQueries ? data.Query : data.Data.AsQueryable();

            if (parentKey != null)
            {
                query = FilterQueryByKey(query, data.ForeignKey, parentKey);
            }

            int totalRecords = query.Count();

            if (totalRecords != 0)
            {
                List<string> fieldNames = GetFieldNames(data.HeaderNames);

                ColumnModel columns = new ColumnModel();
                columns.IndentColumns(indent);

                AddHeaders(GetHeaderNames(data.HeaderNames), columns, indent);

                int skipEntries = 0;

                while (totalRecords > skipEntries)
                {
                    List<TModel> entries;

                    if (config.PaginateQueries)
                    {
                        entries = query.Skip(skipEntries).Take(config.QueryDataChunk).ToList();
                        skipEntries += config.QueryDataChunk;
                    }
                    else
                    {
                        entries = query.ToList();
                        skipEntries = entries.Count;
                    }

                    if (entries.Count > 0)
                    {
                        PropertyInfo[] properties = entries[0].GetType().GetProperties();

                        foreach (TModel row in entries)
                        {
                            FillRowData(row, fieldNames, properties, columns, indent);

                            if (data.ChildData != null)
                            {
                                PropertyInfo primaryKey = properties.Single(x => x.Name == data.PrimaryKey);
                                object primaryKeyValue = primaryKey.GetValue(row);

                                foreach (ExcelExporterData childData in data.ChildData)
                                {
                                    Type type = childData.GetType();
                                    Type[] genericArguments = type.GetGenericArguments();
                                    object[] parameters = new object[] { childData, indent + 1, primaryKeyValue };

                                    exportXlsxMethod.MakeGenericMethod(genericArguments[0]).Invoke(this, parameters);
                                }
                            }
                        }
                    }
                }

                if (!isHierarchicalData)
                {
                    AddTableStyle(columns);
                }
            }
        }

        private void FillRowData<TModel>(TModel row, List<string> fieldNames, PropertyInfo[] properties, ColumnModel columns, int indent)
        {
            foreach (string field in fieldNames)
            {
                WorksheetCell cell = worksheet.GetCell($"{columns.GetNextColumnName()}{rowCount + 1}", CellReferenceMode.A1);
                PropertyInfo property = properties.Single(x => string.Equals(x.Name, field, StringComparison.CurrentCultureIgnoreCase));

                object value = property.GetValue(row);
                UpdateCellValue(cell, value);
                ApplyCellFormat(cell, property);
            }

            worksheet.Rows[rowCount].OutlineLevel = indent;

            rowCount++;

            columns.IndentColumns(indent);
        }

        private bool AddHeaders(IEnumerable<string> headerNames, ColumnModel columns, int indent)
        {
            if (config.AddHeaders)
            {
                foreach (string header in headerNames)
                {
                    string cellAddress = $"{columns.GetNextColumnName()}{rowCount + 1}";
                    WorksheetCell cell = worksheet.GetCell(cellAddress, CellReferenceMode.A1);
                    cell.Value = header;
                    AddOrUpdateCellSize(cell.ColumnIndex, header.Length + 3);

                    cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

                    if (isHierarchicalData)
                    {
                        cell.CellFormat.Font.ColorInfo = new WorkbookColorInfo(Color.White);
                        cell.CellFormat.Fill = CellFill.CreateSolidFill(Color.Black);
                    }
                }

                columns.ResetColumns();
                columns.IndentColumns(indent);

                worksheet.Rows[rowCount].OutlineLevel = indent;
                rowCount++;
            }

            return config.AddHeaders;
        }

        private void AddOrUpdateCellSize(int cell, int size)
        {
            if (maxCellsSize.ContainsKey(cell))
            {
                maxCellsSize[cell] = Math.Max(maxCellsSize[cell], size);
            }
            else
            {
                maxCellsSize.Add(cell, size);
            }
        }

        private Stream Finalize()
        {
            foreach (KeyValuePair<int, int> cellDefinition in maxCellsSize)
            {
                int cellWidth = cellDefinition.Value + 3;
                if (cellWidth > config.MaxColumnWidth)
                {
                    cellWidth = config.MaxColumnWidth;
                    worksheet.Columns[cellDefinition.Key].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                }

                worksheet.Columns[cellDefinition.Key].SetWidth(cellWidth, WorksheetColumnWidthUnit.Character);
            }

            workbook.Save(outputStream);

            if (outputStream.CanSeek)
            {
                outputStream.Position = 0;
            }

            rowCount = 0;
            return outputStream;
        }

        private void UpdateCellValue(WorksheetCell cell, object value)
        {
            if (value != null)
            {
                if (value is bool boolean)
                {
                    AddOrUpdateCellSize(cell.ColumnIndex, 1);
                    cell.Value = boolean ? "Да" : "Не";
                }
                else
                {
                    cell.Value = value;
                    AddOrUpdateCellSize(cell.ColumnIndex, value.ToString().Length);
                }
            }
        }

        private void AddTableStyle(ColumnModel columns)
        {
            if (workbook.StandardTableStyles.Any())
            {
                WorksheetTableStyle tableStyle = workbook.StandardTableStyles.FirstOrDefault(x => x.Name == "TableStyleMedium2");

                if (tableStyle == null)
                {
                    tableStyle = workbook.StandardTableStyles.First();
                }

                string regionName = $"A1:{columns.GetMaxColumnName()}{rowCount}";
                worksheet.Tables.Add(regionName, config.AddHeaders, tableStyle);
            }
        }

        private void AddTableStyle(int columnCount)
        {
            if (workbook.StandardTableStyles.Any())
            {
                WorksheetTableStyle tableStyle = workbook.StandardTableStyles.FirstOrDefault(x => x.Name == "TableStyleMedium2");

                if (tableStyle == null)
                {
                    tableStyle = workbook.StandardTableStyles.First();
                }


                ColumnModel columns = new ColumnModel();
                columns.IndentColumns(columnCount);

                string regionName = $"A1:{columns.GetMaxColumnName()}{rowCount}";
                worksheet.Tables.Add(regionName, config.AddHeaders, tableStyle);
            }
        }

        private static void ApplyCellFormat(WorksheetCell cell, PropertyInfo property)
        {
            ApplyCellFormat(cell, property.PropertyType);
        }

        private static void ApplyCellFormat(WorksheetCell cell, Type type)
        {
            Type[] numericTypes = new Type[]
            {
                typeof(float),
                typeof(double),
                typeof(decimal),
                typeof(short),
                typeof(ushort),
                typeof(int),
                typeof(uint),
                typeof(long),
                typeof(ulong)
            };

            if (numericTypes.Contains(type))
            {
                cell.CellFormat.FormatOptions = WorksheetCellFormatOptions.ApplyNumberFormatting;
            }
        }

        private static List<string> GetFieldNames(IDictionary<string, string> headerNames)
        {
            List<string> result = headerNames.Keys.Select(x => Capitalize(x)).ToList();
            return result;
        }

        private static List<string> GetHeaderNames(IDictionary<string, string> headerNames)
        {
            List<string> result = headerNames.Values.ToList();
            return result;
        }

        private static string Capitalize(string str)
        {
            if (str == null)
            {
                return null;
            }

            if (str.Length > 1)
            {
                return char.ToUpper(str[0]) + str.Substring(1);
            }

            return str.ToUpper();
        }

        private static IQueryable<TModel> FilterQueryByKey<TModel>(IQueryable<TModel> query, string foreignKey, object foreignKeyValue)
            where TModel : class
        {
            ParameterExpression param = Expression.Parameter(typeof(TModel), "p");
            MemberExpression prop = Expression.Property(param, foreignKey);
            ConstantExpression target = Expression.Constant(foreignKeyValue);
            MethodCallExpression equals = Expression.Call(prop, "Equals", null, target);
            LambdaExpression lambda = Expression.Lambda(equals, param);

            Type[] args = new Type[] { query.ElementType };

            MethodCallExpression expression = Expression.Call(typeof(Queryable), "Where", args, query.Expression, lambda);
            return query.Provider.CreateQuery<TModel>(expression);
        }
    }
}
