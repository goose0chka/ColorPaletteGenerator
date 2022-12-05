using ColorPaletteGen.DAL.Model;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ColorPaletteGen.DAL.Comparers;

public class ColorComparer : ValueComparer<IList<Color>>
{
    public ColorComparer() 
        : base(
            (list1, list2) => CompareColors(list1, list2),
            list => 0,
            list => GetSnapshot(list)) { }

    private static IList<Color> GetSnapshot(IList<Color> list)
        => list.Select(color => new Color(color)).ToList();

    private static bool CompareColors(IList<Color>? list1, IList<Color>? list2)
    {
        if (list1 is null || list2 is null)
        {
            return false;
        }
        
        return list1.Count == list2.Count && list1.Zip(list2).All(pair => pair.First.Equals(pair.Second));
    }
}
