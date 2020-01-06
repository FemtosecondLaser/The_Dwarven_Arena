using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace AnimationWrapper
{
    public class PresentingViewModel : BindableBase, IDisposable
    {
        private readonly Stack<object> navigationStack = new Stack<object>();
        private readonly IEventAggregator eventAggregator;
        private readonly IAnimationSetView animationSetView;
        private readonly INewAnimationSetView newAnimationSetView;
        private readonly IEditAnimationSetView editAnimationSetView;
        private readonly INewAnimationView newAnimationView;

        public PresentingViewModel(
            IEventAggregator eventAggregator,
            IAnimationSetView animationSetView,
            INewAnimationSetView newAnimationSetView,
            IEditAnimationSetView editAnimationSetView,
            INewAnimationView newAnimationView
            )
        {
            if (eventAggregator == null)
                throw new ArgumentNullException(nameof(eventAggregator));

            if (animationSetView == null)
                throw new ArgumentNullException(nameof(animationSetView));

            if (newAnimationSetView == null)
                throw new ArgumentNullException(nameof(newAnimationSetView));

            if (editAnimationSetView == null)
                throw new ArgumentNullException(nameof(editAnimationSetView));

            if (newAnimationView == null)
                throw new ArgumentNullException(nameof(newAnimationView));

            this.eventAggregator = eventAggregator;
            this.animationSetView = animationSetView;
            this.newAnimationSetView = newAnimationSetView;
            this.editAnimationSetView = editAnimationSetView;
            this.newAnimationView = newAnimationView;

            this.eventAggregator
                .GetEvent<NewAnimationSetRequestedEvent>()
                .Subscribe(NewAnimationSetRequestedEventHandler, ThreadOption.UIThread, false);

            this.eventAggregator
                .GetEvent<NewAnimationSetFinishedEvent>()
                .Subscribe(NewAnimationSetFinishedEventHandler, ThreadOption.UIThread, false);

            this.eventAggregator
                .GetEvent<EditAnimationSetRequestedEvent>()
                .Subscribe(EditAnimationSetRequestedEventHandler, ThreadOption.UIThread, false);

            this.eventAggregator
                .GetEvent<EditAnimationSetFinishedEvent>()
                .Subscribe(EditAnimationSetFinishedEventHandler, ThreadOption.UIThread, false);

            this.eventAggregator
                .GetEvent<NewAnimationRequestedEvent>()
                .Subscribe(NewAnimationRequestedEventHandler, ThreadOption.UIThread, false);

            this.eventAggregator
                .GetEvent<NewAnimationFinishedEvent>()
                .Subscribe(NewAnimationFinishedEventHandler, ThreadOption.UIThread, false);

            NavigateTo(this.animationSetView);
        }

        public object CurrentView
        {
            get
            {
                if (navigationStack.TryPeek(out object currentView))
                    return currentView;
                else
                    return null;
            }
        }

        private void NavigateTo(object view)
        {
            navigationStack.Push(view);
            RaisePropertyChanged(nameof(CurrentView));
        }

        private bool CanNavigateBack()
        {
            return navigationStack.Count > 0;
        }

        private void NavigateBack()
        {
            if (navigationStack.TryPop(out object view))
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(CurrentView)));
            else
                throw new InvalidOperationException("Could not navigate back.");
        }

        private void NavigateBackIfCurrentViewTypeIs(Type currentViewType)
        {
            if (currentViewType == null)
                throw new ArgumentNullException(nameof(currentViewType));

            if (CanNavigateBack() && currentViewType.IsAssignableFrom(CurrentView.GetType()))
                NavigateBack();
            else
                throw new InvalidOperationException(
                    $"Can not navigate back or the current view is not {currentViewType.Name}"
                    );
        }

        private void NewAnimationSetRequestedEventHandler()
        {
            NavigateTo(newAnimationSetView);
        }

        private void NewAnimationSetFinishedEventHandler()
        {
            NavigateBackIfCurrentViewTypeIs(typeof(INewAnimationSetView));
        }

        private void EditAnimationSetRequestedEventHandler(string animationSetName)
        {
            if (animationSetName == null)
                throw new ArgumentNullException(nameof(animationSetName));

            NavigateTo(editAnimationSetView);
        }

        private void EditAnimationSetFinishedEventHandler()
        {
            NavigateBackIfCurrentViewTypeIs(typeof(IEditAnimationSetView));
        }

        private void NewAnimationRequestedEventHandler(AnimationSet animationSet)
        {
            if (animationSet == null)
                throw new ArgumentNullException(nameof(animationSet));

            NavigateTo(newAnimationView);
        }

        private void NewAnimationFinishedEventHandler()
        {
            NavigateBackIfCurrentViewTypeIs(typeof(INewAnimationView));
        }

        public void Dispose()
        {
            eventAggregator
                .GetEvent<NewAnimationSetRequestedEvent>()
                .Unsubscribe(NewAnimationSetRequestedEventHandler);

            eventAggregator
                .GetEvent<NewAnimationSetFinishedEvent>()
                .Unsubscribe(NewAnimationSetFinishedEventHandler);

            eventAggregator
                .GetEvent<EditAnimationSetRequestedEvent>()
                .Unsubscribe(EditAnimationSetRequestedEventHandler);

            eventAggregator
                .GetEvent<EditAnimationSetFinishedEvent>()
                .Unsubscribe(EditAnimationSetFinishedEventHandler);

            eventAggregator
                .GetEvent<NewAnimationRequestedEvent>()
                .Unsubscribe(NewAnimationRequestedEventHandler);

            eventAggregator
                .GetEvent<NewAnimationFinishedEvent>()
                .Unsubscribe(NewAnimationFinishedEventHandler);
        }
    }
}
