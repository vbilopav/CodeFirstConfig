using System;
using CodeFirstConfig;
// ReSharper disable CheckNamespace

namespace WebExample
{
    public class ComplexClass
    {
        public string Field1 { get; set; } = "Field1";
        public string Field2 { get; set; } = "Field2";
    }

    public enum TestEnum
    {
        Enum1,
        Enum2,
        Enum3
    }

    public class SampleClass
    {
        public string Value1 { get; set; } = "MyValueFromCode1";
        public string Value2 { get; set; } = "MyValueFromCode2";
        public string Value3 { get; set; } = "MyValueFromCode3";
        public string Value4 { get; set; } = "MyValueFromCode4";
        public string Value5 { get; set; } = "MyValueFromCode5";
        public string Value6 { get; set; } = "MyValueFromCode6";
        public string Value7 { get; set; } = "MyValueFromCode7";
        public TestEnum EnumValue1 { get; set; } = TestEnum.Enum1;
        public TestEnum EnumValue2 { get; set; } = TestEnum.Enum2;
        //public DateTime DateTimeValue1 { get; set; } = new DateTime(2015, 1, 15);
        //public DateTime DateTimeValue2 { get; set; } = new DateTime(2015, 5, 30);
        public bool BoolValue1 { get; set; } = true;
        public bool BoolValue2 { get; set; } = true;
        //public ComplexClass ComplexClass1 { get; set; }
        public ComplexClass ComplexClass2 { get; set; } = new ComplexClass();
        //public string[] StringArray1 { get; set; }
        public string[] StringArray2 { get; set; } = { "value1", "value2" };
    }

    public class SampleClassConfigManager : ConfigManager<SampleClass>
    {        
    }
}
