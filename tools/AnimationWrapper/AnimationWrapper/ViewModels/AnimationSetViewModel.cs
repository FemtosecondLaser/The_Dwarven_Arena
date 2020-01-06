using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;

namespace AnimationWrapper
{
    public class AnimationSetViewModel : BindableBase, IDisposable
    {
        private readonly IAnimationSetRepository animationSetRepository;
        private readonly IEventAggregator eventAggregator;
        private readonly ObservableCollection<string> animationSetNames;
        private readonly DelegateCommand requestNewAnimationSetCommand;
        private readonly DelegateCommand<string> editAnimationSetCommand;
        private readonly DelegateCommand<string> deleteAnimationSetCommand;
        private bool canExecuteAnimationSetCommand = true;

        public AnimationSetViewModel(
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

            animationSetNames =
                new ObservableCollection<string>(
                    this.animationSetRepository.GetAllAnimationSetNames()
                    );

            this.animationSetRepository.AnimationSetCreated +=
                AnimationSetRepository_AnimationSetCreated;
            this.animationSetRepository.AnimationSetDeleted +=
                AnimationSetRepository_AnimationSetDeleted;

            requestNewAnimationSetCommand =
                new DelegateCommand(
                    () =>
                    {
                        this.eventAggregator
                        .GetEvent<NewAnimationSetRequestedEvent>()
                        .Publish();
                    }
                    );

            editAnimationSetCommand =
                new DelegateCommand<string>(
                    (animationSetName) =>
                    {
                        CanExecuteAnimationSetCommand = false;

                        try
                        {
                            if (animationSetName == null)
                                throw new ArgumentNullException(nameof(animationSetName));

                            this.eventAggregator
                            .GetEvent<EditAnimationSetRequestedEvent>()
                            .Publish(animationSetName);
                        }
                        finally
                        {
                            CanExecuteAnimationSetCommand = true;
                        }

                    },
                    (animationSetName) =>
                    {
                        return CanExecuteAnimationSetCommand;
                    }
                    );

            deleteAnimationSetCommand =
                new DelegateCommand<string>(
                    async (animationSetName) =>
                    {
                        CanExecuteAnimationSetCommand = false;

                        try
                        {
                            if (animationSetName == null)
                                throw new ArgumentNullException(nameof(animationSetName));

                            await this.animationSetRepository.DeleteAnimationSetAsync(animationSetName);
                        }
                        finally
                        {
                            CanExecuteAnimationSetCommand = true;
                        }
                    },
                    (animationSetName) =>
                    {
                        return CanExecuteAnimationSetCommand;
                    }
                    );
        }

        public ObservableCollection<string> AnimationSetNames
        {
            get
            {
                return animationSetNames;
            }
        }

        public ICommand RequestNewAnimationSetCommand
        {
            get
            {
                return requestNewAnimationSetCommand;
            }
        }

        public ICommand EditAnimationSetCommand
        {
            get
            {
                return editAnimationSetCommand;
            }
        }

        public ICommand DeleteAnimationSetCommand
        {
            get
            {
                return deleteAnimationSetCommand;
            }
        }

        public bool CanExecuteAnimationSetCommand
        {
            get
            {
                return canExecuteAnimationSetCommand;
            }
            private set
            {
                if (canExecuteAnimationSetCommand != value)
                {
                    canExecuteAnimationSetCommand = value;
                    RaisePropertyChanged(nameof(CanExecuteAnimationSetCommand));
                    editAnimationSetCommand.RaiseCanExecuteChanged();
                    deleteAnimationSetCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private void AnimationSetRepository_AnimationSetCreated(
            AnimationSetCreatedEventArgs e
            )
        {
            AnimationSetNames.Add(e.CreatedAnimationSetName);
        }

        private void AnimationSetRepository_AnimationSetDeleted(
            AnimationSetDeletedEventArgs e
            )
        {
            AnimationSetNames.Remove(e.DeletedAnimationSetName);
        }

        public void Dispose()
        {
            this.animationSetRepository.AnimationSetCreated -=
                AnimationSetRepository_AnimationSetCreated;
            this.animationSetRepository.AnimationSetDeleted -=
                AnimationSetRepository_AnimationSetDeleted;
        }
    }
}
