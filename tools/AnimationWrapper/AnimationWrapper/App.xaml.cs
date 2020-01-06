using Autofac;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AnimationWrapper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        IContainer container;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var builder = new ContainerBuilder();

            builder
                .RegisterType<EventAggregator>()
                .As<IEventAggregator>()
                .SingleInstance();

            builder
                .RegisterType<OpenFileDialogFileSelectionProvider>()
                .As<IFileSelectionProvider>()
                .SingleInstance();

            builder
                .RegisterType<PresentingView>()
                .AsSelf()
                .SingleInstance();

            builder
                .RegisterType<PresentingViewModel>()
                .AsSelf()
                .SingleInstance();

            builder
                .RegisterType<AnimationSetView>()
                .As<IAnimationSetView>()
                .SingleInstance();

            builder
                .RegisterType<AnimationSetViewModel>()
                .AsSelf()
                .SingleInstance();

            builder
                .RegisterType<FileSystem>()
                .As<IFileSystem>()
                .SingleInstance();

            var animationSetsDirectory =
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "animation_sets");
            builder
                .RegisterType<TwoFileAnimationSetRepository>()
                .As<IAnimationSetRepository>()
                .WithParameter("repositoryDirectory", animationSetsDirectory)
                .SingleInstance();

            builder
                .RegisterType<NewAnimationSetView>()
                .As<INewAnimationSetView>()
                .SingleInstance();

            builder
                .RegisterType<NewAnimationSetViewModel>()
                .AsSelf()
                .SingleInstance();

            builder
                .RegisterType<EditAnimationSetView>()
                .As<IEditAnimationSetView>()
                .SingleInstance();

            builder
                .RegisterType<EditAnimationSetViewModel>()
                .AsSelf()
                .SingleInstance();

            builder
                .RegisterType<NewAnimationView>()
                .As<INewAnimationView>()
                .SingleInstance();

            builder
                .RegisterType<NewAnimationViewModel>()
                .AsSelf()
                .SingleInstance();

            container = builder.Build();

            container.Resolve<PresentingView>().Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            container?.Dispose();
        }
    }
}
