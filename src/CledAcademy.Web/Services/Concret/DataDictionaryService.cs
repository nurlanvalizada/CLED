using System.Collections.Generic;
using System.Linq;
using CledAcademy.Core.Domain;
using CledAcademy.Repository.UOW;
using CledAcademy.Web.Services.Abstract;

namespace CledAcademy.Web.Services.Concret
{
    public class DataDictionaryService : IDataDictionaryService
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public DataDictionaryService(IUnitOfWorkManager unitOfWorkManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }

        public DataDictionary GetSingle(string key)
        {
           return _unitOfWorkManager.Repository<DataDictionary>().GetSingle(dd => dd.Key == key);
        }

        public List<DataDictionary> Get(IEnumerable<string> keys)
        {
            return _unitOfWorkManager.Repository<DataDictionary>().FindBy(dd => keys.Contains(dd.Key)).ToList();
        }
    }
}
