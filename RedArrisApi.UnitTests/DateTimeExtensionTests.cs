using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RedArrisApi.UnitTests;

using static DateTimeExtensions;

[TestClass]
public class DateTimeExtensionTests
{
    [DataTestMethod]
    [DataRow("2022-01-01")]
    [DataRow("01-01-2022")]
    [DataRow("1/1/22")]
    [DataRow("01/01/2022")]
    [DataRow("2022/01/01")]
    public void NormalizesValidSateString(string input)
    {
        input.NormalizeDateString()
            .Should()
            .Be("2022-01-01");
    }

    [DataTestMethod]
    [DataRow("foobar")]
    [DataRow(null)]
    [DataRow(" ")]
    [DataRow("")]
    public void ReturnsNullForInvalidDateString(string input)
    {
        input.NormalizeDateString()
            .Should().BeNull();
    }

    [TestMethod]
    public void GetsFirstDayOfTheYear()
    {
        GetFirstDayOfYearString()
            .Should()
            .Be($"{DateTime.Now.Year}-01-01");
    }
}