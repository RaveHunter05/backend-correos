public class Spent
{
	public int SpentId {get; set;}

	public string Category {get; set;}
	public string Denomination {get; set;}
	public DateTime Date {get; set;}

	public ICollection<Expense> Expenses {get;} = new List<Expense>();
}
