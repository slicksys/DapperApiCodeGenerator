using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CodeGenerator.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private DalView _dalView = new DalView();
        private EntityView _entityView = new EntityView();
       // private DtoView _dtoView = new DtoView();
        private UserControl _currentView;
        private List<Environment> _environments;
        private Environment _environment;

        public MainViewModel()
        {
            DalCommand = new RelayCommand(delegate { CurrentView = _dalView; });
            EntityCommand = new RelayCommand(delegate { CurrentView = _entityView; });
           // DtoCommand = new RelayCommand(delegate { CurrentView = _dtoView; });
            SqlCommand = new RelayCommand(param => DisplaySqlNA());

            _environments = Helper.Environments;
        }

        public RelayCommand DalCommand { get; private set; }
        public RelayCommand EntityCommand { get; private set; }
        public RelayCommand DtoCommand { get; private set; }
        public RelayCommand SqlCommand { get; private set; }

        public UserControl CurrentView
        {
            get
            {
                return _dalView;
            }

            set
            {
            //    if (_currentView != value)
            //    {
            //        _currentView = value;
            //        OnPropertyChanged();
            //    }
            }
        }

        public List<Environment> Environments
        {
            get
            {
                return _environments;
            }
        }

        public Environment Environment
        {
            get
            {
                return _environment;
            }

            set
            {
                if (_environment != value)
                {
                    _environment = value;
                    ChangeProperty(_environment.Name);
                    OnPropertyChanged();
                }
            }
        }

        public void ChangeProperty(string propertyCode)
        {
            Helper.UpdateProperty(propertyCode);
            _entityView = new EntityView();
           // _dtoView = new DtoView();
            _dalView = new DalView();
            CurrentView = null;
        }

        public void DisplaySqlNA()
        {
            MessageBox.Show("SQL generation is not implemented yet.", "CCI Code Generator");
        }
    }
}