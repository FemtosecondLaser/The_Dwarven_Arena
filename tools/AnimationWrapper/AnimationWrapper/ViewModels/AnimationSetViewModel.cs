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
        private bool canExecuteDeleteAnimationSetCommand = true;

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
                        .GetEvent<NewAnimationSetRequested>()
                        .Publish();
                    }
                    );

            editAnimationSetCommand =
                new DelegateCommand<string>(
                    (animationSetName) =>
                    {
                        if (animationSetName == null)
                            throw new ArgumentNullException(nameof(animationSetName));

                        //TODO: implement
                        throw new NotImplementedException();
                    }
                    );

            deleteAnimationSetCommand =
                new DelegateCommand<string>(
                    async (animationSetName) =>
                    {
                        CanExecuteDeleteAnimationSetCommand = false;

                        try
                        {
                            if (animationSetName == null)
                                throw new ArgumentNullException(nameof(animationSetName));

                            await this.animationSetRepository.DeleteAnimationSet(animationSetName);
                        }
                        finally
                        {
                            CanExecuteDeleteAnimationSetCommand = true;
                        }
                    },
                    (animationSetName) =>
                    {
                        return CanExecuteDeleteAnimationSetCommand;
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

        public bool CanExecuteDeleteAnimationSetCommand
        {
            get
            {
                return canExecuteDeleteAnimationSetCommand;
            }
            private set
            {
                if (canExecuteDeleteAnimationSetCommand != value)
                {
                    canExecuteDeleteAnimationSetCommand = value;
                    RaisePropertyChanged(nameof(CanExecuteDeleteAnimationSetCommand));
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
