using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using BnPBank.ViewModels;
using BnPBank.Views;

namespace BnPBank
{
    public class ViewLocator : IDataTemplate
    {
        public IControl Build(object data)
        {
            var name = data.GetType().FullName!.Replace("ViewModel", "View");
            var type = Type.GetType(name);

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }

            throw new InvalidOperationException($"View not found for ViewModel: {data.GetType().Name}");
        }

        public bool Match(object data)
        {
            return data is ViewModelBase;
        }
    }
}