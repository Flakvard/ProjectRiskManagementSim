public class ListSensitivityAnalysis
{
    public List<SensitivityAnalysis> SensitivityAnalysisList { get; set; }
    public ListSensitivityAnalysis()
    {
        SensitivityAnalysisList = new List<SensitivityAnalysis>();
        LoadSensitivityAnalysis();
    }

    private void LoadSensitivityAnalysis()
    {
        SensitivityAnalysisList.Add(new SensitivityAnalysis { Priority = 1, SensitivityName = "None" });
        SensitivityAnalysisList.Add(new SensitivityAnalysis { Priority = 2, SensitivityName = "None" });
        SensitivityAnalysisList.Add(new SensitivityAnalysis { Priority = 3, SensitivityName = "None" });
        SensitivityAnalysisList.Add(new SensitivityAnalysis { Priority = 4, SensitivityName = "None" });
        SensitivityAnalysisList.Add(new SensitivityAnalysis { Priority = 5, SensitivityName = "None" });
    }
}
public class SensitivityAnalysis
{
    public int Priority { get; set; }
    public string SensitivityName { get; set; }
}
