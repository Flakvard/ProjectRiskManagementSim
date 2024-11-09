
public class ListStaffAnalysis
{
    public List<StaffAnalysis> StaffAnalysisList { get; set; }
    public ListStaffAnalysis()
    {
        StaffAnalysisList = new List<StaffAnalysis>();
        LoadStaffAnalysis();
    }

    private void LoadStaffAnalysis()
    {
        StaffAnalysisList.Add(new StaffAnalysis { Priority = 1, StaffName = "Testing on Production" });
        StaffAnalysisList.Add(new StaffAnalysis { Priority = 2, StaffName = "Testing on Production" });
        StaffAnalysisList.Add(new StaffAnalysis { Priority = 3, StaffName = "Testing on Development" });
        StaffAnalysisList.Add(new StaffAnalysis { Priority = 4, StaffName = "In Progress" });
        StaffAnalysisList.Add(new StaffAnalysis { Priority = 5, StaffName = "Testing on Production" });
    }
}
public class StaffAnalysis
{
    public int Priority { get; set; }
    public string StaffName { get; set; }
}
