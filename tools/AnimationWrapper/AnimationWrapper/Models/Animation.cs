using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace AnimationWrapper
{
    public class Animation : BindableBase, IEquatable<Animation>
    {
        private string name;
        private ObservableCollection<Frame> frames = new ObservableCollection<Frame>();

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;
                    RaisePropertyChanged(nameof(Name));
                }
            }
        }

        public ObservableCollection<Frame> Frames
        {
            get
            {
                return frames;
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Animation);
        }

        public bool Equals(Animation other)
        {
            return other != null &&
                   Name == other.Name &&
                   EqualityComparer<ObservableCollection<Frame>>.Default.Equals(Frames, other.Frames);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Frames);
        }

        public static bool operator ==(Animation left, Animation right)
        {
            return EqualityComparer<Animation>.Default.Equals(left, right);
        }

        public static bool operator !=(Animation left, Animation right)
        {
            return !(left == right);
        }
    }
}
