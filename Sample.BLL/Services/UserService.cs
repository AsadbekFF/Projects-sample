using Microsoft.EntityFrameworkCore;
using Sample.BLL.Infrastructure;
using Sample.Common.Entities;
using Sample.Common.Exceptions;
using Sample.DAL.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.BLL.Services
{
    public class UserService : BaseService<User, int, MasterContext>
    {
        public UserService(MasterContext context) : base(context)
        {
        }

        public async Task<DateTime?> GetLastUpdatedTimeAsync(int id)
        {
            return await Context.Users.Where(u => u.Id == id).Select(u => u.UpdatedDateTime).FirstOrDefaultAsync();
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            if (!(await Context.Users.AnyAsync(u => u.Username == username && u.Password == password)))
            {
                throw new ForbiddenException();
            }

            return await Context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        protected override async Task<IEnumerable<ValidationResult>> ValidateAsync(User user)
        {
            var result = new List<ValidationResult>();

            // Check duplicate username
            await CheckDuplicatesAsync(user, result, nameof(User.Username), "User with this username already exists.",
                u => u.Username == user.Username);

            return result;
        }
    }
}
