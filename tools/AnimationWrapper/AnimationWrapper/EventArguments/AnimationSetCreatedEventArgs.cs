using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationWrapper
{
    public class AnimationSetCreatedEventArgs : EventArgs
    {
        private readonly string createdAnimationSetName;

        public AnimationSetCreatedEventArgs(string createdAnimationSetName)
        {
            if (createdAnimationSetName == null)
                throw new ArgumentNullException(nameof(createdAnimationSetName));

            this.createdAnimationSetName = createdAnimationSetName;
        }

        public string CreatedAnimationSetName
        {
            get
            {
                return createdAnimationSetName;
            }
        }
    }
}
