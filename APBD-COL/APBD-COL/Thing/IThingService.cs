using APBD_COL.Thing.ResponseModels;

namespace APBD_COL.Thing;

public interface IThingService
{
    Task<List<ThingClass>> GetThings(String? name);
    
    Task<ThingClass> AddThing(ThingClass thing);
}