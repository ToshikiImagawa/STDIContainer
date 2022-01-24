// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;
using SampleApp.Model;

namespace SampleApp.Utils
{
    public static class ToDoStatusExtensions
    {
        public static string ToJpString(this ToDoStatus self)
        {
            return self switch
            {
                ToDoStatus.Open => "開始前",
                ToDoStatus.InProgress => "進行中",
                ToDoStatus.Close => "終了",
                ToDoStatus.Stopped => "中断",
                _ => throw new ArgumentOutOfRangeException(nameof(self), self, null)
            };
        }
    }
}