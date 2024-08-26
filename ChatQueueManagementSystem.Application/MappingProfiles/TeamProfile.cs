using AutoMapper;
using ChatQueueManagementSystem.Application.Common.Helpers.PaginationHelper;
using ChatQueueManagementSystem.Application.Features.Team.Queries;
using ChatQueueManagementSystem.Domain.Entities;

namespace ChatQueueManagementSystem.Application.MappingProfiles
{
	public class TeamProfile : Profile
	{
		public TeamProfile()
		{
			CreateMap<Team, GetAllTeamsCapacity.TeamsCapacityResult>().ReverseMap();

			CreateMap<PagedResult<GetAllTeamsCapacity.TeamsCapacityResult>, GetAllTeamsCapacity.Result>()
				.ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
				.ForMember(dest => dest.PageLength, opt => opt.MapFrom(src => src.PageLength))
				.ForMember(dest => dest.CurrentPage, opt => opt.MapFrom(src => src.CurrentPage))
				.ForMember(dest => dest.PageCount, opt => opt.MapFrom(src => src.PageCount))
				.ForMember(dest => dest.ItemCount, opt => opt.MapFrom(src => src.ItemCount))
				.ReverseMap();
		}
	}
}