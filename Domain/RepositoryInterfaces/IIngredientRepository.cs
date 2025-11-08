using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Infrastructure.RepositoryInterfaces
{
    public interface IIngredientRepository : IGenericRepository<Ingredient>
    {
        Task<Ingredient?> GetByNameAsync(string name);
    }


}
