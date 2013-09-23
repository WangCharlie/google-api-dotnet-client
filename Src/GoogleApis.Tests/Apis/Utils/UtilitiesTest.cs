/*
Copyright 2010 Google Inc

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.CodeDom;
using System.ComponentModel;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Google.Apis.Testing;
using Google.Apis.Util;

namespace Google.Apis.Tests.Apis.Util
{
    /// <summary>
    /// Tests for the "Utilities"-class.
    /// </summary>
    [TestFixture]
    public class UtilitiesTest
    {

        /// <summary>
        /// Tests that the AsReadOnly method will fail when provided invalid arguments.
        /// </summary>
        [Test]
        public void IDictionaryAsReadOnlyNullTest()
        {
            IDictionary<string, int> dict = null;
            Assert.Throws(typeof(ArgumentNullException), () => dict.AsReadOnly());
        }

        /// <summary>
        /// Tests whether the AsReadonly method returns a readonly copy of the dictionary.
        /// </summary>
        [Test]
        public void IDictionaryAsReadOnlyReadTest()
        {
            var dict = new Dictionary<int, string>();

            foreach (int i in Enumerable.Range(0, 10))
            {
                dict.Add(i, "" + i);
            }
            var readOnly = dict.AsReadOnly();

            foreach (int i in Enumerable.Range(0, 10))
            {
                Assert.AreEqual(i.ToString(), readOnly[i]);
            }

            foreach (int i in dict.Keys)
            {
                Assert.AreEqual(dict[i], readOnly[i]);
            }
        }

        /// <summary>
        /// Tests that a readonly dictionary is actually readonly.
        /// </summary>
        [Test]
        public void IDictionaryAsReadOnlyWriteTest()
        {
            var dict = new Dictionary<int, string>();

            foreach (int i in Enumerable.Range(0, 10))
            {
                dict.Add(i, "" + i);
            }
            var readOnly = dict.AsReadOnly();

            Assert.Throws(typeof(NotSupportedException), () => readOnly[0] = "fish");
            Assert.Throws(typeof(NotSupportedException), () => readOnly[0] = "0");
            Assert.Throws(typeof(NotSupportedException), () => readOnly[1] = "1");
            Assert.Throws(typeof(NotSupportedException), () => readOnly[500] = "NintySeven");
            Assert.Throws(typeof(NotSupportedException), () => readOnly.Clear());
            Assert.Throws(typeof(NotSupportedException), () => readOnly.Keys.Clear());
            Assert.Throws(typeof(NotSupportedException), () => readOnly.Values.Clear());
            Assert.Throws(typeof(NotSupportedException), () => readOnly.Add(15, "House"));
            Assert.Throws(typeof(NotSupportedException), () => readOnly.Remove(5));
        }

        /// <summary>
        /// Tests the "ThrowIfNull" method.
        /// </summary>
        [Test]
        public void ThrowIfNullTest()
        {
            string str = null;
            Assert.Throws(typeof(ArgumentNullException), () => str.ThrowIfNull("str"));
            str = "123";
            str.ThrowIfNull("Not throwen");
        }

        private enum MockEnum
        {
            [StringValue("Test")]
            EntryWithStringValue,
            [StringValue("3.14159265358979323846")]
            EntryWithSecondStringValue,
            EntryWithoutStringValue
        }

        /// <summary>
        /// Tests the "GetStringValue" extension method of enums.
        /// </summary>
        [Test]
        public void StringValueTest()
        {
            Assert.That(MockEnum.EntryWithStringValue.GetStringValue(), Is.EqualTo("Test"));
            Assert.That(MockEnum.EntryWithSecondStringValue.GetStringValue(), Is.EqualTo("3.14159265358979323846"));
            Assert.Throws<ArgumentException>(() => MockEnum.EntryWithoutStringValue.GetStringValue());
            Assert.Throws<ArgumentNullException>(() => ((MockEnum)123456).GetStringValue());
        }

        /// <summary>
        /// Tests the "ConvertToString" method.
        /// </summary>
        [Test]
        public void ConvertToStringTest()
        {
            Assert.AreEqual("FooBar", Google.Apis.Util.Utilities.ConvertToString("FooBar"));
            Assert.AreEqual("123", Google.Apis.Util.Utilities.ConvertToString(123));

            // check Enums work with and without StringValueAttribute
            Assert.AreEqual("Test", Google.Apis.Util.Utilities.ConvertToString(MockEnum.EntryWithStringValue));
            Assert.AreEqual("3.14159265358979323846",
                Google.Apis.Util.Utilities.ConvertToString(MockEnum.EntryWithSecondStringValue));
            Assert.AreEqual("EntryWithoutStringValue",
                Google.Apis.Util.Utilities.ConvertToString(MockEnum.EntryWithoutStringValue));
            Assert.AreEqual(null, Google.Apis.Util.Utilities.ConvertToString(null));

            // Test nullable types.
            int? nullable = 123;
            Assert.AreEqual("123", Google.Apis.Util.Utilities.ConvertToString(nullable));
            nullable = null;
            Assert.AreEqual(null, Google.Apis.Util.Utilities.ConvertToString(nullable));
            MockEnum? nullEnum = null;
            Assert.AreEqual(null, Google.Apis.Util.Utilities.ConvertToString(nullEnum));
        }
    }
}