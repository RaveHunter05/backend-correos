public class Service
{
	public int ServiceId {get; set;}

	public int Code {get; set;}
	public string Name {get; set;}
	public DateTime Date {get; set;}

	public ICollection<Income> Incomes {get; set;} = new List<Income>();
}
