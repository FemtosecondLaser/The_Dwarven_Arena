using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;

namespace AnimationWrapper
{
    public class TwoFileAnimationSetRepository : IAnimationSetRepository
    {
        private const string animationSetFileExtension = ".animationset";
        private readonly string repositoryDirectory;
        private readonly IFileSystem fileSystem;

        public TwoFileAnimationSetRepository(
            string repositoryDirectory,
            IFileSystem fileSystem
            )
        {
            if (repositoryDirectory == null)
                throw new ArgumentNullException(nameof(repositoryDirectory));

            if (fileSystem == null)
                throw new ArgumentNullException(nameof(fileSystem));

            fileSystem.Directory.CreateDirectory(repositoryDirectory);

            this.repositoryDirectory = repositoryDirectory;
            this.fileSystem = fileSystem;
        }

        public IEnumerable<string> GetAllAnimationSetNames()
        {
            return fileSystem.Directory.EnumerateFiles(repositoryDirectory, $"*{animationSetFileExtension}")
                .Select(fName => fileSystem.Path.GetFileNameWithoutExtension(fName));
        }
    }
}
