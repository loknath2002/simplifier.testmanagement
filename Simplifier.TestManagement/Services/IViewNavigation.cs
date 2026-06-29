// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */
using Simplifier.TestManagement.ViewModels;
namespace Simplifier.TestManagement.Services;

public interface IViewNavigation
{
    void Navigate(ViewModelBase? theFrom, ViewModelBase theTo);
    void NavigateBack(ViewModelBase theFrom);
}