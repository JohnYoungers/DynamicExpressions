using DynamicExpressions.Tests.Linq.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicExpressions.Tests.Linq
{
    public class LinqTestBase
    {
        protected List<Entity> mockData;

        [TestInitialize]
        public void Initialization()
        {
            mockData = Entity.GetList();
        }
    }
}
