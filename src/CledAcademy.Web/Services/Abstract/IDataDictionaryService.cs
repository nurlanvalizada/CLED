using System.Collections.Generic;
using CledAcademy.Core.Domain;

namespace CledAcademy.Web.Services.Abstract
{
    public interface IDataDictionaryService
    {
        DataDictionary GetSingle(string key);

        List<DataDictionary> Get(IEnumerable<string> keys);
    }
}