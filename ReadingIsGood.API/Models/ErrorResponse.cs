using System.Collections.Generic;

namespace ReadingIsGood.API.Models
{
    public class ErrorResponse
    {
        /// <summary>
        ///     Errors
        /// </summary>
        public List<ErrorModel> Errors { get; set; } = new();
    }
}