using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationWrapper
{
    public interface IAnimationSetRepository
    {
        IEnumerable<string> GetAllAnimationSetNames();
        bool AnimationSetExists(string animationSetName);
    }
}
