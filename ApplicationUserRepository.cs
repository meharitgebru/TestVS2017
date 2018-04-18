using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WMS_DAL.Models;

namespace WMS_DAL.Repository
{
    public class ApplicationUserRepository : GenericRepository<User>, IApplicationUserRepository
    {
        private WMSContext _context;
        public ApplicationUserRepository(WMSContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VM>> GetRolesAsync<VM>(string userName, Expression<Func<Role, VM>> columns)
        {
            //return  await _context.User
            //              .Include(ur=>ur.UserRole).ThenInclude(x=>x.Role)
            //              .Where(r=>r.UserName==userName)
            //              .Select(x=>x.UserRole.Role.RoleName).ToListAsync();

            var  quray = _context.User
                          .Include(ur => ur.UserRole).ThenInclude(x => x.Role)
                          .Where(r => r.UserName == userName)
                          //.Select(x=>x.UserRole)
                          .AsNoTracking();

            return await _context.Set<Role>().Where(r => r.UserRole.Select(u => u.User.UserName).Contains(userName))
                               .Select<Role, VM>(columns).ToListAsync();



        }
    }
}
