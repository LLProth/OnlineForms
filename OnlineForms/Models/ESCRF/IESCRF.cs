using System;

namespace OnlineForms.Models.ESCRF
{
    public interface IESCRF
    {
        string ID();
        string Name();
        string ChangeType();
        string CurrentSupervisor();
        string NewSupervisor();
        string ModifiedBy();
        DateTime CreatedDate();
        DateTime EffectiveDate { get; set; }
        int TaskListID();
        string Status();
        void CreateStatus();
    }
}
