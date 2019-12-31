using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public event AnimationSetCreatedEventHandler AnimationSetCreated;

        public IEnumerable<string> GetAllAnimationSetNames()
        {
            return fileSystem.Directory.EnumerateFiles(repositoryDirectory, $"*{animationSetFileExtension}")
                .Select(fName => fileSystem.Path.GetFileNameWithoutExtension(fName));
        }

        public bool AnimationSetExists(string animationSetName)
        {
            if (animationSetName == null)
                throw new ArgumentNullException(nameof(animationSetName));

            return fileSystem.File.Exists(
                fileSystem.Path.Combine(repositoryDirectory, $"{animationSetName}{animationSetFileExtension}")
                );
        }

        public async Task CreateAnimationSet(string spriteSheetFilePath, string animationSetName)
        {
            if (spriteSheetFilePath == null)
                throw new ArgumentNullException(nameof(spriteSheetFilePath));

            if (animationSetName == null)
                throw new ArgumentNullException(nameof(animationSetName));

            var spriteSheetFileName =
                fileSystem.Path.HasExtension(spriteSheetFilePath) ?
                $"{animationSetName}{fileSystem.Path.GetExtension(spriteSheetFilePath)}" :
                animationSetName;

            var animationSetFilePath =
                fileSystem.Path.Combine(repositoryDirectory, $"{animationSetName}{animationSetFileExtension}");

            await CopyFileAsync(
                spriteSheetFilePath,
                fileSystem.Path.Combine(repositoryDirectory, spriteSheetFileName),
                true
                );

            using (var animationSetFileStream =
                fileSystem.FileStream.Create(
                    animationSetFilePath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None
                ))
            using (var animationSetFileStreamWriter =
                new StreamWriter(
                    animationSetFileStream,
                    leaveOpen: true
                    ))
                await animationSetFileStreamWriter.WriteLineAsync(
                    $"sprite_sheet_file_name {spriteSheetFileName.Length} {spriteSheetFileName}"
                    );

            AnimationSetCreated?.Invoke(new AnimationSetCreatedEventArgs(animationSetName));
        }

        private async Task CopyFileAsync(string fromFilePath, string toFilePath, bool overwriteFile)
        {
            if (fromFilePath == null)
                throw new ArgumentNullException(nameof(fromFilePath));

            if (toFilePath == null)
                throw new ArgumentNullException(nameof(toFilePath));

            using (var fromFileStream =
                fileSystem.FileStream.Create(
                    fromFilePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read
                    ))
            using (var toFileStream =
                fileSystem.FileStream.Create(
                    toFilePath,
                    overwriteFile ? FileMode.Create : FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None
                    ))
                await fromFileStream.CopyToAsync(toFileStream).ConfigureAwait(false);

            var fromFileInfo = fileSystem.FileInfo.FromFileName(fromFilePath);
            var toFileInfo = fileSystem.FileInfo.FromFileName(toFilePath);

            toFileInfo.CreationTime = fromFileInfo.CreationTime;
            toFileInfo.LastWriteTime = fromFileInfo.LastWriteTime;
            toFileInfo.LastAccessTime = fromFileInfo.LastAccessTime;
            toFileInfo.Attributes = fromFileInfo.Attributes;
        }
    }
}
