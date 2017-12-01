using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;

namespace QueryWindowExtension
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ExtensionCommand
    {

        public const string guidFirstToolWindowPackageCmdSet = "051cf9c6-cef7-4f81-9a3e-a0ae4ba95537";  // get the GUID from the .vsct file
        public const uint cmdidWindowsMedia = 0x100;
        public const int cmdidWindowsMediaOpen = 0x132;
        public const int ToolbarID = 0x1000;
        private ExtensionHostWindow window;
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("051cf9c6-cef7-4f81-9a3e-a0ae4ba95537");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private ExtensionCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.ShowToolWindow, menuCommandID);
                commandService.AddCommand(menuItem);
            }
       //     window = new ExtensionHostWindow();
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static ExtensionCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new ExtensionCommand(package);
        }

        /// <summary>
        /// Shows the tool window when the menu item is clicked.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        private void ShowToolWindow(object sender, EventArgs e)
        {
            window = (ExtensionHostWindow)this.package.FindToolWindow(typeof(ExtensionHostWindow), 0, true);
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException("Cannot create tool window");
            }

            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());

            //var mcs = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            //var toolbarbtnCmdID = new CommandID(new Guid(ExtensionCommand.guidFirstToolWindowPackageCmdSet),
            //    ExtensionCommand.cmdidWindowsMediaOpen);
            //var menuItem = new MenuCommand(new EventHandler(
            //    ButtonHandler), toolbarbtnCmdID);

            //MenuCommand menuCommand = mcs.FindCommand(menuItem.CommandID);
            //if(menuCommand==null)
            //    mcs.AddCommand(menuItem);
        }
        //private void ButtonHandler(object sender, EventArgs arguments)
        //{
        //    OpenFileDialog openFileDialog = new OpenFileDialog();
        //    DialogResult result = openFileDialog.ShowDialog();
        //    if (result == DialogResult.OK)
        //    {
        //        //window.control.MediaPlayer.Source = new System.Uri(openFileDialog.FileName);
        //        //window.control.ReportConfigFilePath = openFileDialog.FileName;
        //        try
        //        {
        //            window.control.ReportConfigFile.Load(openFileDialog.FileName);
        //            window.control.ParseConfigFile();
        //        }
        //        catch (XmlException ex)
        //        {
        //            //Do nothing file is not an xml file
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("File is not in RightAngle compatible format, Parsing Error Occured");
        //        }
               
        //    }
        //}
    }
}
