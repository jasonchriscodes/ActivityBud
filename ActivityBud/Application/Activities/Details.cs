using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
 public class Details
 {
  /// <summary>
  /// Query class that fetching activity 
  /// </summary> return activity
  public class Query : IRequest<Result<ActivityDto>>
  {
   public Guid Id { get; set; }
  }
  // 
  /// <summary>
  /// Handler class
  /// </summary> return activity
  public class Handler : IRequestHandler<Query, Result<ActivityDto>>
  {
   private readonly DataContext context;

   /// <summary>
   // Handler constructor
   /// </summary>
   /// <param name="context"></param>
   private readonly IMapper mapper;
   public Handler(DataContext context, IMapper mapper)
   {
    this.mapper = mapper;
    this.context = context;
   }
   /// <summary>
   /// 
   /// </summary>
   /// <param name="request"></param>
   /// <param name="cancellationToken"></param>
   /// <returns>activity of specific id</returns>
   public async Task<Result<ActivityDto>> Handle(Query request, CancellationToken cancellationToken)
   {
    var activity = await this.context.Activities
    .ProjectTo<ActivityDto>(this.mapper.ConfigurationProvider)
    .FirstOrDefaultAsync(x => x.Id == request.Id);
    return Result<ActivityDto>.Success(activity);
   }
  }
 }
}
