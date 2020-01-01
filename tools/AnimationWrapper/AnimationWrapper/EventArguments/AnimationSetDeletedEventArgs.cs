using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationWrapper
{
    public class AnimationSetDeletedEventArgs : EventArgs
    {
        private readonly string deletedAnimationSetName;

        public AnimationSetDeletedEventArgs(string deletedAnimationSetName)
        {
            if (deletedAnimationSetName == null)
                throw new ArgumentNullException(nameof(deletedAnimationSetName));

            this.deletedAnimationSetName = deletedAnimationSetName;
        }

        public string DeletedAnimationSetName
        {
            get
            {
                return deletedAnimationSetName;
            }
        }
    }
}
