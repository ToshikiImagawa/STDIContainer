// Copyright (c) 2022 COMCREATE. All rights reserved.

using STDICEditor.Node;
using UnityEngine.UIElements;

namespace STDICEditor.Utils
{
    internal static class StyleExtensions
    {
        public static void Flex(this IStyle self, StyleFloat value)
        {
            self.flexGrow = value;
            self.flexShrink = value;
        }

        public static void Margin(this IStyle self, StyleLength value)
        {
            self.marginBottom = value;
            self.marginTop = value;
            self.marginRight = value;
            self.marginLeft = value;
        }

        public static void PaddingH(this IStyle self, StyleLength value)
        {
            self.paddingRight = value;
            self.paddingLeft = value;
        }

        public static void BorderColor(this IStyle self, StyleColor color)
        {
            self.borderBottomColor = color;
            self.borderTopColor = color;
            self.borderLeftColor = color;
            self.borderRightColor = color;
        }

        public static void BorderWidth(this IStyle self, StyleFloat value)
        {
            self.borderBottomWidth = value;
            self.borderTopWidth = value;
            self.borderLeftWidth = value;
            self.borderRightWidth = value;
        }

        public static void BorderRadius(this IStyle self, StyleLength value)
        {
            self.borderTopRightRadius = value;
            self.borderBottomRightRadius = value;
            self.borderBottomLeftRadius = value;
            self.borderTopLeftRadius = value;
        }

        public static Label TitleLabel(this GraphNode self)
        {
            return self.titleContainer.Q<Label>();
        }
    }
}