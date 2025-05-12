public class AccountPlanDto
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Type { get; set; } = default!;
    public bool AcceptsLaunches { get; set; }
    public Guid? ParentId { get; set; }
}