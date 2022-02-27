// Copyright (c) 2022 COMCREATE. All rights reserved.

using UnityEngine;
using UnityEngine.UIElements;

namespace STDICEditor.Utils
{
    internal static class ColorUtil
    {
        public static Color GetHtmlStringColor(string colorCode)
        {
            return ColorUtility.TryParseHtmlString(colorCode, out var color) ? color : Color.white;
        }

        public static StyleColor GetHtmlStringStyleColor(string colorCode)
        {
            return new StyleColor(GetHtmlStringColor(colorCode));
        }
    }
}