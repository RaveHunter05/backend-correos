public class Budget
{
	public int BudgetId {get; set;}


	// select between "Expense" and "Income"
	public string BudgetType {get; set;}

	public string Title {get; set;}
	// select status between "Draft", "Approved", "Rejected"
	public string Status {get; set;}
	public string Description {get; set;}

	public string BudgetDocumentLink {get; set;}

	// sugested data
	// public double Amount {get; set;}
	// public double Balance {get; set;}
	// public double Total {get; set;}
	// public double TotalExpense {get; set;}
	// public double TotalIncome {get; set;}
	// public double TotalBudget {get; set;}
	// public double TotalBalance {get; set;}
	// public double TotalTotal {get; set;}

	public DateTime ModifiedDate {get; set;}
	public DateTime Date {get; set;}


	// keys
	public string CreatedById {get; set;}

	public ICollection<Comment> Comments {get;} = new List<Comment>();

}
