using System;
using AutoMapper;
using IARA.Mobile.Application.Interfaces.Repositories;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Insp.Application.Interfaces.Factories;
using IARA.Mobile.Insp.Application.Interfaces.Utilities;

namespace IARA.Mobile.Insp.Application.Transactions.Base
{
    public class BaseTransactionProvider
    {
        public BaseTransactionProvider(IAppDbContextBuilder contextBuilder, IExceptionHandler exceptionHandler, IDateTime dateTimeProvider, ITLRepository repository, ICurrentUser currentUser, IRestClient restClient, ISettings settings, IMapper mapper)
        {
            ContextBuilder = contextBuilder ?? throw new ArgumentNullException(nameof(contextBuilder));
            ExceptionHandler = exceptionHandler ?? throw new ArgumentNullException(nameof(exceptionHandler));
            DateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            CurrentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            RestClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public IAppDbContextBuilder ContextBuilder { get; }
        public IExceptionHandler ExceptionHandler { get; }
        public IDateTime DateTimeProvider { get; }
        public ITLRepository Repository { get; }
        public ICurrentUser CurrentUser { get; }
        public IRestClient RestClient { get; }
        public ISettings Settings { get; }
        public IMapper Mapper { get; }
    }
}
