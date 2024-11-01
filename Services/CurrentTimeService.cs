namespace correos_backend.Services
{
    public class CurrentTimeService
    {
	public DateTime GetCurrentTime()
	{
		var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
		return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
	}
    }
}
