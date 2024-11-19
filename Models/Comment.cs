public class Comment
{
	public int CommentId {get; set;}
	public string CreatedById {get; set;}
	public string CommentText {get; set;}

	public int BudgetId {get; set;}


	// dates
	public DateTime ModifiedDate {get; set;}
	public DateTime Date {get; set;}

	// keys
	public Budget? Budget {get;}
}
