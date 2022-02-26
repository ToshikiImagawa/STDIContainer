// Copyright (c) 2022 COMCREATE. All rights reserved.

using JetBrains.Annotations;

namespace STDICEditor.Utils
{
    internal static class ScopeFunctions
    {
        public delegate void AlsoCall<in T>([NotNull] T it);

        [NotNull]
        public static T Also<T>([NotNull] this T self, AlsoCall<T> block)
        {
            block(self);
            return self;
        }

        [CanBeNull]
        public delegate TResult LetSelector<in T, out TResult>([NotNull] T it);

        [CanBeNull]
        public static TResult Let<T, TResult>([NotNull] this T self, LetSelector<T, TResult> selector)
        {
            return selector(self);
        }
    }
}