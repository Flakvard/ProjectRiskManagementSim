
public class ProjectList
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? Manager { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Status { get; set; }
    public bool Selected { get; set; }


    public static List<ProjectList> Init() => new List<ProjectList>
    {
        new ProjectList { Name = "Project 1", Type = "Type 1", Manager = "Manager 1", StartDate = new DateTime(2021, 1, 1), EndDate = new DateTime(2022, 1, 1), Status = "Active" ,Selected = false},
        new ProjectList { Name = "Project 2", Type = "Type 2", Manager = "Manager 2", StartDate = new DateTime(2021, 1, 1), EndDate = new DateTime(2022, 1, 1), Status = "Active" ,Selected = false},
        new ProjectList { Name = "Project 3", Type = "Type 3", Manager = "Manager 3", StartDate = new DateTime(2021, 1, 1), EndDate = new DateTime(2022, 1, 1), Status = "Active" ,Selected = false},
        new ProjectList { Name = "Project 4", Type = "Type 4", Manager = "Manager 4", StartDate = new DateTime(2021, 1, 1), EndDate = new DateTime(2022, 1, 1), Status = "Active" ,Selected = false},
        new ProjectList { Name = "Project 5", Type = "Type 5", Manager = "Manager 5", StartDate = new DateTime(2021, 1, 1), EndDate = new DateTime(2022, 1, 1), Status = "Active" ,Selected = false},
        new ProjectList { Name = "Project 6", Type = "Type 6", Manager = "Manager 6", StartDate = new DateTime(2021, 1, 1), EndDate = new DateTime(2022, 1, 1), Status = "Active" ,Selected = false},
        new ProjectList { Name = "Project 7", Type = "Type 7", Manager = "Manager 7", StartDate = new DateTime(2021, 1, 1), EndDate = new DateTime(2022, 1, 1), Status = "Active" ,Selected = false},
        new ProjectList { Name = "Project 8", Type = "Type 8", Manager = "Manager 8", StartDate = new DateTime(2021, 1, 1), EndDate = new DateTime(2022, 1, 1), Status = "Active" ,Selected = false},
        new ProjectList { Name = "Project 9", Type = "Type 9", Manager = "Manager 9", StartDate = new DateTime(2021, 1, 1), EndDate = new DateTime(2022, 1, 1), Status = "Active" ,Selected = false},
    };
}
