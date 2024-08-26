using AutoMapper;
using ChatQueueManagementSystem.Application.Features.ChatSession.Queries;
using ChatQueueManagementSystem.Domain.Entities;

namespace ChatQueueManagementSystem.Application.MappingProfiles
{
	public class ChatSessionProfile : Profile
	{
		public ChatSessionProfile()
		{
			CreateMap<ChatSession, GetChatSessionStatus.ChatSessionStatusResult>()
				.ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.AgentId, opt => opt.MapFrom(src => src.AgentId))
				.ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
				.ForMember(dest => dest.QueueId, opt => opt.MapFrom(src => src.QueueId))
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
				.ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message))
				.ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
				.ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
				.ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime))
				.ReverseMap();
		}
	}
}