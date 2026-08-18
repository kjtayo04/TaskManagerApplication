using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerApplication.Application.DTOs;

namespace TaskManagerApplication.Application.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskItemDto>> GetAllAsync();
        Task<TaskItemDto?> GetByIdAsync(int id);
        Task<TaskItemDto> CreateAsync(TaskItemDto dto);
        Task UpdateAsync(TaskItemDto dto);
        Task DeleteAsync(int id);
    }
}
