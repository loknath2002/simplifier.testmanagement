using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Simplifier.TestManagement.ViewModels;
using SukiUI.Dialogs;
using SukiUI.Toasts;

namespace Simplifier.TestManagement.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public MainWindow(ISukiToastManager toastManager, ISukiDialogManager theDialogManager) : this()
    {
        ToastHost.Manager = toastManager;
        DialogHost.Manager = theDialogManager;
    }
}