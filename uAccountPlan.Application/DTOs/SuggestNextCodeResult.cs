namespace uAccountPlan.Application.DTOs;

public class SuggestNextCodeResult
{
    public string SuggestedCode { get; set; } = default!;
    public Guid? SuggestedParentId { get; set; }
}