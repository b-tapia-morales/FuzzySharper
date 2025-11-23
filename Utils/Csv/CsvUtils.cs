using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using static Utils.Csv.DelimiterType;

namespace Utils.Csv;

public static class CsvUtils
{
    public static List<T> RetrieveRows<T, TMap>(string filePath,
        bool hasHeader = false, DelimiterType type = Semicolon) where TMap : ClassMap
    {
        var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Encoding = Encoding.UTF8,
            Delimiter = Delimiter.ToValue(type).Character,
            HasHeaderRecord = hasHeader
        };

        using var textReader = new StreamReader(filePath, Encoding.UTF8);
        using var csv = new CsvReader(textReader, configuration);
        csv.Context.RegisterClassMap<TMap>();
        return csv.GetRecords<T>().ToList();
    }
}