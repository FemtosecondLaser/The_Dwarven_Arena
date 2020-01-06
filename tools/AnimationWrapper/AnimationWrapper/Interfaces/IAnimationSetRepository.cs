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
        Task CreateAnimationSetAsync(string spriteSheetFilePath, string animationSetName);
        Task DeleteAnimationSetAsync(string animationSetName);
        Task<AnimationSet> GetAnimationSetAsync(string animationSetName);
    }
}
