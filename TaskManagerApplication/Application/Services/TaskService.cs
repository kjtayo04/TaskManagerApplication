using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagerApplication.Application.DTOs;
using TaskManagerApplication.Application.Interfaces;
using TaskManagerApplication.Domain.Entities;
using TaskManagerApplication.Domain.Interfaces;

namespace TaskManagerApplication.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _repository;

        public TaskService(ITaskRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<TaskItemDto>> GetAllAsync()
        {
            var items = await _repository.GetAllAsync();
            return items.Select(MapToDto);
        }

        public async Task<TaskItemDto?> GetByIdAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            return item == null ? null : MapToDto(item);
        }

        public async Task<TaskItemDto> CreateAsync(TaskItemDto dto)
        {
            var entity = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Priority = dto.Priority,
                DueDate = dto.DueDate,
                Status = dto.Status,
                EnteredOn = dto.EnteredOn == default ? System.DateTime.UtcNow : dto.EnteredOn
            };

            var created = await _repository.AddAsync(entity);
            return MapToDto(created);
        }

        public async Task UpdateAsync(TaskItemDto dto)
        {
            var existing = await _repository.GetByIdAsync(dto.Id);
            if (existing == null) return;
            existing.Title = dto.Title;
            existing.Description = dto.Description;
            existing.Priority = dto.Priority;
            existing.DueDate = dto.DueDate;
            existing.Status = dto.Status;
            // keep EnteredOn as-is
            await _repository.UpdateAsync(existing);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        private static TaskItemDto MapToDto(TaskItem t) => new TaskItemDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            Priority = t.Priority,
            DueDate = t.DueDate,
            Status = t.Status,
            EnteredOn = t.EnteredOn
        };
    }
}
