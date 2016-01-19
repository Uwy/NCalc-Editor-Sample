using GalaSoft.MvvmLight;

namespace ScriptSample.ViewModel
{
    public class ParameterViewModel : ViewModelBase
    {
        private string label;

        private string value;

        public ParameterViewModel()
        {
        }

        public string Label
        {
            get
            {
                return this.label;
            }
            set
            {
                this.label = value;
                this.RaisePropertyChanged();
            }
        }

        public string Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
                this.RaisePropertyChanged();
            }
        }
    }
}