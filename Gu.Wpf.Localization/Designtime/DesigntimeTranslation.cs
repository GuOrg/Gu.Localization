﻿namespace Gu.Wpf.Localization
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows.Data;

    using Gu.Localization;

    internal class DesigntimeTranslation : ITranslation
    {
        private readonly ITranslation _translation;
        private readonly BindingExpression _bindingExpression;

        public DesigntimeTranslation(ITranslation translation, BindingExpression bindingExpression)
        {
            _translation = translation;
            _bindingExpression = bindingExpression;
            _translation.PropertyChanged += OnTranslationChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Translated => _translation.Translated;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        private void OnTranslationChanged(object sender, PropertyChangedEventArgs e)
        {
            Debugger.Break();
            if (e.PropertyName != nameof(_translation.Translated))
            {
                return;
            }

            _bindingExpression?.UpdateTarget();
            OnPropertyChanged(e);
        }
    }
}