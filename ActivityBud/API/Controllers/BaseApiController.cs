using Application.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace API.Controllers
{
 // attribute
 [ApiController]
 [Route("api/[controller]")] // need to match url in the postman
 public class BaseApiController : ControllerBase
 {
  private IMediator mediator;

  /// <summary>
  /// Inject mediator to ActivitiesController() so 
  /// that database is queried and return activity to the client
  /// applicable to BaseApiController class
  /// </summary>
  protected IMediator Mediator => mediator ??= HttpContext.RequestServices
      .GetService<IMediator>();//??= means a null coalescing assignment operator if mediator null, assign mediator service

  protected ActionResult HandleResult<T>(Result<T> result)
  {
   if (result == null)
   {
    return NotFound();
   }
   if (result.IsSuccess && result.Value != null)
   {
    return Ok(result.Value);
   }
   if (result.IsSuccess && result.Value == null)
   {
    return NotFound();
   }
   return BadRequest(result.Error);
  }
 }
}
