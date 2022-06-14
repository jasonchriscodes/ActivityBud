using Application.Activities;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
 public class ActivitiesController : BaseApiController
 {

  // end points
  [HttpGet] // using get to get all activity (result will be shown in URL)
  public async Task<IActionResult> GetActivities()
  {
   return HandleResult(await Mediator.Send(new List.Query())); // get response back from mediator handler
  }

  [HttpGet("{id}")] // using get to get an activity (result will be shown in URL)
  public async Task<IActionResult> GetActivities(Guid id)
  {
   return HandleResult(await Mediator.Send(new Details.Query { Id = id }));
  }

  [HttpPost] // using post method to create activity (safer than get)
  public async Task<IActionResult> CreateActivity(Activity activity)// IActionResul give access to http response task such as return OK, badrequest, not found
  {
   return HandleResult(await Mediator.Send(new Create.Command { Activity = activity }));
  }

  [Authorize(Policy = "IsActivityHost")] // using authorize to check if the user is the host of the activity
  [HttpPut("{id}")] // put for updating resources
  public async Task<IActionResult> EditActivity(Guid id, Activity activity) // add id to activity object before pass it to handler
  {
   activity.Id = id;
   return HandleResult(await Mediator.Send(new Edit.Command { Activity = activity }));
  }

  [Authorize(Policy = "IsActivityHost")]
  [HttpDelete("{id}")] // to delete resource
  public async Task<IActionResult> DeleteActivity(Guid id)// IActionResult (doest not require a type)
  {
   return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
  }

  [HttpPost("{id}/attend")] // to attend activity
  public async Task<IActionResult> Attend(Guid id)
  {
   return HandleResult(await Mediator.Send(new UpdateAttendance.Command { Id = id }));
  }
 }
}
