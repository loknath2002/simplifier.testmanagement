using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Simplifier.TestManagement.ViewModels;
using SukiUI.Toasts;

namespace Simplifier.TestManagement.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public void SetToastManager(ISukiToastManager toastManager)
    {
        ToastHost.Manager = toastManager;
    }
}