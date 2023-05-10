using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine.UI;

namespace Common.Utility
{
    public static class NumberUtils
    {
        private const string IntegerFormat = "#,##0";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetFormattedInteger(long origin)
        {
            return origin.ToString(IntegerFormat);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetFormattedInteger(int origin)
        {
            return origin.ToString(IntegerFormat);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetFormattedInteger(float origin)
        {
            return GetFormattedInteger(Convert.ToInt64(origin));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetFormattedInteger(double origin)
        {
            return GetFormattedInteger(Convert.ToInt64(origin));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetFormattedInteger(this Text textComp, long origin)
        {
            textComp.text = GetFormattedInteger(origin);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetFormattedInteger(this Text textComp, int origin)
        {
            textComp.text = GetFormattedInteger(origin);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetFormattedInteger(this Text textComp, float origin)
        {
            textComp.text = GetFormattedInteger(origin);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetFormattedInteger(this Text textComp, double origin)
        {
            textComp.text = GetFormattedInteger(origin);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetNumber(this Text textComp, long number)
        {
            textComp.text = number.ToString();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetFormattedInteger(this TextMeshProUGUI textComp, long origin)
        {
            textComp.text = GetFormattedInteger(origin);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetFormattedInteger(this TextMeshProUGUI textComp, int origin)
        {
            textComp.text = GetFormattedInteger(origin);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetFormattedInteger(this TextMeshProUGUI textComp, float origin)
        {
            textComp.text = GetFormattedInteger(origin);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetFormattedInteger(this TextMeshProUGUI textComp, double origin)
        {
            textComp.text = GetFormattedInteger(origin);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetNumber(this TextMeshProUGUI textComp, long number)
        {
            textComp.text = number.ToString();
        }
    }
}