namespace Postulate.Base.Interfaces
{
	/// <summary>
	/// Use this on model classes to indicate what expression or property to 
	/// return in order to map an id value to meaningful text the user will know.
	/// Expression must resolve to string when queried
	/// </summary>
	public interface INameLookup
	{
		/// <summary>
		/// Column name or expression that returns a name meaningful to user for a given Id value
		/// </summary>
		string NameExpression { get; }
	}
}