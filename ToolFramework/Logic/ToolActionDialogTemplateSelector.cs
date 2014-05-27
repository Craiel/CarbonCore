﻿namespace CarbonCore.ToolFramework.Logic
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    using CarbonCore.ToolFramework.Contracts;

    public class ToolActionDialogTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ProgressTemplate { get; set; }
        public DataTemplate SplashTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
            {
                return base.SelectTemplate(null, container);
            }

            var typed = item as IToolActionDialogViewModel;
            if (typed == null)
            {
                throw new ArgumentException("Expected dialog view model");
            }

            switch (typed.DisplayMode)
            {
                case ToolActionDisplayMode.Progress:
                    {
                        return this.ProgressTemplate;
                    }

                case ToolActionDisplayMode.Splash:
                    {
                        return this.SplashTemplate;
                    }

                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }
    }
}
