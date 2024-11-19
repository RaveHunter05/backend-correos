using OfficeOpenXml;

namespace correos_backend.Services
{
    public class XLSXService
    {
	public IEnumerable<T> ReadXLSXFile<T>(Stream fileStream) where T : class
	{
	    try
	    {
		using (var package = new ExcelPackage(fileStream))
		{
		    var worksheet = package.Workbook.Worksheets[0];
		    var rowCount = worksheet.Dimension.Rows;
		    var colCount = worksheet.Dimension.Columns;

		    var properties = typeof(T).GetProperties();
		    var records = new List<T>();

		    for (int row = 2; row <= rowCount; row++)
		    {
			var record = Activator.CreateInstance<T>();
			for (int col = 1; col <= colCount; col++)
			{
			    var property = properties.FirstOrDefault(p => p.Name == worksheet.Cells[1, col].Value.ToString());
			    if (property != null)
			    {
				property.SetValue(record, Convert.ChangeType(worksheet.Cells[row, col].Value, property.PropertyType));
			    }
			}
			records.Add(record);
		    }
		    return records;
		}
	    }
	    catch (Exception ex)
	    {
		throw new ApplicationException("Error reading XLSX file", ex);
	    }
	}
    }
}
