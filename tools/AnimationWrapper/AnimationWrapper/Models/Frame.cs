using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Text;

namespace AnimationWrapper
{
    public class Frame : BindableBase, IEquatable<Frame>
    {
        private Rectangle clipZone;
        private float duration;
        private Point offset;
        private bool isDamaging;
        private Rectangle hitbox;
        private int damage;

        public Rectangle ClipZone
        {
            get
            {
                return clipZone;
            }
            set
            {
                if (clipZone != value)
                {
                    clipZone = value;
                    RaisePropertyChanged(nameof(ClipZone));
                }
            }
        }

        public float Duration
        {
            get
            {
                return duration;
            }
            set
            {
                if (duration != value)
                {
                    duration = value;
                    RaisePropertyChanged(nameof(Duration));
                }
            }
        }

        public Point Offset
        {
            get
            {
                return offset;
            }
            set
            {
                if (offset != value)
                {
                    offset = value;
                    RaisePropertyChanged(nameof(Offset));
                }
            }
        }

        public bool IsDamaging
        {
            get
            {
                return isDamaging;
            }
            set
            {
                if (isDamaging != value)
                {
                    isDamaging = value;
                    RaisePropertyChanged(nameof(IsDamaging));
                }
            }
        }

        public Rectangle Hitbox
        {
            get
            {
                return hitbox;
            }
            set
            {
                if (hitbox != value)
                {
                    hitbox = value;
                    RaisePropertyChanged(nameof(Hitbox));
                }
            }
        }

        public int Damage
        {
            get
            {
                return damage;
            }
            set
            {
                if (damage != value)
                {
                    damage = value;
                    RaisePropertyChanged(nameof(Damage));
                }
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Frame);
        }

        public bool Equals(Frame other)
        {
            return other != null &&
                   ClipZone.Equals(other.ClipZone) &&
                   Duration == other.Duration &&
                   Offset.Equals(other.Offset) &&
                   IsDamaging == other.IsDamaging &&
                   Hitbox.Equals(other.Hitbox) &&
                   Damage == other.Damage;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ClipZone, Duration, Offset, IsDamaging, Hitbox, Damage);
        }

        public static bool operator ==(Frame left, Frame right)
        {
            return EqualityComparer<Frame>.Default.Equals(left, right);
        }

        public static bool operator !=(Frame left, Frame right)
        {
            return !(left == right);
        }
    }
}
