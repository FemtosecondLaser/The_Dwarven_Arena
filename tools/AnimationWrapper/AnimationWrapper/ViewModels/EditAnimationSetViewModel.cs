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
        private readonly DelegateCommand<Animation> editAnimationCommand;
        private readonly DelegateCommand<Animation> deleteAnimationCommand;
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

            editAnimationCommand =
                new DelegateCommand<Animation>(
                    (animation) =>
                    {
                        if (animation == null)
                            throw new ArgumentNullException(nameof(animation));

                        //TODO: implement
                        throw new NotImplementedException();

                        //this.eventAggregator
                        //.GetEvent<EditAnimationRequestedEvent>()
                        //.Publish(animationName);
                    },
                    (animation) =>
                    {
                        return CanExecuteCommands;
                    }
                    );

            deleteAnimationCommand =
                new DelegateCommand<Animation>(
                    (animation) =>
                    {
                        if (animation == null)
                            throw new ArgumentNullException(nameof(animation));

                        CurrentlyEditedAnimationSet.Animations.Remove(animation);
                    },
                    (animation) =>
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

        public ICommand EditAnimationCommand
        {
            get
            {
                return editAnimationCommand;
            }
        }

        public ICommand DeleteAnimationCommand
        {
            get
            {
                return deleteAnimationCommand;
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
                    editAnimationCommand.RaiseCanExecuteChanged();
                    deleteAnimationCommand.RaiseCanExecuteChanged();
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
