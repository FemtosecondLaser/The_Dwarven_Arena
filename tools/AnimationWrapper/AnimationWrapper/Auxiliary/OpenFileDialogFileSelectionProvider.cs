using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationWrapper
{
    public class OpenFileDialogFileSelectionProvider : IFileSelectionProvider
    {
        public string SelectPngFile()
        {
            var dialog = new OpenFileDialog();
            dialog.DefaultExt = ".png";
            dialog.Filter = "Image files (.png)|*.png";

            var dialogResult = dialog.ShowDialog();

            if (dialogResult.HasValue && dialogResult.Value)
                return dialog.FileName;

            return null;
        }
    }
}
