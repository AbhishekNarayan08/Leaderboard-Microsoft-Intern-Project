// <copyright file="EncodeDecodeIdHelperTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CosmosDBTest.Util
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using CosmosDB.Util;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EncodeDecodeIdHelperTests
    {
        [TestMethod]
        public void EncodeDecodeValidData()
        {
            string plainText = "Test+user/Id=";
            var encodedData = EncodeDecodeIdHelper.Encode(plainText);
            Assert.IsNotNull(encodedData);
            Assert.AreEqual(plainText, EncodeDecodeIdHelper.Decode(encodedData));

            Assert.AreEqual(
                "simple&test\\",
                EncodeDecodeIdHelper.Decode(EncodeDecodeIdHelper.Encode("simple&test\\")));
        }

        [TestMethod]
        public void EncodeValidData()
        {
            Assert.IsNull(EncodeDecodeIdHelper.Encode(null));
            Assert.IsNull(EncodeDecodeIdHelper.Decode(null));
            Assert.IsNull(EncodeDecodeIdHelper.Encode(string.Empty));
            Assert.IsNull(EncodeDecodeIdHelper.Decode(string.Empty));
        }
    }
}
