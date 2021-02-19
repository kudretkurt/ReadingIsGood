using System;
using NServiceBus;

namespace ReadingIsGood.OrderService.Contracts.Messages
{
    [Express]
    public class OrderViewRequest : IMessage
    {
        /// <summary>
        ///     OrderViewRequest
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public OrderViewRequest(Guid userId, int pageSize, int pageNumber)
        {
            if (userId == default) throw new ArgumentNullException(nameof(userId));

            if (pageSize > 10) throw new InvalidOperationException("PageSize should less than 11");

            if (pageSize <= 0) throw new InvalidOperationException("PageSize should greater than 0");

            if (pageNumber <= 0) throw new InvalidOperationException("PageNumber should greater than 0");

            if (pageNumber == int.MaxValue)
                throw new InvalidOperationException($"PageNumber should not be equal {int.MaxValue}");

            UserId = userId;
            PageSize = pageSize;
            PageNumber = pageNumber;
        }

        /// <summary>
        ///     UserId
        /// </summary>
        public Guid UserId { get; protected set; }

        /// <summary>
        ///     PageSize
        /// </summary>
        public int PageSize { get; protected set; }

        /// <summary>
        ///     PageNumber
        /// </summary>
        public int PageNumber { get; protected set; }
    }
}