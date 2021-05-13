namespace GeoFinder.Infrastructure.Abstractions
{
    /// <summary>
    /// Similar to IComparer but with ref parameters
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRefComparer<T>
    {
        int Compare(ref T x, ref T y);
    }
}
