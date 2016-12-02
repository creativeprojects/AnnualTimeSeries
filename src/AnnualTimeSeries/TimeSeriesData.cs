
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AnnualTimeSeries
{
	/// <summary>
	/// Time Series Data of T where T is any object of interface <see cref="IAnnual">IAnnual</see>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class TimeSeriesData<T> : IEnumerable<T> where T : IAnnual
	{
		private readonly List<T> _data;

		public TimeSeriesData()
		{
			_data = new List<T>();
		}

		/// <summary>
		/// Constructor pre-populating time series data. This is mainly used by the JSON deserializer
		/// </summary>
		/// <param name="values"></param>
		public TimeSeriesData(IEnumerable<T> values)
		{
			if (values == null)
			{
				_data = new List<T>();
				return;
			}

			values = PrepareDataFromJson(values);
			_data = new List<T>(values);
			ValidateData();
			SortData();
		}

		private IEnumerable<T> PrepareDataFromJson(IEnumerable<T> values)
		{
			if (values.Count() <= 1) return values;

			var keys = values.GroupBy(v => v.Year).Count();
			if (keys > 1) return values;

			// That's where it's getting hairy: The ACE page doesn't populate the "Year" field, hence we have to reindex the years here
			var yearValues = new List<T>();
			int year = 0;
			foreach (var value in values)
			{
				yearValues.Add((T)value.ChangeYear(year));
				year++;
			}
			return yearValues;
		}

		private void ValidateData()
		{
			var duplicates = _data.GroupBy(v => v.Year).Where(g => g.Count() > 1).Select(g => g.Key.ToString()).ToList();
			if (duplicates.Count > 0)
			{
				throw new InvalidOperationException("The Time Series Data contain duplicated years: " + string.Join(", ", duplicates));
			}
		}

		private void SortData()
		{
			_data.Sort(
				(x, y) =>
				{
					if (x.Year < y.Year)
						return -1;

					if (x.Year == y.Year)
						return 0;

					//if (Year > other.Year)
					return 1;
				}
			);
		}

		private void AddData(T value)
		{
			var index = _data.FindIndex(t => t.Year == value.Year);
			if (index >= 0)
			{
				_data[index] = value;
			}
			else
			{
				_data.Add(value);
			}
		}

		public bool ContainsYear(int year)
		{
			return _data.Exists(t => t.Year == year);
		}

		public T GetYear(int year)
		{
			return _data.Find(t => t.Year == year);
		}

		/// <summary>
		/// Add OR replace the element for the year. A new value for the year will replace the previous value.
		/// </summary>
		/// <param name="value"></param>
		public void Add(T value)
		{
			AddData(value);
			SortData();
		}

		public void AddRange(IEnumerable<T> values)
		{
			foreach (var value in values)
			{
				AddData(value);
			}
			SortData();
		}

		/// <summary>
		/// Returns a list of years that are missing from the time series data. Please note that years where the values are null or zero are not considered missing.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<int> GetYearsGap()
		{
			var gaps = new List<int>();
			var startYear = StartYear();
			var endYear = EndYear();

			for (int year = startYear; year <= endYear; year++)
			{
				if (!ContainsYear(year))
				{
					gaps.Add(year);
				}
			}
			return gaps;
		}

		public void FillInGapsWith(Func<int, T> value)
		{
			if (IsEmpty()) return;

			var startYear = StartYear();
			var endYear = EndYear();

			for (int year = startYear; year <= endYear; year++)
			{
				if (!ContainsYear(year))
				{
					_data.Add(value(year));
				}
			}
			SortData();
		}

		public IEnumerable<int> GetYears()
		{
			return _data.Select(v => v.Year);
		}

		public bool YearHasNonNullValue(int year)
		{
			return ContainsYear(year) && GetYear(year).HasNonNullValue();
		}

		public bool YearHasNonZeroValue(int year)
		{
			return ContainsYear(year) && GetYear(year).HasNonZeroValue();
		}

		/// <summary>
		/// Returns True if AT LEAST ONE value is not null
		/// </summary>
		/// <returns>bool</returns>
		public bool HasNonNullValues()
		{
			return _data.Any(a => a.HasNonNullValue());
		}

		/// <summary>
		/// Returns True if AT LEAST ONE value is not zero (nor null)
		/// </summary>
		/// <returns>bool</returns>
		public bool HasNonZeroValues()
		{
			return _data.Any(a => a.HasNonZeroValue());
		}

		public bool IsEmpty()
		{
			return _data.Count == 0;
		}

		public bool IsNotEmpty()
		{
			return _data.Count > 0;
		}

		/// <summary>
		/// Returns the lowest year in the time series data. Please note it will still return a year even if its value is null (or zero)
		/// </summary>
		/// <returns>Lowest year</returns>
		public int StartYear()
		{
			return _data.Min(v => v.Year);
		}

		/// <summary>
		/// Returns the highest year in the time series data. Please note it will still return a year even if its value is null (or zero)
		/// </summary>
		/// <returns>Highest year</returns>
		public int EndYear()
		{
			return _data.Max(v => v.Year);
		}

		public int StartYearOfNonZeroValue()
		{
			return _data.Where(
					yearData =>
						yearData.HasNonZeroValue()
					).Min(el => el.Year);
		}

		public int EndYearOfNonZeroValue()
		{
			return _data.Where(
					yearData =>
						yearData.HasNonZeroValue())
					.Max(el => el.Year);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _data.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)_data).GetEnumerator();
		}

		public int Count => _data.Count;

		public bool Exists(Predicate<T> predicate)
		{
			return _data.Exists(predicate);
		}

		public bool Contains(T value)
		{
			return _data.Contains(value);
		}

		public void Clear()
		{
			_data.Clear();
		}

		private void TrimNulls(IEnumerable<T> data)
		{
			foreach (var item in data)
			{
				if (item.HasNonNullValue())
				{
					break;
				}
				_data.Remove(item);
			}
		}

		public void TrimNullDataFromTheEnd()
		{
			if (_data.Count == 0) return;
			TrimNulls(_data.OrderByDescending(v => v.Year));
		}

		public void TrimNullDataFromTheStart()
		{
			if (_data.Count == 0) return;
			TrimNulls(_data.OrderBy(v => v.Year));
		}

		public void TrimNullData()
		{
			TrimNullDataFromTheStart();
			TrimNullDataFromTheEnd();
		}

	}
}
