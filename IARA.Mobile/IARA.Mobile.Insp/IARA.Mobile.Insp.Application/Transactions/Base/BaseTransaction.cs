using AutoMapper;
using IARA.Mobile.Application.Interfaces.Repositories;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Insp.Application.Interfaces.Factories;
using IARA.Mobile.Insp.Application.Interfaces.Utilities;

namespace IARA.Mobile.Insp.Application.Transactions.Base
{
    public class BaseTransaction
    {
        public BaseTransaction(BaseTransactionProvider provider)
        {
            ContextBuilder = provider.ContextBuilder;
            ExceptionHandler = provider.ExceptionHandler;
            DateTimeProvider = provider.DateTimeProvider;
            CurrentUser = provider.CurrentUser;
            Repository = provider.Repository;
            RestClient = provider.RestClient;
            Settings = provider.Settings;
            Mapper = provider.Mapper;
        }

        protected IAppDbContextBuilder ContextBuilder { get; }
        protected IExceptionHandler ExceptionHandler { get; }
        protected IDateTime DateTimeProvider { get; }
        protected ITLRepository Repository { get; }
        protected ICurrentUser CurrentUser { get; }
        protected IRestClient RestClient { get; }
        protected ISettings Settings { get; }
        protected IMapper Mapper { get; }
    }
}
