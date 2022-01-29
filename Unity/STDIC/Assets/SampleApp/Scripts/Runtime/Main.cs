// Copyright (c) 2022 COMCREATE. All rights reserved.

using SampleApp.Domain.Service;
using SampleApp.UI;
using SampleApp.ViewModel;
using UnityEngine;

namespace SampleApp
{
    public class Main : MonoBehaviour
    {
        // ReSharper disable once RedundantDefaultMemberInitializer
        [SerializeField] private ToDoListDisplay toDoListDisplay = default;

        public void Awake()
        {
            var service = ToDoContainer.Instance.Container.Resolve<IToDoService>();
            var listViewModel = new ToDoListViewModel(service);
            toDoListDisplay.Inject(listViewModel);
        }

        private void OnDestroy()
        {
            ToDoContainer.Instance.Container.Dispose();
        }
    }
}