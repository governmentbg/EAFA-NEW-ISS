using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using IARA.Common;
using IARA.Common.TempFileUtils;
using IARA.DomainModels.RequestModels;
using IARA.Excel.Tools.Interfaces;
using IARA.Excel.Tools.Models;

namespace IARA.Excel.Tools.Services
{
    public class ExcelExporterService : IExcelExporterService
    {
        private readonly IFilesSweeper filesSweeper;

        public ExcelExporterService(IFilesSweeper filesSweeper)
        {
            this.filesSweeper = filesSweeper;
        }

        public Stream BuildExcelFile<TRequestModel, TModel>(ExcelExporterRequestModel<TRequestModel> request,
                                                            ExcelExporterData<TModel> data,
                                                            ExcelExporterConfiguration config = null)
            where TRequestModel : BaseRequestModel
            where TModel : class
        {
            config ??= new ExcelExporterConfiguration();

            TempFileStream fileStream = new TempFileStream(TempFileBuilder.GenerateUniqueFileName());
            filesSweeper.AddFileForRemoval(fileStream);

            if (request.SortColumns != null && request.SortColumns.Any())
            {
                data.Query = OrderByField(data.Query, request.SortColumns[0].PropertyName, "OrderBy", request.SortColumns[0].SortOrder);

                for (int i = 1; i < request.SortColumns.Count; ++i)
                {
                    data.Query = OrderByField(data.Query, request.SortColumns[i].PropertyName, "ThenBy", request.SortColumns[i].SortOrder);
                }
            }

            return new ExcelExporter(fileStream, request.Filename, config).ExportXlsx(data);
        }

        private static IQueryable<T> OrderByField<T>(IQueryable<T> query, string propertyName, string methodName, string sortOrder)
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "p");
            MemberExpression body = Expression.Property(parameterExpression, propertyName);
            LambdaExpression lambdaExpression = Expression.Lambda(body, parameterExpression);

            Type[] typeArguments = new Type[2]
            {
                query.ElementType,
                lambdaExpression.Body.Type
            };

            if (sortOrder == "desc")
            {
                methodName = $"{methodName}Descending";
            }

            MethodCallExpression expression = Expression.Call(typeof(Queryable), methodName, typeArguments, query.Expression, lambdaExpression);
            return query.Provider.CreateQuery<T>(expression);
        }
    }
}
