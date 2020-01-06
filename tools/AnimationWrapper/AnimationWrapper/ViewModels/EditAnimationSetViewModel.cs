using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace AnimationWrapper
{
    public class EditAnimationSetViewModel : BindableBase, IDisposable
    {
        private readonly IAnimationSetRepository animationSetRepository;
        private readonly IEventAggregator eventAggregator;
        private readonly DelegateCommand newAnimationCommand;
        private readonly DelegateCommand saveAndReturnCommand;
        private readonly DelegateCommand cancelCommand;
        private bool canExecuteCommands = true;
        private AnimationSet currentlyEditedAnimationSet;

        public EditAnimationSetViewModel(
            IAnimationSetRepository animationSetRepository,
            IEventAggregator eventAggregator
            )
        {
            if (animationSetRepository == null)
                throw new ArgumentNullException(nameof(animationSetRepository));

            if (eventAggregator == null)
                throw new ArgumentNullException(nameof(eventAggregator));

            this.animationSetRepository = animationSetRepository;
            this.eventAggregator = eventAggregator;

            this.eventAggregator
                .GetEvent<EditAnimationSetRequestedEvent>()
                .Subscribe(EditAnimationSetRequestedEventHandler, ThreadOption.UIThread, false);

            newAnimationCommand = new DelegateCommand(
                () =>
                {
                    this.eventAggregator
                    .GetEvent<NewAnimationRequestedEvent>()
                    .Publish(CurrentlyEditedAnimationSet);
                },
                () =>
                {
                    return CanExecuteCommands;
                }
                );

            saveAndReturnCommand = new DelegateCommand(
                () =>
                {
                    //TODO: implement
                    throw new NotImplementedException();
                },
                () =>
                {
                    return CanExecuteCommands;
                }
                );

            cancelCommand = new DelegateCommand(
                () =>
                {
                    this.eventAggregator
                    .GetEvent<EditAnimationSetFinishedEvent>()
                    .Publish();
                },
                () =>
                {
                    return CanExecuteCommands;
                }
                );
        }

        public ICommand NewAnimationCommand
        {
            get
            {
                return newAnimationCommand;
            }
        }

        public ICommand SaveAndReturnCommand
        {
            get
            {
                return saveAndReturnCommand;
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return cancelCommand;
            }
        }

        public bool CanExecuteCommands
        {
            get
            {
                return canExecuteCommands;
            }
            private set
            {
                if (canExecuteCommands != value)
                {
                    canExecuteCommands = value;
                    RaisePropertyChanged(nameof(CanExecuteCommands));
                    newAnimationCommand.RaiseCanExecuteChanged();
                    saveAndReturnCommand.RaiseCanExecuteChanged();
                    cancelCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public AnimationSet CurrentlyEditedAnimationSet
        {
            get
            {
                return currentlyEditedAnimationSet;
            }
            set
            {
                if (currentlyEditedAnimationSet != value)
                {
                    currentlyEditedAnimationSet = value;
                    RaisePropertyChanged(nameof(CurrentlyEditedAnimationSet));
                }
            }
        }

        private async void EditAnimationSetRequestedEventHandler(string animationSetName)
        {
            if (animationSetName == null)
                throw new ArgumentNullException(nameof(animationSetName));

            CurrentlyEditedAnimationSet =
                await animationSetRepository.GetAnimationSetAsync(animationSetName);
        }

        public void Dispose()
        {
            eventAggregator
                .GetEvent<EditAnimationSetRequestedEvent>()
                .Unsubscribe(EditAnimationSetRequestedEventHandler);
        }
    }
}
