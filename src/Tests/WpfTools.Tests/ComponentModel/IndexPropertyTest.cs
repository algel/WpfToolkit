using System.Collections.Generic;
using Algel.WpfTools.ComponentModel;
using Xunit;

namespace Algel.WpfTools.Tests.ComponentModel
{
    public class IndexPropertyTest
    {
        private readonly Dictionary<int, string> _dictionary;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public IndexPropertyTest()
        {
            _dictionary = new Dictionary<int, string>();

            OneDimensionWritableProperty = new IndexProperty<string, int>(GetOneDimensionValue, SetOneDimensionValue);
        }

        private void SetOneDimensionValue(int i, string v)
        {
            _dictionary[i] = v;
        }

        private string GetOneDimensionValue(int i)
        {
            return _dictionary[i];
        }

        private IndexProperty<string, int> OneDimensionWritableProperty { get; }

        [Theory]
        [MemberData(nameof(OneDimensionPropertyData))]
        public void SetValueByOneDimension(int index, string value)
        {
            Assert.PropertyChanged(OneDimensionWritableProperty, "Item[]", () => OneDimensionWritableProperty[index] = value);
        }

        [Theory]
        [MemberData(nameof(OneDimensionPropertyData))]
        public void GetValueFromOneDimensionWritableProperty(int index, string value)
        {
            SetOneDimensionValue(index, value);

            Assert.Equal(OneDimensionWritableProperty[index], value);
        }

        public static IEnumerable<object[]> OneDimensionPropertyData()
        {
            yield return new object[] { 1, "1111111" };
            yield return new object[] { 3, "33333" };
        }
    }
}
