using System;
using System.Collections.Generic;
using System.Drawing;
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
        public event AnimationSetDeletedEventHandler AnimationSetDeleted;

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

        public async Task CreateAnimationSetAsync(string spriteSheetFilePath, string animationSetName)
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

        public async Task DeleteAnimationSetAsync(string animationSetName)
        {
            if (animationSetName == null)
                throw new ArgumentNullException(nameof(animationSetName));

            var animationSetFilePath =
                fileSystem.Path.Combine(repositoryDirectory, $"{animationSetName}{animationSetFileExtension}");

            using (var animationSetFileStream =
                fileSystem.FileStream.Create(
                    animationSetFilePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read
                ))
            using (var animationSetFileStreamReader =
                new StreamReader(
                    animationSetFileStream,
                    leaveOpen: true
                    ))
            {
                string line;
                while ((line = await animationSetFileStreamReader.ReadLineAsync()) != null)
                {
                    if (line.StartsWith("sprite_sheet_file_name"))
                    {
                        string spriteSheetFileName = line.Substring(line.LastIndexOf(' ') + 1);
                        fileSystem.File.Delete(
                            fileSystem.Path.Combine(repositoryDirectory, spriteSheetFileName)
                            );
                        break;
                    }
                }
            }

            fileSystem.File.Delete(animationSetFilePath);

            AnimationSetDeleted?.Invoke(new AnimationSetDeletedEventArgs(animationSetName));
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

        public async Task<AnimationSet> GetAnimationSetAsync(string animationSetName)
        {
            if (animationSetName == null)
                throw new ArgumentNullException(nameof(animationSetName));

            var animationSetFilePath =
                fileSystem.Path.Combine(repositoryDirectory, $"{animationSetName}{animationSetFileExtension}");

            var animationSet = new AnimationSet();

            using (var animationSetReader = new StreamReader(animationSetFilePath))
            {
                string line;
                while ((line = await animationSetReader.ReadLineAsync()) != null)
                {
                    var lineComponents = line.Split(' ');

                    switch (lineComponents[0])
                    {
                        case "sprite_sheet_file_name":
                            animationSet.SpriteSheetFileName = lineComponents[2];
                            break;

                        case "animation_start":
                            animationSet.Animations.Add(
                                await GetAnimationAsync(animationSetReader).ConfigureAwait(false)
                                );
                            break;

                        default:
                            break;
                    }
                }
            }

            return animationSet;
        }

        private async Task<Animation> GetAnimationAsync(StreamReader animationSetReader)
        {
            if (animationSetReader == null)
                throw new ArgumentNullException(nameof(animationSetReader));

            var animation = new Animation();

            string line;
            while ((line = await animationSetReader.ReadLineAsync()) != null)
            {
                var lineComponents = line.Split(' ');

                switch (lineComponents[0])
                {
                    case "name":
                        animation.Name = lineComponents[1];
                        break;

                    case "frame_start":
                        animation.Frames.Add(
                            await GetFrameAsync(animationSetReader).ConfigureAwait(false)
                            );
                        break;

                    case "animation_end":
                        goto exit;

                    default:
                        break;
                }
            }

        exit:
            return animation;
        }

        private async Task<Frame> GetFrameAsync(StreamReader animationSetReader)
        {
            if (animationSetReader == null)
                throw new ArgumentNullException(nameof(animationSetReader));

            var frame = new Frame();

            string line;
            while ((line = await animationSetReader.ReadLineAsync()) != null)
            {
                var lineComponents = line.Split(' ');

                switch (lineComponents[0])
                {
                    case "clip_zone":
                        Rectangle clipZone = new Rectangle();
                        clipZone.X = int.Parse(lineComponents[1]);
                        clipZone.Y = int.Parse(lineComponents[2]);
                        clipZone.Width = int.Parse(lineComponents[3]);
                        clipZone.Height = int.Parse(lineComponents[4]);
                        frame.ClipZone = clipZone;
                        break;

                    case "duration":
                        frame.Duration = float.Parse(lineComponents[1]);
                        break;

                    case "offset":
                        Point offset = new Point();
                        offset.X = int.Parse(lineComponents[1]);
                        offset.Y = int.Parse(lineComponents[2]);
                        frame.Offset = offset;
                        break;

                    case "is_damaging":
                        frame.IsDamaging = int.Parse(lineComponents[1]) != 0;
                        break;

                    case "hitbox":
                        Rectangle hitbox = new Rectangle();
                        hitbox.X = int.Parse(lineComponents[1]);
                        hitbox.Y = int.Parse(lineComponents[2]);
                        hitbox.Width = int.Parse(lineComponents[3]);
                        hitbox.Height = int.Parse(lineComponents[4]);
                        frame.Hitbox = hitbox;
                        break;

                    case "damage":
                        frame.Damage = int.Parse(lineComponents[1]);
                        break;

                    case "frame_end":
                        goto exit;

                    default:
                        break;
                }
            }

        exit:
            return frame;
        }
    }
}
