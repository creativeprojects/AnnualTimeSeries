using System;
using AnnualTimeSeries;

namespace AnnualTimeSeriesTests
{
	/// <summary>
	/// Decimal value per year used for unit test of TimeSeriesData
	/// </summary>
	internal class AnnualDecimal : IAnnual, IEquatable<AnnualDecimal>
	{
		public int Year { get; }
		public decimal? Value { get; }

		public AnnualDecimal(int year, decimal? value)
		{
			Year = year;
			Value = value;
		}

		public bool HasNonNullValue()
		{
			return Value.HasValue;
		}

		public bool HasNonZeroValue()
		{
			return Value.HasValue && Value != 0;
		}

		public IAnnual ChangeYear(int year)
		{
			return new AnnualDecimal(year, Value);
		}

		public bool Equals(AnnualDecimal other)
		{
			return Year == other.Year && Value == other.Value;
		}

		public override string ToString()
		{
			return string.Format("Year: {0}, Value: {1}", Year, Value);
		}
	}
}