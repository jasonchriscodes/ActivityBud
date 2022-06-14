using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Activities;
using AutoMapper;
using Domain;

namespace Application.Core
{
 public class MappingProfiles : Profile
 {
  public MappingProfiles()
  {
   CreateMap<Activity, Activity>(); // map from Activity to Activity must match with that in edit handler
   CreateMap<Activity, ActivityDto>() // map from Activity to ActivityDto
   .ForMember(d => d.HostUsername, opt => opt.MapFrom(s => s.Attendees
   .FirstOrDefault(x => x.IsHost).AppUser.UserName));
   CreateMap<ActivityAttendee, AttendeeDto>() // map from AppUser to Profile, need specific because autoMapper also use profile
   .ForMember(d => d.DisplayName, opt => opt.MapFrom(s => s.AppUser.DisplayName))
   .ForMember(d => d.Username, opt => opt.MapFrom(s => s.AppUser.UserName))
   .ForMember(d => d.Bio, opt => opt.MapFrom(s => s.AppUser.Bio))
   .ForMember(d => d.Image, opt => opt.MapFrom(s => s.AppUser.Photos.FirstOrDefault(x => x.IsMain).Url));
   CreateMap<AppUser, Profiles.Profile>()
   .ForMember(d => d.Image, opt => opt.MapFrom(s => s.Photos.FirstOrDefault(x => x.IsMain).Url));
  }
 }
}
