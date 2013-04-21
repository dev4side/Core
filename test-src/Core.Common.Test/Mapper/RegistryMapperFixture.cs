using System;
using System.Diagnostics;
using Core.Common.Mapper;
using Core.Common.Mapper.Registry;
using Core.Common.Mappers.Registry;
using Microsoft.Win32;
using NUnit.Framework;

namespace Core.Common.Test.Mapper
{
    [TestFixture]
    public class RegistryMapperFixture
    {
        /* 
         * This class tests the behavius of RegistryMapper
         */

        [SetUp]
        public void SetUp()
        {
            InstallRegistry();
        }

        [MapToRegistryKey]
        internal class TestDto
        {
            [MapToRegistryKeyProperty(@"HKEY_LOCAL_MACHINE\SOFTWARE\Dev4Side", "Prop1")]
            public string Prop1 { get; set; }

            [MapToRegistryKeyProperty(@"HKEY_LOCAL_MACHINE\SOFTWARE\Dev4Side\SubKey", "Prop2")]
            public string Prop2 { get; set; }
        }


        [MapToRegistryKey]
        internal class TestDtoBoolConversions
        {
            [MapToRegistryKeyProperty(@"HKEY_LOCAL_MACHINE\SOFTWARE\Dev4Side", "BoolEnabledDisabled",
                RegistryConversion = RegistryConversion.BoolEnabledDisabled)]
            public bool BoolEnabledDisabled { get; set; }

            [MapToRegistryKeyProperty(@"HKEY_LOCAL_MACHINE\SOFTWARE\Dev4Side", "BitEnabledDisabled",
                RegistryConversion = RegistryConversion.BoolBit)]
            public bool BoolBit { get; set; }
        }


        [Test]
        //Ensures boolean conversions in get
        public void CanGetAndConvertBoolToEnabledDisabled()
        {
            TestDtoBoolConversions sutResult = RegistryMapper<TestDtoBoolConversions>.Get();
            Assert.IsNotNull(sutResult);
            Assert.IsTrue(!sutResult.BoolEnabledDisabled);
            Assert.IsTrue(sutResult.BoolBit);
        }


        [Test]
        //Ensures boolean conversions in set
        public void CanSetAndConvertBoolToEnabledDisabled()
        {
            var testDto = new TestDtoBoolConversions()
                {
                    BoolEnabledDisabled = false,
                    BoolBit = true
                };
            try
            {
                RegistryMapper<TestDtoBoolConversions>.Set(testDto);
            }
            catch (UnauthorizedAccessException)
            {
                Assert.Inconclusive("Please run this test as an administrator as it requries to insert registry values.");
            }

            Assert.IsTrue((string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Dev4Side", "BoolEnabledDisabled", null) == "disabled");
            Assert.IsTrue((string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Dev4Side", "BitEnabledDisabled", null) == "1");

        }

        [Test]
        //Ensures mapping from registry values to dto
        public void CanGetObject()
        {
            TestDto sutResult = RegistryMapper<TestDto>.Get();
            Assert.IsNotNull(sutResult);
            Assert.IsTrue(sutResult.Prop1 == "Test1");
            Assert.IsTrue(sutResult.Prop2 == "Test2");
        }

        [Test]
        //Ensures mapping from dto values to registry
        public void CanSetObject()
        {
            var testDto = new TestDto
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

            Assert.IsTrue((string) Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Dev4Side", "Prop1", null) == "TestA");
            Assert.IsTrue((string) Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Dev4Side\\SubKey", "Prop2", null) == "TestB");
        }


        private void InstallRegistry()
        {
            Process regeditProcess = Process.Start("regedit.exe",
                                                   "/s " + Environment.CurrentDirectory + "\\Files\\Registry\\Test.reg");
            regeditProcess.WaitForExit();
        }
    }
}