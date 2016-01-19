using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using NCalc;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml;

namespace ScriptSample.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private string scriptError;
        private object scriptResult;

        public MainViewModel()
        {
            this.RemoveParameter = new RelayCommand<object>(this.OnRemoveParameter);
            this.ExecuteScript = new RelayCommand(this.ExecuteScriptAction, this.CanExecuteScript);
            this.Script.TextChanged += this.OnScriptTextChanged;
            if (!this.IsInDesignMode)
            {
                using (var ncalcSyntaxReader = new XmlTextReader(Path.Combine("Resources", "NCalc-Mode.xshd")))
                {
                    this.SyntaxHighlight = HighlightingLoader.Load(ncalcSyntaxReader, HighlightingManager.Instance);
                }
            }
        }

        public RelayCommand ExecuteScript { get; }

        public ObservableCollection<ParameterViewModel> Parameters { get; } = new ObservableCollection<ParameterViewModel>();

        public RelayCommand<object> RemoveParameter { get; }

        public TextDocument Script { get; } = new TextDocument();

        public string ScriptError
        {
            get
            {
                return this.scriptError;
            }
            private set
            {
                this.scriptError = value;
                this.RaisePropertyChanged();
            }
        }

        public object ScriptResult
        {
            get
            {
                return this.scriptResult;
            }
            private set
            {
                this.scriptResult = value;
                this.RaisePropertyChanged();
            }
        }

        public IHighlightingDefinition SyntaxHighlight { get; }

        private bool CanExecuteScript()
        {
            if (string.IsNullOrWhiteSpace(this.Script.Text))
            {
                return false;
            }
            var expression = new Expression(this.Script.Text);
            if (expression.HasErrors())
            {
                this.ScriptError = expression.Error;
                return false;
            }
            else
            {
                this.ScriptError = string.Empty;
                return true;
            }
        }

        private void ExecuteScriptAction()
        {
            var expression = new Expression(this.Script.Text);

            if (expression.HasErrors())
            {
                this.ScriptResult = string.Empty;
                this.ScriptError = expression.Error;
            }
            else
            {
                try
                {
                    foreach (var parameter in this.Parameters.Where(x => !string.IsNullOrWhiteSpace(x.Label)))
                    {
                        expression.Parameters.Add(parameter.Label, parameter.Value);
                    }
                    this.ScriptResult = expression.Evaluate();
                    this.ScriptError = string.Empty;
                }
                catch (Exception ex)
                {
                    this.ScriptError = ex.Message;
                    this.ScriptResult = string.Empty;
                }
            }
        }

        private void OnRemoveParameter(object obj)
        {
            var casted = obj as ParameterViewModel;
            if(casted != null)
            {
                this.Parameters.Remove(casted);

            }
        }

        private void OnScriptTextChanged(object sender, EventArgs e)
        {
            this.ExecuteScript.RaiseCanExecuteChanged();
        }
    }
}