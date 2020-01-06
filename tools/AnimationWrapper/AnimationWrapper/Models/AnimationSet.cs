using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace AnimationWrapper
{
    public class AnimationSet : BindableBase, IEquatable<AnimationSet>
    {
        private string spriteSheetFileName;
        private ObservableCollection<Animation> animations = new ObservableCollection<Animation>();

        public string SpriteSheetFileName
        {
            get
            {
                return spriteSheetFileName;
            }
            set
            {
                if (spriteSheetFileName != value)
                {
                    spriteSheetFileName = value;
                    RaisePropertyChanged(nameof(SpriteSheetFileName));
                }
            }
        }

        public ObservableCollection<Animation> Animations
        {
            get
            {
                return animations;
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as AnimationSet);
        }

        public bool Equals(AnimationSet other)
        {
            return other != null &&
                   SpriteSheetFileName == other.SpriteSheetFileName &&
                   EqualityComparer<ObservableCollection<Animation>>.Default.Equals(Animations, other.Animations);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SpriteSheetFileName, Animations);
        }

        public static bool operator ==(AnimationSet left, AnimationSet right)
        {
            return EqualityComparer<AnimationSet>.Default.Equals(left, right);
        }

        public static bool operator !=(AnimationSet left, AnimationSet right)
        {
            return !(left == right);
        }
    }
}
