using correos_backend.Models.Enums;

public class Budget
{
	public int BudgetId {get; set;}


	// select between "Expense" and "Income"
	public string BudgetType {get; set;}

	public string Title {get; set;}
	public string Description {get; set;}

	// select status between "Pending", "Approved", "Rejected"
	public ApprovalStatus? ApprovalStatus {get; set;}

	public string FileName {get; set;}
	public string FileUrl {get; set;}
	public long? FileSize {get; set;}

	public DateTime ModifiedDate {get; set;}
	// created at
	public DateTime Date {get; set;}


	// keys
	public string? CreatedById {get; set;}
	public string? CreatedByName {get; set;}

	public ICollection<Comment> Comments {get;} = new List<Comment>();

}
