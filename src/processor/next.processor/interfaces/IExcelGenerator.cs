using legallead.records.search.Models;
using OfficeOpenXml;

namespace next.processor.api.interfaces
{
    public interface IExcelGenerator
    {
        ExcelPackage? GetAddresses(WebFetchResult fetchResult);
        byte[] SerializeResult(ExcelPackage package);
    }
}
