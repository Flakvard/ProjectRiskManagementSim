
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
        StaffAnalysisList.Add(new StaffAnalysis { Priority = 1, StaffName = "None" });
        StaffAnalysisList.Add(new StaffAnalysis { Priority = 2, StaffName = "None" });
        StaffAnalysisList.Add(new StaffAnalysis { Priority = 3, StaffName = "None" });
        StaffAnalysisList.Add(new StaffAnalysis { Priority = 4, StaffName = "None" });
        StaffAnalysisList.Add(new StaffAnalysis { Priority = 5, StaffName = "None" });
    }
}
public class StaffAnalysis
{
    public int Priority { get; set; }
    public string StaffName { get; set; }
}
