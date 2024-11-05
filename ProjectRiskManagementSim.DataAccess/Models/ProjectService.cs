using Microsoft.EntityFrameworkCore;
namespace ProjectRiskManagementSim.DataAccess.Models;

public class ProjectService
{
    private readonly OxygenAnalyticsContext _context;

    public ProjectService(OxygenAnalyticsContext context)
    {
        _context = context;
    }

    public async Task<List<ProjectJira>> GetProjectsAsync()
    {
        return await _context.Projects.ToListAsync();
    }
    public async Task<List<ProjectJira>> GetAllProjectsAsync()
    {
        return await _context.GetProjectsAsync();
    }
}
