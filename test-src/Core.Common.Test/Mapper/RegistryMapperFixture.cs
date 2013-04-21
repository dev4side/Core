using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Mapper;
using Core.Common.Mapper.ConversionMethod;
using Core.Common.Mappers;
using NUnit.Framework;

namespace Core.Common.Test.Mapper
{
    [TestFixture]
    public class RegistryMapperFixture
    {
        /* 
         * This class tests the behavius of RegistryMapper
         */

        [MapToRegistryKey]
        internal class TestDto
        {
            [MapToRegistryKeyProperty(@"HKEY_LOCAL_MACHINE\SOFTWARE\Dev4Side", "Prop1")]
            public string Prop1 { get; set; }

            [MapToRegistryKeyProperty(@"HKEY_LOCAL_MACHINE\SOFTWARE\Dev4Side\SubKey", "Prop2")]
            public string Prop2 { get; set; }
        }

        [SetUp]
        public void SetUp()
        {
            InstallRegistry();
        }


        [Test]
        public void CanGetObject()
        {
            var sutResult = RegistryMapper<TestDto>.Get();
            Assert.IsNotNull(sutResult);
            Assert.IsTrue(sutResult.Prop1 == "Test1");
            Assert.IsTrue(sutResult.Prop2 == "Test2");
        }

        [Test]
        public void CanSetObject()
        {
            var testDto = new TestDto()
                {
                    Prop1 = "TestA",
                    Prop2 = "TestB"
                };
            try
            {
                RegistryMapper<TestDto>.Set(testDto);
            }
            catch (UnauthorizedAccessException)
            {
               Assert.Inconclusive("Please run this test as an administrator as it requries to insert registry values.");
            }
           
            Assert.IsTrue((string) Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Dev4Side", "Prop1", null) == "TestA");
            Assert.IsTrue((string)Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Dev4Side\\SubKey", "Prop2", null) == "TestB");

        }

        private void InstallRegistry()
        {
            var regeditProcess = Process.Start("regedit.exe", "/s " + Environment.CurrentDirectory + "\\Files\\Registry\\Test.reg");
            regeditProcess.WaitForExit();
        }


    }
}
