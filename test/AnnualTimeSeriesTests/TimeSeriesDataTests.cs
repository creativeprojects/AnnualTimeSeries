
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using AnnualTimeSeries;
using Newtonsoft.Json;
using NUnit.Framework;

namespace AnnualTimeSeriesTests
{
	[TestFixture]
	class TimeSeriesDataTests
	{
		[Test]
		public void ShouldBeEmptyAfterInstanciation()
		{
			var timeSeriesData = new TimeSeriesData<AnnualDecimal>();
			Assert.That(timeSeriesData, Is.Not.Null);
			Assert.That(timeSeriesData.Count(), Is.EqualTo(0));
		}

		[Test]
		public void GettingItemFromAnEmptyArray()
		{
			var timeSeriesData = new TimeSeriesData<AnnualDecimal>();
			Assert.That(timeSeriesData.GetYear(10), Is.Null);
		}
		[Test]
		public void GettingAbsentValue()
		{
			var timeSeriesData = new TimeSeriesData<AnnualDecimal>();
			timeSeriesData.Add(new AnnualDecimal(5, 5));
			timeSeriesData.Add(new AnnualDecimal(20, 20));
			Assert.That(timeSeriesData.GetYear(10), Is.Null);
		}

		[Test]
		public void SettingAndGettingData()
		{
			var timeSeriesData = new TimeSeriesData<AnnualDecimal>();
			timeSeriesData.Add(new AnnualDecimal(10, 100));
			Assert.That(timeSeriesData.Count, Is.EqualTo(1));
			Assert.That(timeSeriesData.GetYear(10).Value, Is.EqualTo(100));
		}

		[Test]
		public void RewritingTheSameYearOnAnSingleElementList()
		{
			var timeSeriesData = new TimeSeriesData<AnnualDecimal>();
			timeSeriesData.Add(new AnnualDecimal(10, 100));
			timeSeriesData.Add(new AnnualDecimal(10, 200));
			Assert.That(timeSeriesData.Count, Is.EqualTo(1));
			Assert.That(timeSeriesData.GetYear(10).Value, Is.EqualTo(200));
		}

		[Test]
		public void RewritingTheSameYearOnPopulatedList()
		{
			var timeSeriesData = new TimeSeriesData<AnnualDecimal>();
			timeSeriesData.Add(new AnnualDecimal(1, 100));
			timeSeriesData.Add(new AnnualDecimal(2, 100));
			timeSeriesData.Add(new AnnualDecimal(10, 100));
			timeSeriesData.Add(new AnnualDecimal(10, 200));
			Assert.That(timeSeriesData.Count, Is.EqualTo(3));
			Assert.That(timeSeriesData.GetYear(10).Value, Is.EqualTo(200));
		}
		[Test]
		public void CanSerializeAndDeserializeEmptyClass()
		{
			var tsd = new TimeSeriesData<AnnualDecimal>();
			var json = JsonConvert.SerializeObject(tsd);
			Assert.That(json, Is.Not.Null);
			Debug.WriteLine(json);
			var deserialized = JsonConvert.DeserializeObject<TimeSeriesData<AnnualDecimal>>(json);
			Assert.That(deserialized, Is.Not.Null);
		}

		[Test]
		public void CanSerializeAndDeserialize()
		{
			var timeSeriesData = new TimeSeriesData<AnnualDecimal>();
			timeSeriesData.Add(new AnnualDecimal(10, 100));
			timeSeriesData.Add(new AnnualDecimal(20, 200));
			Assert.That(timeSeriesData.Count, Is.EqualTo(2));
			var json = JsonConvert.SerializeObject(timeSeriesData);
			Assert.That(json, Is.Not.Null);
			Debug.WriteLine(json);
			var deserialized = JsonConvert.DeserializeObject<TimeSeriesData<AnnualDecimal>>(json);
			Assert.That(deserialized, Is.Not.Null);
			Assert.That(deserialized.Count, Is.EqualTo(2));
		}

		[Test]
		public void CanSerializeAndDeserializeAnnualDecimal()
		{
			var value = new AnnualDecimal(1992, 100);
			var json = JsonConvert.SerializeObject(value);
			Assert.That(json, Is.Not.Null);
			Debug.WriteLine(json);
			var deserialized = JsonConvert.DeserializeObject<AnnualDecimal>(json);
			Assert.That(deserialized, Is.Not.Null);
			Assert.That(deserialized.Value, Is.EqualTo(100));
			Assert.That(deserialized.Year, Is.EqualTo(1992));
		}

		[Test]
		public void CanSerializeAndDeserializeTimeSeriesDataWithSomeZeroAndNullValues()
		{
			var timeSeriesData = new TimeSeriesData<AnnualDecimal>();
			timeSeriesData.Add(new AnnualDecimal(10, 100));
			timeSeriesData.Add(new AnnualDecimal(20, 0));
			timeSeriesData.Add(new AnnualDecimal(30, null));
			var json = JsonConvert.SerializeObject(timeSeriesData);
			Assert.That(json, Is.Not.Null);
			Assert.That(json, Is.EqualTo("[{\"Year\":10,\"Value\":100.0},{\"Year\":20,\"Value\":0.0},{\"Year\":30,\"Value\":null}]"));
			var deserialized = JsonConvert.DeserializeObject<TimeSeriesData<AnnualDecimal>>(json);
			Assert.That(deserialized, Is.Not.Null);
			Assert.That(deserialized.Count, Is.EqualTo(3));
			Assert.That(timeSeriesData, Is.EqualTo(deserialized));
		}

		[Test]
		public void HasNonZeroValues()
		{
			var timeSeriesData = new TimeSeriesData<AnnualDecimal>();
			timeSeriesData.Add(new AnnualDecimal(10, 0));
			timeSeriesData.Add(new AnnualDecimal(20, 200));
			Assert.That(timeSeriesData.HasNonNullValues(), Is.True);
			Assert.That(timeSeriesData.HasNonZeroValues(), Is.True);
		}

		[Test]
		public void HasNonNullValues()
		{
			var timeSeriesData = new TimeSeriesData<AnnualDecimal>();
			timeSeriesData.Add(new AnnualDecimal(10, 0));
			timeSeriesData.Add(new AnnualDecimal(20, null));
			Assert.That(timeSeriesData.HasNonNullValues(), Is.True);
			Assert.That(timeSeriesData.HasNonZeroValues(), Is.False);
		}

		[Test]
		public void HasNullValues()
		{
			var timeSeriesData = new TimeSeriesData<AnnualDecimal>();
			timeSeriesData.Add(new AnnualDecimal(10, null));
			timeSeriesData.Add(new AnnualDecimal(20, null));
			Assert.That(timeSeriesData.HasNonNullValues(), Is.False);
			Assert.That(timeSeriesData.HasNonZeroValues(), Is.False);
			Assert.That(timeSeriesData.StartYear(), Is.EqualTo(10));
			Assert.That(timeSeriesData.EndYear(), Is.EqualTo(20));
		}

		[Test]
		public void HasZeroValues()
		{
			var timeSeriesData = new TimeSeriesData<AnnualDecimal>();
			timeSeriesData.Add(new AnnualDecimal(10, 0));
			timeSeriesData.Add(new AnnualDecimal(20, 0));
			Assert.That(timeSeriesData.HasNonNullValues(), Is.True);
			Assert.That(timeSeriesData.HasNonZeroValues(), Is.False);
			Assert.That(timeSeriesData.StartYear(), Is.EqualTo(10));
			Assert.That(timeSeriesData.EndYear(), Is.EqualTo(20));
		}

		[Test]
		public void StartAndEndYearShouldFailWhenTimeSeriesEmpty()
		{
			var timeSeriesData = new TimeSeriesData<AnnualDecimal>();
			Assert.That(timeSeriesData.IsNotEmpty(), Is.False);
			Assert.Throws<InvalidOperationException>(() => timeSeriesData.StartYear());
			Assert.Throws<InvalidOperationException>(() => timeSeriesData.EndYear());
			Assert.Throws<InvalidOperationException>(() => timeSeriesData.StartYearOfNonZeroValue());
			Assert.Throws<InvalidOperationException>(() => timeSeriesData.EndYearOfNonZeroValue());
		}

		[Test]
		public void StartAndEndYearShouldFailWhenTimeSeriesIsAllZero()
		{
			var timeSeriesData = new TimeSeriesData<AnnualDecimal>();
			timeSeriesData.Add(new AnnualDecimal(10, 0));
			timeSeriesData.Add(new AnnualDecimal(20, 0));
			Assert.That(timeSeriesData.IsNotEmpty(), Is.True);
			Assert.That(timeSeriesData.StartYear(), Is.EqualTo(10));
			Assert.That(timeSeriesData.EndYear(), Is.EqualTo(20));
			Assert.Throws<InvalidOperationException>(() => timeSeriesData.StartYearOfNonZeroValue());
			Assert.Throws<InvalidOperationException>(() => timeSeriesData.EndYearOfNonZeroValue());
		}

		[Test]
		public void DataIsSortedAfterAddingOneByOne()
		{
			var timeSeriesData = new TimeSeriesData<AnnualDecimal>();
			timeSeriesData.Add(new AnnualDecimal(30, 3));
			timeSeriesData.Add(new AnnualDecimal(10, 1));
			timeSeriesData.Add(new AnnualDecimal(40, 4));
			timeSeriesData.Add(new AnnualDecimal(20, 2));

			var enumerator = timeSeriesData.GetEnumerator();
			enumerator.Reset();
			enumerator.MoveNext();
			Assert.That(enumerator.Current, Is.EqualTo(new AnnualDecimal(10, 1)));
			enumerator.MoveNext();
			Assert.That(enumerator.Current, Is.EqualTo(new AnnualDecimal(20, 2)));
			enumerator.MoveNext();
			Assert.That(enumerator.Current, Is.EqualTo(new AnnualDecimal(30, 3)));
			enumerator.MoveNext();
			Assert.That(enumerator.Current, Is.EqualTo(new AnnualDecimal(40, 4)));
		}

		[Test]
		public void DataIsSortedAfterAddingRange()
		{
			var timeSeriesData = new TimeSeriesData<AnnualDecimal>();
			timeSeriesData.AddRange(new[]
			{
				new AnnualDecimal(30, 3),
				new AnnualDecimal(10, 1),
				new AnnualDecimal(40, 4),
				new AnnualDecimal(20, 2)
			});

			var enumerator = timeSeriesData.GetEnumerator();
			enumerator.Reset();
			enumerator.MoveNext();
			Assert.That(enumerator.Current, Is.EqualTo(new AnnualDecimal(10, 1)));
			enumerator.MoveNext();
			Assert.That(enumerator.Current, Is.EqualTo(new AnnualDecimal(20, 2)));
			enumerator.MoveNext();
			Assert.That(enumerator.Current, Is.EqualTo(new AnnualDecimal(30, 3)));
			enumerator.MoveNext();
			Assert.That(enumerator.Current, Is.EqualTo(new AnnualDecimal(40, 4)));
		}

		[Test]
		public void DataIsSortedAfterDeserializing()
		{
			var json = "[{\"Year\":20,\"Value\":2},{\"Year\":10,\"Value\":1},{\"Year\":40,\"Value\":4},{\"Year\":30,\"Value\":3}]";
			var timeSeriesData = JsonConvert.DeserializeObject<TimeSeriesData<AnnualDecimal>>(json);

			var enumerator = timeSeriesData.GetEnumerator();
			enumerator.Reset();
			enumerator.MoveNext();
			Assert.That(enumerator.Current, Is.EqualTo(new AnnualDecimal(10, 1)));
			enumerator.MoveNext();
			Assert.That(enumerator.Current, Is.EqualTo(new AnnualDecimal(20, 2)));
			enumerator.MoveNext();
			Assert.That(enumerator.Current, Is.EqualTo(new AnnualDecimal(30, 3)));
			enumerator.MoveNext();
			Assert.That(enumerator.Current, Is.EqualTo(new AnnualDecimal(40, 4)));
		}

		[Test]
		public void DoesNotAllowDuplicatedYears()
		{
			var json = "[{\"Year\":20,\"Value\":2},{\"Year\":10,\"Value\":1},{\"Year\":30,\"Value\":4},{\"Year\":20,\"Value\":3}]";
			Assert.Throws<InvalidOperationException>(() => JsonConvert.DeserializeObject<TimeSeriesData<AnnualDecimal>>(json));
		}

		[Test]
		public void GapsAreFilledInAndListSorted()
		{
			var timeSeriesData = new TimeSeriesData<AnnualDecimal>();
			timeSeriesData.AddRange(new[]
			{
				new AnnualDecimal(10, 99),
				new AnnualDecimal(13, 99),
				new AnnualDecimal(15, 99)
			});
			timeSeriesData.FillInGapsWith(year => new AnnualDecimal(year, 55));

			var enumerator = timeSeriesData.GetEnumerator();
			enumerator.Reset();
			enumerator.MoveNext();
			Assert.That(enumerator.Current, Is.EqualTo(new AnnualDecimal(10, 99)));
			enumerator.MoveNext();
			Assert.That(enumerator.Current, Is.EqualTo(new AnnualDecimal(11, 55)));
			enumerator.MoveNext();
			Assert.That(enumerator.Current, Is.EqualTo(new AnnualDecimal(12, 55)));
			enumerator.MoveNext();
			Assert.That(enumerator.Current, Is.EqualTo(new AnnualDecimal(13, 99)));
			enumerator.MoveNext();
			Assert.That(enumerator.Current, Is.EqualTo(new AnnualDecimal(14, 55)));
			enumerator.MoveNext();
			Assert.That(enumerator.Current, Is.EqualTo(new AnnualDecimal(15, 99)));
		}

		[Test]
		public void ListWithNoGapWillRemainTheSame()
		{
			var timeSeriesData = new TimeSeriesData<AnnualDecimal>();
			timeSeriesData.AddRange(new[]
			{
				new AnnualDecimal(10, 10),
				new AnnualDecimal(11, 11),
				new AnnualDecimal(12, 12),
				new AnnualDecimal(13, 13),
				new AnnualDecimal(14, 14),
				new AnnualDecimal(15, 15)
			});
			timeSeriesData.FillInGapsWith(year => new AnnualDecimal(year, 55));

			var enumerator = timeSeriesData.GetEnumerator();
			enumerator.Reset();
			enumerator.MoveNext();
			Assert.That(enumerator.Current, Is.EqualTo(new AnnualDecimal(10, 10)));
			enumerator.MoveNext();
			Assert.That(enumerator.Current, Is.EqualTo(new AnnualDecimal(11, 11)));
			enumerator.MoveNext();
			Assert.That(enumerator.Current, Is.EqualTo(new AnnualDecimal(12, 12)));
			enumerator.MoveNext();
			Assert.That(enumerator.Current, Is.EqualTo(new AnnualDecimal(13, 13)));
			enumerator.MoveNext();
			Assert.That(enumerator.Current, Is.EqualTo(new AnnualDecimal(14, 14)));
			enumerator.MoveNext();
			Assert.That(enumerator.Current, Is.EqualTo(new AnnualDecimal(15, 15)));
		}

		[Test]
		public void EmptyListHasNoGap()
		{
			var timeSeriesData = new TimeSeriesData<AnnualDecimal>();
			timeSeriesData.FillInGapsWith(year => new AnnualDecimal(year, 55));
			Assert.That(timeSeriesData.Count, Is.EqualTo(0));
		}

		[Test]
		public void TrimmedNonNullDataWillReturnTheSameTimeSeries()
		{
			var timeSeriesData = new TimeSeriesData<AnnualDecimal>();
			timeSeriesData.AddRange(new[]
			{
				new AnnualDecimal(10, 10),
				new AnnualDecimal(11, 11),
				new AnnualDecimal(12, 12),
				new AnnualDecimal(13, 13),
				new AnnualDecimal(14, 14),
				new AnnualDecimal(15, 15)
			});
			timeSeriesData.TrimNullData();
			Assert.That(timeSeriesData.Count, Is.EqualTo(6));
			Assert.That(timeSeriesData.GetYear(timeSeriesData.StartYear()), Is.EqualTo(new AnnualDecimal(10, 10)));
			Assert.That(timeSeriesData.GetYear(timeSeriesData.EndYear()), Is.EqualTo(new AnnualDecimal(15, 15)));
		}

		[Test]
		public void ShouldOnlyTrimFromTheStart()
		{
			var timeSeriesData = new TimeSeriesData<AnnualDecimal>();
			timeSeriesData.AddRange(new[]
			{
				new AnnualDecimal(10, null),
				new AnnualDecimal(11, null),
				new AnnualDecimal(12, 12),
				new AnnualDecimal(13, 13),
				new AnnualDecimal(14, 14),
				new AnnualDecimal(15, 15)
			});
			timeSeriesData.TrimNullData();
			Assert.That(timeSeriesData.Count, Is.EqualTo(4));
			Assert.That(timeSeriesData.GetYear(timeSeriesData.StartYear()), Is.EqualTo(new AnnualDecimal(12, 12)));
			Assert.That(timeSeriesData.GetYear(timeSeriesData.EndYear()), Is.EqualTo(new AnnualDecimal(15, 15)));
		}

		[Test]
		public void ShouldOnlyTrimFromTheEnd()
		{
			var timeSeriesData = new TimeSeriesData<AnnualDecimal>();
			timeSeriesData.AddRange(new[]
			{
				new AnnualDecimal(10, 10),
				new AnnualDecimal(11, 11),
				new AnnualDecimal(12, 12),
				new AnnualDecimal(13, 13),
				new AnnualDecimal(14, null),
				new AnnualDecimal(15, null)
			});
			timeSeriesData.TrimNullData();
			Assert.That(timeSeriesData.Count, Is.EqualTo(4));
			Assert.That(timeSeriesData.GetYear(timeSeriesData.StartYear()), Is.EqualTo(new AnnualDecimal(10, 10)));
			Assert.That(timeSeriesData.GetYear(timeSeriesData.EndYear()), Is.EqualTo(new AnnualDecimal(13, 13)));
		}

		[Test]
		public void ShouldNotRemoveNullValuesFromTheMiddle()
		{
			var timeSeriesData = new TimeSeriesData<AnnualDecimal>();
			timeSeriesData.AddRange(new[]
			{
				new AnnualDecimal(10, 10),
				new AnnualDecimal(11, null),
				new AnnualDecimal(12, null),
				new AnnualDecimal(13, null),
				new AnnualDecimal(14, null),
				new AnnualDecimal(15, 15)
			});
			timeSeriesData.TrimNullData();
			Assert.That(timeSeriesData.Count, Is.EqualTo(6));
			Assert.That(timeSeriesData.GetYear(timeSeriesData.StartYear()), Is.EqualTo(new AnnualDecimal(10, 10)));
			Assert.That(timeSeriesData.GetYear(timeSeriesData.EndYear()), Is.EqualTo(new AnnualDecimal(15, 15)));
		}

		[Test]
		public void ShouldOnlyTrimTheEnd()
		{
			var timeSeriesData = new TimeSeriesData<AnnualDecimal>();
			timeSeriesData.AddRange(new[]
			{
				new AnnualDecimal(10, null),
				new AnnualDecimal(11, null),
				new AnnualDecimal(12, 12),
				new AnnualDecimal(13, 13),
				new AnnualDecimal(14, null),
				new AnnualDecimal(15, null)
			});
			timeSeriesData.TrimNullDataFromTheEnd();
			Assert.That(timeSeriesData.Count, Is.EqualTo(4));
			Assert.That(timeSeriesData.GetYear(timeSeriesData.StartYear()), Is.EqualTo(new AnnualDecimal(10, null)));
			Assert.That(timeSeriesData.GetYear(timeSeriesData.EndYear()), Is.EqualTo(new AnnualDecimal(13, 13)));
		}

		[Test]
		public void ShouldOnlyTrimTheStart()
		{
			var timeSeriesData = new TimeSeriesData<AnnualDecimal>();
			timeSeriesData.AddRange(new[]
			{
				new AnnualDecimal(10, null),
				new AnnualDecimal(11, null),
				new AnnualDecimal(12, 12),
				new AnnualDecimal(13, 13),
				new AnnualDecimal(14, null),
				new AnnualDecimal(15, null)
			});
			timeSeriesData.TrimNullDataFromTheStart();
			Assert.That(timeSeriesData.Count, Is.EqualTo(4));
			Assert.That(timeSeriesData.GetYear(timeSeriesData.StartYear()), Is.EqualTo(new AnnualDecimal(12, 12)));
			Assert.That(timeSeriesData.GetYear(timeSeriesData.EndYear()), Is.EqualTo(new AnnualDecimal(15, null)));
		}

		[Test]
		public void TrimmingAnEmptyTimeSeriesData()
		{
			var timeSeriesData = new TimeSeriesData<AnnualDecimal>();
			timeSeriesData.TrimNullData();
			Assert.That(timeSeriesData.Count, Is.EqualTo(0));
		}

		[Test]
		public void TrimmingAllNullValuesTimeSeriesDataFromTheStart()
		{
			var timeSeriesData = new TimeSeriesData<AnnualDecimal>();
			timeSeriesData.AddRange(new[]
			{
				new AnnualDecimal(10, null),
				new AnnualDecimal(11, null),
				new AnnualDecimal(12, null),
				new AnnualDecimal(13, null),
				new AnnualDecimal(14, null),
				new AnnualDecimal(15, null)
			});
			timeSeriesData.TrimNullDataFromTheStart();
			Assert.That(timeSeriesData.Count, Is.EqualTo(0));
		}

		[Test]
		public void TrimmingAllNullValuesTimeSeriesDataFromTheEnd()
		{
			var timeSeriesData = new TimeSeriesData<AnnualDecimal>();
			timeSeriesData.AddRange(new[]
			{
				new AnnualDecimal(10, null),
				new AnnualDecimal(11, null),
				new AnnualDecimal(12, null),
				new AnnualDecimal(13, null),
				new AnnualDecimal(14, null),
				new AnnualDecimal(15, null)
			});
			timeSeriesData.TrimNullDataFromTheEnd();
			Assert.That(timeSeriesData.Count, Is.EqualTo(0));
		}

		[Test]
		public void GettingNonExistentYearShouldReturnNull()
		{
			var timeSeriesData = new TimeSeriesData<AnnualDecimal>();
			timeSeriesData.AddRange(new[]
			{
				new AnnualDecimal(10, null),
				new AnnualDecimal(11, null),
				new AnnualDecimal(12, null),
			});
			Assert.That(timeSeriesData.GetYear(100), Is.Null);
			Assert.That(timeSeriesData.GetYear(10), Is.EqualTo(new AnnualDecimal(10, null)));
			Assert.That(timeSeriesData.GetYear(12), Is.EqualTo(new AnnualDecimal(12, null)));
		}

		[Test]
		public void CanCreateWithNonPopulatedYears()
		{
			var prepopulated = new List<AnnualDecimal>();
			prepopulated.Add(new AnnualDecimal(0, 1));
			prepopulated.Add(new AnnualDecimal(0, 2));
			prepopulated.Add(new AnnualDecimal(0, 3));
			var timeSeriesData = new TimeSeriesData<AnnualDecimal>(prepopulated);
			Assert.That(timeSeriesData.Count, Is.EqualTo(3));
			Assert.That(timeSeriesData.GetYear(0), Is.EqualTo(new AnnualDecimal(0, 1)));
			Assert.That(timeSeriesData.GetYear(1), Is.EqualTo(new AnnualDecimal(1, 2)));
			Assert.That(timeSeriesData.GetYear(2), Is.EqualTo(new AnnualDecimal(2, 3)));
		}
	}
}
