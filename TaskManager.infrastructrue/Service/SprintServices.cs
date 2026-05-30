using TaskManager.Core.Dto;
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
                throw new InvalidOperationException("Project not found.");

            if (dto.DateFrom > dto.DateTo)
                throw new InvalidOperationException("Sprint start date cannot be after end date.");

            if (dto.DateFrom.Date < DateTime.Today)
                throw new InvalidOperationException("Sprint start date cannot be in the past.");

            if (dto.DateTo.Date < DateTime.Today)
                throw new InvalidOperationException("Sprint end date cannot be in the past.");

            var sprint = dto.ToEntity();

            sprint.ReferenceNumber =
                ReferenceNumberHelper.GenerateSprintReference();

            await _unitOfWork.Sprints.CreateAsync(sprint);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<SprintDto>> GetAllSprintsAsync()
        {
            var sprints = await _unitOfWork.Sprints.GetAllAsync();

            return sprints.Select(x => x.ToDto()).ToList();
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
                throw new InvalidOperationException("Project not found.");

            if (dto.DateFrom > dto.DateTo)
                throw new InvalidOperationException("Sprint start date cannot be after end date.");

            if (dto.DateFrom.Date < DateTime.Today)
                throw new InvalidOperationException("Sprint start date cannot be in the past.");

            if (dto.DateTo.Date < DateTime.Today)
                throw new InvalidOperationException("Sprint end date cannot be in the past.");

            sprint.Name = dto.Name;
            sprint.DateFrom = dto.DateFrom;
            sprint.DateTo = dto.DateTo;
            sprint.ProjectId = dto.ProjectId;

            _unitOfWork.Sprints.Update(sprint);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteSprintAsync(int id)
        {
            var sprint = await _unitOfWork.Sprints.GetByIdAsync(id);

            if (sprint == null)
                return;

            _unitOfWork.Sprints.Delete(sprint);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<SprintDto>> GetUserSprintsAsync(int userId)
        {
            var userProjects = await _unitOfWork.UserProjects.GetAllAsync();

            var projectIds = userProjects
                .Where(x => x.UserId == userId)
                .Select(x => x.ProjectId)
                .ToList();

            var sprints = await _unitOfWork.Sprints.GetAllAsync();

            return sprints
                .Where(x => projectIds.Contains(x.ProjectId))
                .Select(x => x.ToDto())
                .ToList();
        }
    }
}
