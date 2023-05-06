// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;
using SampleApp.Model;
using STDIC;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace SampleApp.Infrastructure.Helper
{
    public class LocalDataBaseHelper
    {
        private const string SAVE_KEY = "ToDo_Save";

        [Inject]
        public LocalDataBaseHelper()
        {
        }

        public bool Save(ToDo[] todoList)
        {
            var oldJsonString = PlayerPrefs.GetString(SAVE_KEY, string.Empty);
            try
            {
                if (todoList != null && todoList.Length != 0)
                {
                    var jsonString = JsonConvert.SerializeObject(todoList);
                    PlayerPrefs.SetString(SAVE_KEY, jsonString);
                }
                else
                {
                    PlayerPrefs.SetString(SAVE_KEY, string.Empty);
                }
            }
            catch
            {
                PlayerPrefs.SetString(SAVE_KEY, oldJsonString);
                return false;
            }

            PlayerPrefs.Save();
            return true;
        }

        public ToDo[] Load()
        {
            var jsonString = PlayerPrefs.GetString(SAVE_KEY, string.Empty);
            if (string.IsNullOrEmpty(jsonString))
            {
                return Array.Empty<ToDo>();
            }

            try
            {
                return JsonConvert.DeserializeObject<ToDo[]>(jsonString);
            }
            catch
            {
                PlayerPrefs.DeleteKey(SAVE_KEY);
                PlayerPrefs.Save();
                return Array.Empty<ToDo>();
            }
        }
    }
}