using AutoMapper;
using AutoMapper.QueryableExtensions;
using ChatQueueManagementSystem.Application.Common.Helpers;
using ChatQueueManagementSystem.Application.Common.Helpers.PaginationHelper;
using ChatQueueManagementSystem.Application.Common.Interfaces.Repositories;
using ChatQueueManagementSystem.Domain.Entities;
using MediatR;

namespace ChatQueueManagementSystem.Application.Features.Team.Queries
{
	public static class GetAllTeamsCapacity
	{
		public class Query : IRequest<Result> , IPagedRequest
		{
			public int Page { get; set; }
			public int PageLength { get; set; }
		}

		public class Result: PagedResult<TeamsCapacityResult>
		{

		}

		public class TeamsCapacityResult 
		{
			public Guid Id { get; set; }
			public string Name { get; set; }
			public int Capacity { get; set; }
			public List<Agent> Agents { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result>
		{
			private readonly ITeamRepository _teamRepository;
			private readonly IMapper _mapper;

			public Handler(ITeamRepository teamRepository, IMapper mapper)
			{
				_teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
				_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
			}

			public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
			{
				var allTeamResult = await _teamRepository
					.GetAllAsQueryable()
					.ProjectTo<TeamsCapacityResult>(_mapper.ConfigurationProvider)
					.ToPagedResultsAsync(request);

				foreach (var team in allTeamResult.Items)
				{
					team.Capacity = ChatsHelper.CalculateCurrentChatCapacity(team.Agents);
				}

				return _mapper.Map<GetAllTeamsCapacity.Result>(allTeamResult);
			}
		}
	}
}