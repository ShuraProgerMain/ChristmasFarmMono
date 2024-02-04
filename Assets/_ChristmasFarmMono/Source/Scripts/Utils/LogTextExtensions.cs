using UnityEngine;

namespace _ChristmasFarmMono.Source.Scripts.Utils
{
    public static class LogTextExtensions
    {
        public static string Bold(this string str) => "<b>" + str + "</b>";
        
        public static string Color(this string str, Color color) 
            => $"<color=#{(byte)(color.r * 255f):X2}{(byte)(color.g * 255f):X2}{(byte)(color.b * 255f):X2}>{str}</color>";
        
        public static string Italic(this string str) => "<i>" + str + "</i>";
        
        public static string Size(this string str, int size) => $"<size={size}>{str}</size>";
    }
}