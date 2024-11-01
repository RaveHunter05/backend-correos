public class CostCenter
{
	public int CostCenterId {get; set;}

	public string GerencyCode {get; set;}
	public string AreaCode {get; set;}
	public string OfficeCode {get; set;}
	public string Code {get; set;}
	public string Name {get; set;}
	public DateTime? Date {get; set;}

	public ICollection <Expense> Expenses {get;} = new List<Expense>(); 
	public ICollection <Income> Incomes {get;} = new List<Income>();
}
