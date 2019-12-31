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

        public PresentingViewModel(
            IEventAggregator eventAggregator,
            IAnimationSetView animationSetView,
            INewAnimationSetView newAnimationSetView
            )
        {
            if (eventAggregator == null)
                throw new ArgumentNullException(nameof(eventAggregator));

            if (animationSetView == null)
                throw new ArgumentNullException(nameof(animationSetView));

            if (newAnimationSetView == null)
                throw new ArgumentNullException(nameof(newAnimationSetView));

            this.eventAggregator = eventAggregator;
            this.animationSetView = animationSetView;
            this.newAnimationSetView = newAnimationSetView;

            this.eventAggregator
                .GetEvent<NewAnimationSetRequested>()
                .Subscribe(NewAnimationSetRequestedEventHandler, ThreadOption.UIThread, false);

            this.eventAggregator
                .GetEvent<NewAnimationSetFinished>()
                .Subscribe(NewAnimationSetFinishedEventHandler, ThreadOption.UIThread, false);

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

        private void NewAnimationSetRequestedEventHandler()
        {
            NavigateTo(newAnimationSetView);
        }

        private void NewAnimationSetFinishedEventHandler()
        {
            // TODO: reusable concept
            if (CanNavigateBack() && typeof(INewAnimationSetView).IsAssignableFrom(CurrentView.GetType()))
                NavigateBack();
            else
                throw new InvalidOperationException(
                    $"Can not navigate back or the current view is not {nameof(INewAnimationSetView)}"
                    );
        }

        public void Dispose()
        {
            eventAggregator
                .GetEvent<NewAnimationSetRequested>()
                .Unsubscribe(NewAnimationSetRequestedEventHandler);

            eventAggregator
                .GetEvent<NewAnimationSetFinished>()
                .Unsubscribe(NewAnimationSetFinishedEventHandler);
        }
    }
}
