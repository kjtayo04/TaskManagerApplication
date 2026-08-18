using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagerApplication.Domain.Entities;
using TaskManagerApplication.Domain.Interfaces;
using TaskManagerApplication.Infrastructure.Data;

namespace TaskManagerApplication.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _db;

        public TaskRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<TaskItem>> GetAllAsync()
        {
            return await _db.Tasks.AsNoTracking().ToListAsync();
        }

        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            return await _db.Tasks.FindAsync(id);
        }

        public async Task<TaskItem> AddAsync(TaskItem item)
        {
            _db.Tasks.Add(item);
            await _db.SaveChangesAsync();
            return item;
        }

        public async Task UpdateAsync(TaskItem item)
        {
            _db.Tasks.Update(item);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _db.Tasks.FindAsync(id);
            if (item == null) return;
            _db.Tasks.Remove(item);
            await _db.SaveChangesAsync();
        }
    }
}
