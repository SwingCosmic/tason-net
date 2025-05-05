namespace TASON.Util;

/// <summary>
/// 提供关于集合相关的辅助方法
/// </summary>
public static class EnumerableExtensions 
{
    /// <summary>
    /// 将<typeparamref name="T"/>类型集合中所有可以转换为<typeparamref name="U"/>类型的元素返回，若类型不符合则跳过
    /// </summary>
    /// <typeparam name="T">源类型</typeparam>
    /// <typeparam name="U">目标类型</typeparam>
    /// <param name="list">源集合</param>
    /// <returns><see cref="IEnumerable{U}"/></returns>
    public static IEnumerable<U> CastIf<T, U>(this IEnumerable<T> list) 
    {
        foreach (var item in list)
        {
            if (item is U u)
            {
                yield return u;
            }
        }
    }
}