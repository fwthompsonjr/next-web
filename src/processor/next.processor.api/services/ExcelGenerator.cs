using legallead.records.search.Classes;
using legallead.records.search.Models;
using legallead.records.search;
using OfficeOpenXml;
using next.processor.api.interfaces;

namespace next.processor.api.services
{
    public class ExcelGenerator : IExcelGenerator
    {
        public ExcelPackage? GetAddresses(WebFetchResult fetchResult)
        {
            string extXml = CommonKeyIndexes.ExtensionXml;
            string extFile = CommonKeyIndexes.ExtensionXlsx;
            string tmpFileName = fetchResult.Result.Replace(extXml, extFile);
            if (fetchResult.WebsiteId == 0) { fetchResult.WebsiteId = 1; }
            ExcelWriter writer = new();
            return writer.ConvertToPersonTable(
            addressList: fetchResult.PeopleList,
            worksheetName: "Addresses",
            saveFile: false,
            outputFileName: tmpFileName,
            websiteId: fetchResult.WebsiteId);
        }
        public byte[] SerializeResult(ExcelPackage package)
        {
            try
            {
                using var ms = new MemoryStream();
                package.SaveAs(ms);
                return ms.ToArray();
            }
            catch (Exception)
            {
                return [];
            }
        }
    }
}
