using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.CommonService
{
    public interface ICommonService
    {
        Task<IQueryable<object>> GetAllEntities();
        Task<object> GetEntityByID(int entityID);
        Task<IQueryable<object>> GetEntityByName(string EntityName);
    }
}
