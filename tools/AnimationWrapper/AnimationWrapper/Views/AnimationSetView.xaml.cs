using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AnimationWrapper
{
    /// <summary>
    /// Interaction logic for AnimationSetView.xaml
    /// </summary>
    public partial class AnimationSetView : UserControl, IAnimationSetView
    {
        public AnimationSetView(AnimationSetViewModel animationSetViewModel)
        {
            if (animationSetViewModel == null)
                throw new ArgumentNullException(nameof(animationSetViewModel));

            DataContext = animationSetViewModel;

            InitializeComponent();
        }
    }
}
