
namespace AnnualTimeSeries
{
	public interface IAnnual
	{
		int Year { get; }

		bool HasNonNullValue();
		bool HasNonZeroValue();
		IAnnual ChangeYear(int year);
	}
}
