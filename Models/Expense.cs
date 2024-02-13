public class Expense
{
	public int ExpenseId {get; set;}

	public decimal ProjectedAmount {get; set;}
	public decimal ExecutedAmount {get; set;}
	public DateTime Date {get; set;}

	// keys
	public int SpentId {get; set;}
	public int CostCenterId {get; set;}
	public Spent Spent {get; set;}
	public CostCenter CostCenter {get; set;}
}
