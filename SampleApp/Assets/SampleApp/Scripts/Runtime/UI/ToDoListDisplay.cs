// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using SampleApp.Utils;
using SampleApp.ViewModel;
using UnityEngine;
using UnityEngine.UI;

namespace SampleApp.UI
{
    public class ToDoListDisplay : MonoBehaviour
    {
        // ReSharper disable once RedundantDefaultMemberInitializer
        [SerializeField] private ToDoDisplay prefab = default;

        // ReSharper disable once RedundantDefaultMemberInitializer
        [SerializeField] private Transform parentTransform = default;

        // ReSharper disable once RedundantDefaultMemberInitializer
        [SerializeField] private Button createButton = default;

        // ReSharper disable once RedundantDefaultMemberInitializer
        [SerializeField] private InputField todoInputField = default;
        private ToDoListViewModel _viewModel;
        private IDisposable _disposable;
        private readonly Dictionary<int, ToDoDisplay> _displays = new Dictionary<int, ToDoDisplay>();
        private readonly Queue<ToDoDisplay> _objectPool = new Queue<ToDoDisplay>();

        public void Inject(ToDoListViewModel viewModel)
        {
            _viewModel = viewModel;
            Init();
        }

        private async void Init()
        {
            var list = await _viewModel.Current;
            foreach (var viewModel in list)
            {
                _displays[viewModel.Id] = Spawn(viewModel);
            }

            _disposable = new CompositeDisposable()
                .Add(_viewModel.OnAdd.Subscribe(OnAdd))
                .Add(_viewModel.OnRemove.Subscribe(OnRemove))
                .Add(createButton.onClick.Subscribe(OnCreate));
        }

        private async void OnCreate()
        {
            var message = todoInputField.text;
            if (string.IsNullOrEmpty(message)) return;
            var todo = await _viewModel.Create(message);
            Debug.Log($"Create ToDo:{todo.Id}");
        }

        private void OnAdd(ToDoViewModel viewModel)
        {
            if (_displays.ContainsKey(viewModel.Id)) return;
            _displays[viewModel.Id] = Spawn(viewModel);
        }

        private void OnRemove(ToDoViewModel viewModel)
        {
            if (!_displays.ContainsKey(viewModel.Id)) return;
            Despawn(_displays[viewModel.Id]);
            _displays.Remove(viewModel.Id);
        }

        private ToDoDisplay CreateToDoDisplay()
        {
            var display = Instantiate(prefab, parentTransform, false);
            return display;
        }

        private ToDoDisplay Spawn(ToDoViewModel viewModel)
        {
            var display = _objectPool.Count > 0 ? _objectPool.Dequeue() : CreateToDoDisplay();
            display.gameObject.SetActive(true);
            display.Inject(viewModel);
            display.name = $"{nameof(ToDoDisplay)}_{viewModel.Id}";
            display.transform.SetAsLastSibling();
            return display;
        }

        private void Despawn(ToDoDisplay display)
        {
            if (display.gameObject == null) return;
            display.gameObject.SetActive(false);
            _objectPool.Enqueue(display);
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
            _disposable = null;
            var displays = _displays.Values;
            _displays.Clear();
            foreach (var display in displays.Where(display => display != null && display.gameObject != null))
            {
                Destroy(display.gameObject);
            }
        }
    }
}