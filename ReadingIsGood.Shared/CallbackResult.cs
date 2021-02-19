using NServiceBus;

namespace ReadingIsGood.Shared
{
    public class CallbackResult<T> : IMessage
    {
        public CallbackResult(bool success, string response, T payload)
        {
            Success = success;
            Response = response;
            Payload = payload;
        }

        /// <summary>
        ///     Success
        /// </summary>
        public bool Success { get; protected set; }

        /// <summary>
        ///     Response
        /// </summary>
        public string Response { get; protected set; }

        /// <summary>
        ///     Payload
        /// </summary>
        public T Payload { get; protected set; }

        /// <summary>
        ///     SuccessResult
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public static CallbackResult<T> SuccessResult(T payload)
        {
            return new(true, string.Empty, payload);
        }

        /// <summary>
        ///     ErrorResult
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static CallbackResult<T> ErrorResult(T payload, string errorMessage)
        {
            return new(false, errorMessage, payload);
        }
    }
}