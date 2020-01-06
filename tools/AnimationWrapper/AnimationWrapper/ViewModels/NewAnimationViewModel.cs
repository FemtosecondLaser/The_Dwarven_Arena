using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace AnimationWrapper
{
    public class NewAnimationViewModel : BindableBase, INotifyDataErrorInfo, IDisposable
    {
        private readonly IEventAggregator eventAggregator;
        private string animationName;
        private readonly DelegateCommand createAnimationCommand;
        private bool canExecuteCreateAnimationCommand = true;
        private readonly DelegateCommand cancelCommand;
        private readonly ErrorsContainer<ValidationResult> errorsContainer;
        private AnimationSet animationSetToAddAnimationTo;

        public NewAnimationViewModel(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
                throw new ArgumentNullException(nameof(eventAggregator));

            this.eventAggregator = eventAggregator;

            this.eventAggregator
                .GetEvent<NewAnimationRequestedEvent>()
                .Subscribe(NewAnimationRequestedEventHandler, ThreadOption.UIThread, false);

            errorsContainer =
                new ErrorsContainer<ValidationResult>(
                    (propertyName) => { RaiseErrorsChanged(propertyName); }
                    );

            createAnimationCommand =
                new DelegateCommand(
                    () =>
                    {
                        CanExecuteCreateAnimationCommand = false;

                        try
                        {
                            ValidateAnimationName();
                            if (HasErrors) return;

                            animationSetToAddAnimationTo.Animations.Add(new Animation() { Name = AnimationName });

                            this.eventAggregator.GetEvent<NewAnimationFinishedEvent>().Publish();
                        }
                        finally
                        {
                            CanExecuteCreateAnimationCommand = true;
                        }
                    },
                    () =>
                    {
                        return CanExecuteCreateAnimationCommand;
                    }
                    );

            cancelCommand =
                new DelegateCommand(
                    () => { this.eventAggregator.GetEvent<NewAnimationFinishedEvent>().Publish(); }
                    );
        }

        public string AnimationName
        {
            get
            {
                return animationName;
            }
            set
            {
                if (animationName != value)
                {
                    animationName = value;
                    RaisePropertyChanged(nameof(AnimationName));
                    var errors = GetErrors(nameof(AnimationName));
                    if (errors != null && errors.GetEnumerator().MoveNext())
                        ValidateAnimationName();
                }
            }
        }

        public ICommand CreateAnimationCommand
        {
            get
            {
                return createAnimationCommand;
            }
        }

        public bool CanExecuteCreateAnimationCommand
        {
            get
            {
                return canExecuteCreateAnimationCommand;
            }
            private set
            {
                if (canExecuteCreateAnimationCommand != value)
                {
                    canExecuteCreateAnimationCommand = value;
                    RaisePropertyChanged(nameof(CanExecuteCreateAnimationCommand));
                    createAnimationCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return cancelCommand;
            }
        }

        public bool HasErrors
        {
            get
            {
                return errorsContainer.HasErrors;
            }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            return errorsContainer.GetErrors(propertyName);
        }

        protected void RaiseErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        private void ValidateAnimationName()
        {
            var errorlist = new List<ValidationResult>();

            if (String.IsNullOrEmpty(AnimationName))
                errorlist.Add(new ValidationResult("Animation name must not be empty."));
            if (AnimationName == null ? false : AnimationName.Any(char.IsWhiteSpace))
                errorlist.Add(new ValidationResult("Animation name must not contain white space."));
            if (AnimationName == null ? false : animationSetToAddAnimationTo.Animations.Select(animation => animation.Name).Contains(AnimationName))
                errorlist.Add(new ValidationResult("Animation name must not be taken."));

            errorsContainer.SetErrors(nameof(AnimationName), errorlist);
        }

        private void NewAnimationRequestedEventHandler(AnimationSet animationSet)
        {
            if (animationSet == null)
                throw new ArgumentNullException(nameof(animationSet));

            animationSetToAddAnimationTo = animationSet;
        }

        public void Dispose()
        {
            this.eventAggregator
                .GetEvent<NewAnimationRequestedEvent>()
                .Unsubscribe(NewAnimationRequestedEventHandler);
        }
    }
}
