using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Exceptions;
using IARA.Mobile.Application.Interfaces.Database;
using IARA.Mobile.Application.Interfaces.Factories;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Entities.Exceptions;
using IARA.Mobile.Domain.Enums;
using Xamarin.Forms;

namespace IARA.Mobile.Shared.Utilities
{
    public class ExceptionHandlerUtility : IExceptionHandler
    {
        private readonly IMobileInfo _mobileInfo;
        private readonly IAnalytics _analytics;

        public ExceptionHandlerUtility(IMobileInfo mobileInfo, IAnalytics analytics)
        {
            _mobileInfo = mobileInfo;
            _analytics = analytics;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }

        private async void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            await TryLog(new ExceptionApiDto
            {
                Level = LogLevel.Error,
                Message = FormatException(e.Exception),
                StackTrace = e.Exception.StackTrace,
                ExceptionSource = e.Exception.Source,
                LogDate = DateTime.Now,
            }, forceLogIntoDB: true);
        }

        private async void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = e.ExceptionObject as Exception;

            if (exception != null)
            {
                return;
            }

            await TryLog(new ExceptionApiDto
            {
                Level = LogLevel.Error,
                Message = FormatException(exception),
                StackTrace = exception.StackTrace,
                ExceptionSource = exception.Source,
                LogDate = DateTime.Now,
            }, forceLogIntoDB: true);
        }

        public async Task HandleException(Exception exception)
        {
            if (exception == null)
            {
                return;
            }

            await TryLog(new ExceptionApiDto
            {
                Level = LogLevel.Error,
                Message = FormatException(exception),
                StackTrace = exception.StackTrace,
                ExceptionSource = exception.Source,
                LogDate = DateTime.Now,
            });
        }

        public Task DebugLog(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return Task.CompletedTask;
            }

            return TryLog(new ExceptionApiDto
            {
                Level = LogLevel.Debug,
                Message = message,
                LogDate = DateTime.Now,
            });
        }

        private async Task TryLog(ExceptionApiDto dto, bool forceLogIntoDB = false)
        {
            try
            {
                if (dto == null)
                {
                    return;
                }
                else if (CommonGlobalVariables.InternetStatus == InternetStatus.Disconnected || forceLogIntoDB)
                {
                    IDbContextBuilder builder = DependencyService.Resolve<IDbContextBuilder>();

                    if (builder.DatabaseExists)
                    {
                        using IDbContext context = builder.CreateContext();
                        context.ErrorLogs.Add(new ErrorLog
                        {
                            ExceptionSource = dto.ExceptionSource,
                            Level = dto.Level,
                            LogDate = dto.LogDate.Value,
                            Message = dto.Message,
                            StackTrace = dto.StackTrace
                        });
                    }
                }
                else
                {
                    dto.Client = _mobileInfo.Info;
                    IRestClient restClient = DependencyService.Resolve<IRestClient>();
                    await restClient.PostAsync("Logger/Log", dto, alertOnException: false);
                }
            }
            catch (Exception ex)
            {
                _analytics.TrackError(ex, new Dictionary<string, string>() {
                        { nameof(dto.ExceptionSource), dto.ExceptionSource },
                        { nameof(dto.Message), dto.Message },
                        { nameof(dto.StackTrace), dto.StackTrace }
                    });
            }
        }

        private string FormatException(Exception exception)
        {
            if (exception == null)
            {
                return null;
            }

            StringBuilder builder = new StringBuilder()
                .Append(exception.Message);

            while (exception.InnerException != null)
            {
                builder.Append("---END MESSAGE---");
                builder.AppendLine(exception.InnerException.Message);
                exception = exception.InnerException;
            }

            return builder.ToString();
        }
    }
}
