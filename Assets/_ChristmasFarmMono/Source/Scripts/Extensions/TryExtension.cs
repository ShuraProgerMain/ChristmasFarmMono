using _ChristmasFarmMono.Source.Scripts.GardenBed;

namespace _ChristmasFarmMono.Source.Scripts.Extensions
{
    public static class TryExtension
    {
        public static T TrySelect<T>(this T origin)
        {
            if (origin is ISelectable selectable)
            {
                selectable.Select();
            }
            return origin;
        }
        
        public static T TryDropSelect<T>(this T origin)
        {
            if (origin is ISelectable selectable)
            {
                selectable.DropSelect();
            }
            return origin;
        }
    }
}