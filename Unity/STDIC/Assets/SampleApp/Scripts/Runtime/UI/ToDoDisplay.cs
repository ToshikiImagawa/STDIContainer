// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;
using SampleApp.Model;
using SampleApp.Utils;
using SampleApp.ViewModel;
using UnityEngine;
using UnityEngine.UI;

namespace SampleApp.UI
{
    public class ToDoDisplay : MonoBehaviour
    {
        // ReSharper disable once RedundantDefaultMemberInitializer
        [SerializeField] private Text messageText = default;

        // ReSharper disable once RedundantDefaultMemberInitializer
        [SerializeField] private Text stateText = default;

        // ReSharper disable once RedundantDefaultMemberInitializer
        [SerializeField] private Button reopenButton = default;

        // ReSharper disable once RedundantDefaultMemberInitializer
        [SerializeField] private Button startButton = default;

        // ReSharper disable once RedundantDefaultMemberInitializer
        [SerializeField] private Button closeButton = default;

        // ReSharper disable once RedundantDefaultMemberInitializer
        [SerializeField] private Button stopButton = default;

        // ReSharper disable once RedundantDefaultMemberInitializer
        [SerializeField] private Button deleteButton = default;

        // ReSharper disable once RedundantDefaultMemberInitializer
        [SerializeField] private Button toggleButtonsButton = default;

        // ReSharper disable once RedundantDefaultMemberInitializer
        [SerializeField] private RectTransform buttonList = default;

        // ReSharper disable once RedundantDefaultMemberInitializer
        [SerializeField] private RectTransform openViewArea = default;

        // ReSharper disable once RedundantDefaultMemberInitializer
        [SerializeField] private RectTransform inProgressViewArea = default;

        // ReSharper disable once RedundantDefaultMemberInitializer
        [SerializeField] private RectTransform stopViewArea = default;

        // ReSharper disable once RedundantDefaultMemberInitializer
        [SerializeField] private RectTransform closeViewArea = default;

        private IDisposable _disposable;

        public void Inject(ToDoViewModel viewModel)
        {
            messageText.text = viewModel.Message;
            UpdateDisplay(viewModel.Status);
            _disposable?.Dispose();
            _disposable = new CompositeDisposable()
                .Add(reopenButton.onClick.Subscribe(() =>
                {
                    if (viewModel.Status == ToDoStatus.Open) return;
                    viewModel.Status = ToDoStatus.Open;
                    buttonList.gameObject.SetActive(!buttonList.gameObject.activeSelf);
                }))
                .Add(startButton.onClick.Subscribe(() =>
                {
                    if (viewModel.Status != ToDoStatus.Open) return;
                    viewModel.Status = ToDoStatus.InProgress;
                    buttonList.gameObject.SetActive(!buttonList.gameObject.activeSelf);
                }))
                .Add(closeButton.onClick.Subscribe(() =>
                {
                    if (viewModel.Status == ToDoStatus.Stopped) return;
                    viewModel.Status = ToDoStatus.Close;
                    buttonList.gameObject.SetActive(!buttonList.gameObject.activeSelf);
                }))
                .Add(stopButton.onClick.Subscribe(() =>
                {
                    if (viewModel.Status == ToDoStatus.Close) return;
                    viewModel.Status = ToDoStatus.Stopped;
                    buttonList.gameObject.SetActive(!buttonList.gameObject.activeSelf);
                }))
                .Add(toggleButtonsButton.onClick.Subscribe(() =>
                {
                    buttonList.gameObject.SetActive(!buttonList.gameObject.activeSelf);
                }))
                .Add(deleteButton.onClick.Subscribe(viewModel.Delete))
                .Add(viewModel.ToDo.Subscribe(OnUpdate));
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
            _disposable = null;
        }

        private void OnUpdate(ToDo toDo)
        {
            UpdateDisplay(toDo.Status);
        }

        private void UpdateDisplay(ToDoStatus status)
        {
            stateText.text = status.ToJpString();
            reopenButton.interactable = status != ToDoStatus.Open;
            startButton.interactable = status == ToDoStatus.Open;
            closeButton.interactable = status == ToDoStatus.Open || status == ToDoStatus.InProgress;
            stopButton.interactable = status == ToDoStatus.Open || status == ToDoStatus.InProgress;
            switch (status)
            {
                case ToDoStatus.Open:
                    transform.SetParent(openViewArea, false);
                    transform.SetAsLastSibling();
                    break;
                case ToDoStatus.InProgress:
                    transform.SetParent(inProgressViewArea, false);
                    transform.SetAsLastSibling();
                    break;
                case ToDoStatus.Close:
                    transform.SetParent(closeViewArea, false);
                    transform.SetAsLastSibling();
                    break;
                case ToDoStatus.Stopped:
                    transform.SetParent(stopViewArea, false);
                    transform.SetAsLastSibling();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }
    }
}