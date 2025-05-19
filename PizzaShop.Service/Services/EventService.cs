using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Services;

public class EventService : IEventService
{
    private readonly IGenericRepository<Event> _eventRepository;
    private readonly IGenericRepository<EventStatus> _eventStatusRepository;
    private readonly IGenericRepository<EventType> _eventTypeRepository;

    public EventService(IGenericRepository<Event> eventRepository, IGenericRepository<EventStatus> eventStatusRepository, IGenericRepository<EventType> eventTypeRepository)
    {
        _eventRepository = eventRepository;
        _eventStatusRepository = eventStatusRepository;
        _eventTypeRepository = eventTypeRepository;
    }


    public EventIndexViewModel Get()
    {
        EventIndexViewModel model = new()
        {
            Statuses = _eventStatusRepository.GetAll().ToList(),
            Types = _eventTypeRepository.GetAll().ToList()
        };
        return model;
    }
}
