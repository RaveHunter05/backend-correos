using CsvHelper;
using CsvHelper.TypeConversion;
using CsvHelper.Configuration;
using System.Globalization;

namespace correos_backend.Services
{
    public class CsvService
    {
        public IEnumerable<T> ReadCsvFile<T>(Stream fileStream) where T : class
        {
            try
            {
		var config = new CsvConfiguration(CultureInfo.InvariantCulture)
		{
			HeaderValidated = null,
			MissingFieldFound = null,
		};
                using (var reader = new StreamReader(fileStream))
                using (var csv = new CsvReader(reader, config))
                {
                    var records = csv.GetRecords<T>();
                    return records.ToList();
                }
            }
            catch (HeaderValidationException ex)
            {
                // Specific exception for header issues
                throw new ApplicationException("CSV file header is invalid.", ex);
            }
            catch (TypeConverterException ex)
            {
                // Specific exception for type conversion issues
                throw new ApplicationException("CSV file contains invalid data format.", ex);
            }
            catch (Exception ex)
            {
                // General exception for other issues
                throw new ApplicationException("Error reading CSV file", ex);
            }
        }
    }
}
