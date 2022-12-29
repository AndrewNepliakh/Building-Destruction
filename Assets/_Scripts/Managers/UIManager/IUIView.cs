using System;

namespace Managers
{
    public interface IUIView
    {
        AssetsLoader AssetsLoader { get; set; }
        void Show(UIViewArguments arguments);
        void Hide(UIViewArguments arguments);
    }
}