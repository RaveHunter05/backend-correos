using correos_backend.Models.Enums;

public class Report
{
	public int ReportId {get; set;}

	public string Title {get; set;}
	public string Description {get; set;}
	public int AuthorId {get; set;}
	public DateTime Date {get; set;} = DateTime.Now;

	public ApprovalStatus Status {get; set;} = ApprovalStatus.Pending;
	public ReviewStatus ReviewStatus {get; set;} = ReviewStatus.NotReviewed;
	public string ReviewerComments {get; set;} = "";

	public DateTime ApprovalDate {get; set;}

	public string FileName {get; set;}
	public string FileUrl {get; set;}
	public long? FileSize {get; set;}

}
