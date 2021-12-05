using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Persistence.Models;

namespace Application.Core
{
    public class BaseProcessor : DbContext
    {
        protected DataContext _Context;
        public BaseProcessor(DataContext Context)
        {
            _Context = Context;
        }
    }
}