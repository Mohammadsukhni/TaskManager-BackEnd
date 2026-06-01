using TaskManager.Core.Dto;
using TaskManager.Core.Entities;
using TaskManager.Core.Exceptions;
using TaskManager.Core.Helper;
using TaskManager.Core.IRepositories;
using TaskManager.Core.IService;
using TaskManager.Core.Mapping;

namespace TaskManager.Infrastructure.Service
{
    public class SprintServices : ISprintServices
    {
        private readonly IUnitOfWork _unitOfWork;

        public SprintServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateSprintAsync(SprintDto dto)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(dto.ProjectId);

            if (project == null)
                throw new BadRequestException("Project not found.");

            if (dto.DateFrom > dto.DateTo)
                throw new BadRequestException("Sprint start date cannot be after end date.");

            if (dto.DateFrom.Date < DateTime.Today)
                throw new BadRequestException("Sprint start date cannot be in the past.");

            if (dto.DateTo.Date < DateTime.Today)
                throw new BadRequestException("Sprint end date cannot be in the past.");

            var sprint = dto.ToEntity();

            sprint.ReferenceNumber =
                ReferenceNumberHelper.GenerateSprintReference();

            await _unitOfWork.Sprints.CreateAsync(sprint);
        }

        public async Task<PagedResultDto<SprintDto>> GetAllSprintsAsync(
            int pageNumber,
            int pageSize)
        {
            var sprints = await _unitOfWork.Sprints.GetPagedAsync(pageNumber, pageSize);

            return sprints.ToPagedDto(x => x.ToDto());
        }

        public async Task<SprintFilterResultDto> FilterSprintsAsync(
            string? search,
            int pageNumber,
            int pageSize)
        {
            var term = NormalizeSearch(search);

            PagedResultDto<Sprint> sprints;

            if (string.IsNullOrWhiteSpace(term))
            {
                sprints = await _unitOfWork.Sprints.GetPagedAsync(pageNumber, pageSize);
            }
            else
            {
                var hasDate = DateTime.TryParse(term, out var date);

                sprints = await _unitOfWork.Sprints.GetPagedAsync(
                    x => x.Name.Contains(term) ||
                         x.ReferenceNumber.Contains(term) ||
                         (hasDate && (x.DateFrom.Date == date.Date ||
                                      x.DateTo.Date == date.Date)),
                    pageNumber,
                    pageSize);
            }

            var sprintIds = sprints.Items.Select(x => x.Id).ToList();

            var workItems = sprintIds.Count == 0
                ? new List<WorkItem>()
                : await _unitOfWork.WorkItems.GetAllAsync(x => sprintIds.Contains(x.SprintId));

            return new SprintFilterResultDto
            {
                Sprints = sprints.ToPagedDto(x => x.ToDto()),
                WorkItems = workItems.Select(x => x.ToDto()).ToList()
            };
        }

        public async Task<SprintDto?> GetSprintByIdAsync(int id)
        {
            var sprint = await _unitOfWork.Sprints.GetByIdAsync(id);

            if (sprint == null)
                return null;

            return sprint.ToDto();
        }

        public async Task UpdateSprintAsync(SprintDto dto)
        {
            var sprint = await _unitOfWork.Sprints.GetByIdAsync(dto.Id);

            if (sprint == null)
                return;

            var project = await _unitOfWork.Projects.GetByIdAsync(dto.ProjectId);

            if (project == null)
                throw new BadRequestException("Project not found.");

            if (dto.DateFrom > dto.DateTo)
                throw new BadRequestException("Sprint start date cannot be after end date.");

            if (dto.DateFrom.Date < DateTime.Today)
                throw new BadRequestException("Sprint start date cannot be in the past.");

            if (dto.DateTo.Date < DateTime.Today)
                throw new BadRequestException("Sprint end date cannot be in the past.");

            sprint.Name = dto.Name;
            sprint.DateFrom = dto.DateFrom;
            sprint.DateTo = dto.DateTo;
            sprint.ProjectId = dto.ProjectId;

            await _unitOfWork.Sprints.Update(sprint);
        }

        public async Task DeleteSprintAsync(int id)
        {
            var sprint = await _unitOfWork.Sprints.GetByIdAsync(id);

            if (sprint == null)
                return;

            await _unitOfWork.Sprints.Delete(sprint);
        }

        public async Task<PagedResultDto<SprintDto>> GetUserSprintsAsync(
            int userId,
            int pageNumber,
            int pageSize)
        {
            var userProjects = await _unitOfWork.UserProjects.GetAllAsync();

            var projectIds = userProjects
                .Where(x => x.UserId == userId)
                .Select(x => x.ProjectId)
                .ToList();

            var sprints = await _unitOfWork.Sprints.GetPagedAsync(
                x => projectIds.Contains(x.ProjectId),
                pageNumber,
                pageSize);

            return sprints.ToPagedDto(x => x.ToDto());
        }

        private static string NormalizeSearch(string? search)
        {
            return search?.Trim() ?? string.Empty;
        }
    }
}
