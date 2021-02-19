using System;
using NServiceBus;
using NServiceBus.Transport;
using ReadingIsGood.Shared;
using ReadingIsGood.Shared.Exceptions;

namespace ReadingIsGood.ProductService.Policies
{
    public sealed class OptimisticRetryPolicy
    {
        public RecoverabilityAction Retry(RecoverabilityConfig recoverabilityConfig, ErrorContext errorContext)
        {
            if (errorContext.Exception is OptimisticConcurrencyException)
            {
                var ttl = errorContext.Message.Headers[
                    ApplicationConfiguration.Instance.GetValue<string>(
                        "ProductService:RetryPolicy:HeaderNames:UpdateStockInProduct")];

                if (Convert.ToInt32(ttl) == 0)
                    return RecoverabilityAction.MoveToError(recoverabilityConfig.Failed.ErrorQueue);

                //TODO:Kudret--> Beklediğim bir senaryo, bu yüzden immediate olarak retry etsin
                return RecoverabilityAction.ImmediateRetry();
            }

            return DefaultRecoverabilityPolicy.Invoke(recoverabilityConfig, errorContext);
        }
    }
}