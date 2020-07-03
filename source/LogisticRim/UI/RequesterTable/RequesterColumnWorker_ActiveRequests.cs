using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticRim
{
    class RequesterColumnWorker_ActiveRequests : ColumnWorker_DataGetter<LogisticRequester>
    {
        public override string GetData ( LogisticRequester entry )
        {
            return entry.activeRequestCount.ToString();
        }
    }
}
