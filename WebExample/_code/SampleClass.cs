using CodeFirstConfig;
using System;
// ReSharper disable CheckNamespace

namespace WebExample
{
    public class ComplexClass
    {
        public string Field1 { get; set; } = "Field1";
        public string Field2 { get; set; } = "Field2";
    }

    public enum TestEnum { Enum1, Enum2, Enum3 }
    [Flags] public enum Flags { Flag1 = 1, Flag2 = 2, Flag3 = 4 }

    public class SampleClass
    {
        public string Value1 { get; set; } = "MyValueFromCode1";
        public string Value2 { get; set; } = "MyValueFromCode2";
        public string Value3 { get; set; } = null;
        
        public TestEnum EnumValue1 { get; set; } = TestEnum.Enum1;
        public TestEnum EnumValue2 { get; set; } = TestEnum.Enum2;
        public TestEnum? EnumValue3 { get; set; } = null;
        public TestEnum? EnumValue4 { get; set; } = TestEnum.Enum3;

        public Flags FlagsValue1 { get; set; } = Flags.Flag1;
        public Flags FlagsValue2 { get; set; } = Flags.Flag1 | Flags.Flag2;
        public Flags? FlagsValue3 { get; set; } = null;
        public Flags? FlagsValue4 { get; set; } = Flags.Flag1 | Flags.Flag2 | Flags.Flag3;

        public int IntValue1 { get; set; } = 1;
        public int IntValue2 { get; set; } = 2;
        public int? IntValue3 { get; set; } = null;
        public int? IntValue4 { get; set; } = 4;

        public double DoubleValue1 { get; set; } = 1.23;
        public double DoubleValue2 { get; set; } = 2.34;
        public double? DoubleValue3 { get; set; } = null;
        public double? DoubleValue4 { get; set; } = 4.56;

        public decimal DecimalValue1 { get; set; } = 1.23M;
        public decimal DecimalValue2 { get; set; } = 2.34M;
        public decimal? DecimalValue3 { get; set; } = null;
        public decimal? DecimalValue4 { get; set; } = 4.56M;

        public bool BoolValue1 { get; set; } = true;
        public bool BoolValue2 { get; set; } = false;
        public bool? BoolValue3 { get; set; } = null;
        public bool? BoolValue4 { get; set; } = true;

        //public DateTime DateTimeValue1 { get; set; } = new DateTime(2015, 1, 15);
        //public DateTime DateTimeValue2 { get; set; } = new DateTime(2015, 5, 30);

        public ComplexClass ComplexClass1 { get; set; }
        public ComplexClass ComplexClass2 { get; set; } = new ComplexClass();
        public ComplexClass ComplexClass3 { get; set; } = new ComplexClass { Field1 = "ComplexClass3_1", Field2 = "ComplexClass3_2" };

        public string[] StringArray1 { get; set; }
        public string[] StringArray2 { get; set; } = { "value1", "value2" };
        public string[] StringArray3 { get; set; } = { "array3_value1", "array3_value2", "array3_value3" };

        public int[] IntArray1 { get; set; }
        public int[] IntArray2 { get; set; } = { 1, 2 };
        public int[] IntArray3 { get; set; } = { 1, 2, 3 };
    }

    public class SampleClassConfigManager : ConfigManager<SampleClass>
    {        
    }
}
