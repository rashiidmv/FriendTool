namespace QueryWindowExtension
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using System.ComponentModel.Design;
    using Microsoft.VisualStudio.Shell.Interop;
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("2b008ba6-0b84-4eb7-bb24-91b50a86fef2")]
    public class ExtensionHostWindow : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionHostWindow"/> class.
        /// </summary>
        public ExtensionHostWindow() : base(null)
        {
            this.Caption = "Query Window";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.


            Bootstrapper b = new Bootstrapper();
            b.Run();
            this.Content = b.GetShell();
            control = (Shell)b.GetShell();


            //this.ToolBar = new CommandID(new Guid(ExtensionCommand.guidFirstToolWindowPackageCmdSet),
            //ExtensionCommand.ToolbarID);
            //this.ToolBarLocation = (int)VSTWT_LOCATION.VSTWT_TOP;
        }

        public Shell control;
    }
}
