using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AnimationWrapper
{
    public interface IAnimationSetRepository
    {
        event AnimationSetCreatedEventHandler AnimationSetCreated;
        event AnimationSetDeletedEventHandler AnimationSetDeleted;
        IEnumerable<string> GetAllAnimationSetNames();
        bool AnimationSetExists(string animationSetName);
        Task CreateAnimationSet(string spriteSheetFilePath, string animationSetName);
        Task DeleteAnimationSet(string animationSetName);
    }
}
