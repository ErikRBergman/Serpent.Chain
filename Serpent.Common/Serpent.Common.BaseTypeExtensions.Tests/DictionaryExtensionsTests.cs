using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Serpent.Common.BaseTypeExtensions.Tests
{
    using Serpent.Common.BaseTypeExtensions.Collections;

    [TestClass]
    public class DictionaryExtensionsTests
    {
        private class Item
        {
            public int Key { get; set; }

            public string Value { get; set; }

            public Item(int key, string value)
            {
                Key = key;
                Value = value;
            }
        }

        [TestMethod]
        public void GetValueOrDefaultTest()
        {
            var dictionary = new[] { new Item(1, "One"), new Item(2, "Two"), new Item(3, "Three"), }.ToDictionary(i => i.Key);

            Assert.IsNotNull(dictionary.GetValueOrDefault(1));
            Assert.IsNull(dictionary.GetValueOrDefault(0));

            var item = dictionary.GetValueOrDefault(2);

            Assert.IsNotNull(item);

            Assert.IsTrue(item.Key == 2 && item.Value == "Two");
        }

        [TestMethod]
        public void ToDictionaryTests()
        {
            var dictionary = new[] { new Item(1, "One"), new Item(2, "Two"), new Item(3, "Three"), }.ToDictionary(i => i.Key, i => i.Value);

            Assert.IsNotNull(dictionary.GetValueOrDefault(1));
            Assert.IsNull(dictionary.GetValueOrDefault(0));

            var value = dictionary.GetValueOrDefault(2);

            Assert.IsNotNull(value);

            Assert.AreEqual("Two", value);
        }
    }
}
