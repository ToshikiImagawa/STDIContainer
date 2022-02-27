// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;

namespace SampleApp.Utils
{
    public static class ScopeFunctions
    {
        public static T Also<T>(this T self, Action<T> block)
        {
            block(self);
            return self;
        }
    }
}