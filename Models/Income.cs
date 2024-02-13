public class Income
{
	public int IncomeId {get; set;}

	public decimal ProjectedAmount {get; set;}
	public decimal ExecutedAmount {get; set;}
	public DateTime Date {get; set;}
	// keys
	public int ServiceId {get; set;}
	public int CostCenterId {get; set;}
	public Service Service {get; set;}
	public CostCenter CostCenter {get; set;}
}
