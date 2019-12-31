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
    public class AnimationSetViewModel : IDisposable
    {
        private readonly IAnimationSetRepository animationSetRepository;
        private readonly IEventAggregator eventAggregator;
        private readonly ObservableCollection<string> animationSetNames;
        private readonly DelegateCommand requestNewAnimationSetCommand;

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

            this.animationSetRepository.AnimationSetCreated +=
                AnimationSetRepository_AnimationSetCreated;

            animationSetNames =
                new ObservableCollection<string>(
                    this.animationSetRepository.GetAllAnimationSetNames()
                    );

            requestNewAnimationSetCommand =
                new DelegateCommand(
                    () =>
                    {
                        this.eventAggregator
                        .GetEvent<NewAnimationSetRequested>()
                        .Publish();
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

        private void AnimationSetRepository_AnimationSetCreated(
            AnimationSetCreatedEventArgs e
            )
        {
            AnimationSetNames.Add(e.CreatedAnimationSetName);
        }

        public void Dispose()
        {
            this.animationSetRepository.AnimationSetCreated -=
                AnimationSetRepository_AnimationSetCreated;
        }
    }
}
