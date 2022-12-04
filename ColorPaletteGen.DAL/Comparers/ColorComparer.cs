using ColorPaletteGen.DAL.Model;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ColorPaletteGen.DAL.Comparers;

public class ColorComparer : ValueComparer<IList<Color>>
{
    public ColorComparer() 
        : base(
            (list1, list2) => CompareColors(list1, list2),
            list => GetHash(list)) { }

    private static int GetHash(IList<Color> list)
    {
        throw new NotImplementedException();
    }

    private static bool CompareColors(IList<Color>? list1, IList<Color>? list2)
    {
        if (list1 is null || list2 is null)
        {
            return false;
        }
        
        return list1.Count == list2.Count && list1.Zip(list2).All(pair => pair.First.Equals(pair.Second));
    }
}
