using System;

namespace ReadingIsGood.API.Models
{
    public class UserModel
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
    }
}