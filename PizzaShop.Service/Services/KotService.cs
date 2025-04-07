using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class KotService : IKotService
{
    private readonly IGenericRepository<Category> _categoryRepository;

    public KotService(IGenericRepository<Category> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<List<CategoryViewModel>> Get()
    {
        IEnumerable<Category>? categories =  await _categoryRepository.GetByCondition(
            predicate: c => !c.IsDeleted,
            orderBy: q => q.OrderBy(c => c.Id));

        List<CategoryViewModel> list = categories.Select(c => new CategoryViewModel{
            Id = c.Id,
            Name = c.Name
        }).ToList();

        return list;
    }

}
