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
    /// Interaction logic for EditAnimationSetView.xaml
    /// </summary>
    public partial class EditAnimationSetView : UserControl, IEditAnimationSetView
    {
        public EditAnimationSetView(EditAnimationSetViewModel editAnimationSetViewModel)
        {
            if (editAnimationSetViewModel == null)
                throw new ArgumentNullException(nameof(editAnimationSetViewModel));

            DataContext = editAnimationSetViewModel;

            InitializeComponent();
        }
    }
}
