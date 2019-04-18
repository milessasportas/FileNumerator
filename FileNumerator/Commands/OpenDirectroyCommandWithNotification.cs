using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfHelperLibrary.Commands;

namespace FileNumerator.Commands
{
    /// <summary>
    /// Adds the DirectorySelected event
    /// </summary>
    public class OpenDirectroyCommandWithNotification : OpenDirectoryCommand
    {
        public override string SelectedDirectory
        {
            set
            {
                if(SelectedDirectory != value)
                {
                    base.SelectedDirectory = value;
                    DirectorySelected.Invoke(this, value);
                }
            }
        }

        /// <summary>
        /// gets fired when the Directory gets selected by the user
        /// </summary>
        public event EventHandler<string> DirectorySelected;
    }
}
