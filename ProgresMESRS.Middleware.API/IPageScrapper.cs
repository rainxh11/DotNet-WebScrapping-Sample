using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgresMESRS.Middleware.API
{
    public interface IPageScrapper<T>
    {
        IEnumerable<T> ExtractData(string pageHtml);
        T ExtractSingleData(string pageHtml, string id);
    }
}
