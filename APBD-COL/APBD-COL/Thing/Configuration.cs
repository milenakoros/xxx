using APBD_COL.Thing.ResponseModels;

namespace APBD_COL.Thing;

public static class Configuration
{
    public static void RegisterEndpointsForThing(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/get", async (IThingService service, String? name) =>
        {
            var things = await service.GetThings(name);
     
            return things.Count == 0 ? Results.NotFound() : Results.Ok(things);
        });
        
        app.MapPost("api/add", async (IThingService service, ThingClass thing) =>
        {
            try
            {
                var newThing = await service.AddThing(thing);
                return Results.Created("api/add", newThing);
            }
            catch (Exception e)
            {
                return Results.Problem(e.Message);
            }
        });
    }
}