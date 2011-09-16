using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeServerData;

namespace ServiceHandlers
{
    static class DataEntityFactory
    {

        private static ServerDataEntities _dataEntities = new ServerDataEntities();

        public static ServerDataEntities GetDataEntities()
        {
            return _dataEntities;
        }
    }
}
