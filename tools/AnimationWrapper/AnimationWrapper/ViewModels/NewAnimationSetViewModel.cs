using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO.Abstractions;
using System.Text;
using System.Windows.Input;

namespace AnimationWrapper
{
    public class NewAnimationSetViewModel : BindableBase, INotifyDataErrorInfo
    {
        private readonly IAnimationSetRepository animationSetRepository;
        private readonly IFileSelectionProvider fileSelectionProvider;
        private readonly IEventAggregator eventAggregator;
        private readonly IFileSystem fileSystem;
        private string spriteSheetFilePath;
        private string animationSetName;
        private readonly DelegateCommand browseSpriteSheetFilePathCommand;
        private readonly DelegateCommand createAnimationSetCommand;
        private readonly DelegateCommand cancelCommand;
        private readonly ErrorsContainer<ValidationResult> errorsContainer;

        public NewAnimationSetViewModel(
            IAnimationSetRepository animationSetRepository,
            IFileSelectionProvider fileSelectionProvider,
            IEventAggregator eventAggregator,
            IFileSystem fileSystem
            )
        {
            if (animationSetRepository == null)
                throw new ArgumentNullException(nameof(animationSetRepository));

            if (fileSelectionProvider == null)
                throw new ArgumentNullException(nameof(fileSelectionProvider));

            if (eventAggregator == null)
                throw new ArgumentNullException(nameof(eventAggregator));

            if (fileSystem == null)
                throw new ArgumentNullException(nameof(fileSystem));

            this.animationSetRepository = animationSetRepository;
            this.fileSelectionProvider = fileSelectionProvider;
            this.eventAggregator = eventAggregator;
            this.fileSystem = fileSystem;

            errorsContainer =
                new ErrorsContainer<ValidationResult>(
                    (propertyName) => { RaiseErrorsChanged(propertyName); }
                    );

            browseSpriteSheetFilePathCommand =
                new DelegateCommand(
                    () => { SpriteSheetFilePath = fileSelectionProvider.SelectPngFile(); }
                    );

            createAnimationSetCommand =
                new DelegateCommand(
                    () =>
                    {
                        ValidateSpriteSheetFilePath();
                        ValidateAnimationSetName();
                        if (HasErrors) return;
                    }
                    );

            cancelCommand =
                new DelegateCommand(() => { });
        }

        public string SpriteSheetFilePath
        {
            get
            {
                return spriteSheetFilePath;
            }
            set
            {
                if (spriteSheetFilePath != value)
                {
                    spriteSheetFilePath = value;
                    RaisePropertyChanged(nameof(SpriteSheetFilePath));
                    var errors = GetErrors(nameof(SpriteSheetFilePath));
                    if (errors != null && errors.GetEnumerator().MoveNext())
                        ValidateSpriteSheetFilePath();
                }
            }
        }

        public string AnimationSetName
        {
            get
            {
                return animationSetName;
            }
            set
            {
                if (animationSetName != value)
                {
                    animationSetName = value;
                    RaisePropertyChanged(nameof(AnimationSetName));
                    var errors = GetErrors(nameof(AnimationSetName));
                    if (errors != null && errors.GetEnumerator().MoveNext())
                        ValidateAnimationSetName();
                }
            }
        }

        public ICommand BrowseSpriteSheetFilePathCommand
        {
            get
            {
                return browseSpriteSheetFilePathCommand;
            }
        }

        public ICommand CreateAnimationSetCommand
        {
            get
            {
                return createAnimationSetCommand;
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

        private void ValidateSpriteSheetFilePath()
        {
            var errorlist = new List<ValidationResult>();

            if (String.IsNullOrEmpty(SpriteSheetFilePath))
                errorlist.Add(new ValidationResult("Sprite sheet file path must not be empty."));
            if (!fileSystem.File.Exists(SpriteSheetFilePath))
                errorlist.Add(new ValidationResult("Sprite sheet file must exist."));
            if (fileSystem.Path.GetExtension(SpriteSheetFilePath)?.ToLower() != ".png")
                errorlist.Add(new ValidationResult("Sprite sheet file must have .png extension."));
            
            errorsContainer.SetErrors(nameof(SpriteSheetFilePath), errorlist);
        }

        private void ValidateAnimationSetName()
        {
            var errorlist = new List<ValidationResult>();
            if (String.IsNullOrEmpty(AnimationSetName))
                errorlist.Add(new ValidationResult("Animation set name must not be empty."));
            if (AnimationSetName == null ? false : animationSetRepository.AnimationSetExists(AnimationSetName))
                errorlist.Add(new ValidationResult("Animation set name must not be taken."));
            errorsContainer.SetErrors(nameof(AnimationSetName), errorlist);
        }
    }
}
