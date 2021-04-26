using System;

namespace Domain
{
    public class User
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }

        public DateTime JoiningDate { get; set; }
    }
}