namespace ProjectRiskManagementSim.SimulationBlazor.Components.Pages.Dashboard;

public class KanbanColumnModel
{
    public string Status { get; set; }
    public int Wip { get; set; }
    public int LowB { get; set; }
    public int HighB { get; set; }
    public int Buffer { get; set; }
    public bool IsBufferActive { get; set; }

    public static List<KanbanColumnModel> InitializeKanbanColumns()
    {
        return new List<KanbanColumnModel>
      {
        new KanbanColumnModel { Status = "Open", Wip = 20, LowB = 2, HighB = 50, Buffer = 0, IsBufferActive = true },
        new KanbanColumnModel { Status = "In Progress", Wip = 3, LowB = 1, HighB = 30, Buffer = 0, IsBufferActive = false },
        new KanbanColumnModel { Status = "Finished", Wip = 20, LowB = 2, HighB = 40, Buffer = 0, IsBufferActive = true },
        new KanbanColumnModel { Status = "Ready to test on Development", Wip = 17, LowB = 2, HighB = 50, Buffer = 0, IsBufferActive = true },
        new KanbanColumnModel { Status = "Testing on Development", Wip = 2, LowB = 1, HighB = 52, Buffer = 0, IsBufferActive = false },
        new KanbanColumnModel { Status = "Ready to test on Production", Wip = 10, LowB = 2, HighB = 45, Buffer = 0, IsBufferActive = true },
        new KanbanColumnModel { Status = "Testing on Production", Wip = 1, LowB = 5, HighB = 70, Buffer = 0, IsBufferActive = false },
        new KanbanColumnModel { Status = "Done", Wip = 20, LowB = 1, HighB = 30, Buffer = 0, IsBufferActive = true },
        new KanbanColumnModel { Status = "Ready to test on Production", Wip = 10, LowB = 2, HighB = 45, Buffer = 0, IsBufferActive = true },
        new KanbanColumnModel { Status = "Testing on Production", Wip = 1, LowB = 5, HighB = 70, Buffer = 0, IsBufferActive = false },
        new KanbanColumnModel { Status = "Done", Wip = 20, LowB = 1, HighB = 30, Buffer = 0, IsBufferActive = true }
    };
    }
}
