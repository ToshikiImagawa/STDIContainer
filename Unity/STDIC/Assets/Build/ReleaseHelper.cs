// Copyright (c) 2022 COMCREATE. All rights reserved.

using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace STDICBuild.Build
{
    public static class ReleaseHelper
    {
        [MenuItem("STDIC/Release")]
        public static async void Release()
        {
            var request = Client.Pack("Assets/STDIC", "../output");
            while (!request.IsCompleted)
            {
                await Task.Delay(1000);
            }

            if (request.Status == StatusCode.Success)
            {
                Debug.Log(request.Result);
            }
            else
            {
                Debug.LogError($"message:{request.Error.message}, errorCode:{request.Error.errorCode}");
            }
        }
    }
}