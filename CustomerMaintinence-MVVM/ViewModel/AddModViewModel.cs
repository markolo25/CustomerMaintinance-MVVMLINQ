using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CustomerMaintinence_MVVM.Model;
using GalaSoft.MvvmLight.Messaging;
using System.Windows;
using System.Data.Entity.Infrastructure;

namespace CustomerMaintinence_MVVM.ViewModel
{
    public class AddModViewModel : ViewModelBase
    {
        private string _name, _address, _city, _zip, _title;
        private string _selectedState;
        List<State> _states;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                this.RaisePropertyChanged(() => Title);
            }
        }
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                this.RaisePropertyChanged(() => Name);
            }
        }
        public string Address
        {
            get
            {
                return _address;
            }
            set
            {
                _address = value;
                this.RaisePropertyChanged(() => Address);
            }
        }
        public string City
        {
            get
            {
                return _city;
            }
            set
            {
                _city = value;
                this.RaisePropertyChanged(() => City);
            }
        }
        public string SelectedState
        {
            get
            {
                return _selectedState;
            }
            set
            {
                _selectedState = value;
                this.RaisePropertyChanged(() => SelectedState);
            }
        }
        public List<State> States
        {
            get
            {
                return _states;
            }
            set
            {
                _states = value;
                this.RaisePropertyChanged(() => States);
            }
        }
        public string Zip
        {
            get
            {
                return _zip;
            }
            set
            {
                _zip = value;
                this.RaisePropertyChanged(() => Zip);
            }
        }
        public ICommand AcceptButton { get; set; }
        public ICommand CancelButton { get; set; }
        public AddMod windowType;
        public Customer customer;

        public AddModViewModel()
        {
            Messenger.Default.Register<AddMod>(this, recieve);
            AcceptButton = new RelayCommand(AcceptCommand);
            CancelButton = new RelayCommand(CancelCommand);
            var stateQuery = from state in MMABooksEntity.MMABooks.States
                             orderby state.StateName
                             select state;
            States = (stateQuery.ToList());
        }

        private void DisplayCustomer(Customer selectedCustomer)
        {
            Name = selectedCustomer.Name;
            Address = selectedCustomer.Address;
            City = selectedCustomer.City;
            SelectedState = selectedCustomer.State1.StateCode;
            Zip = selectedCustomer.ZipCode;
        }


        private void PutCustomerData(Customer customer)
        {
            customer.Name = Name;
            customer.Address = Address;
            customer.City = City;
            customer.State = SelectedState;
            customer.ZipCode = Zip;
        }

        private void ClearControls()
        {
            Name = "";
            Address = "";
            City = "";
            SelectedState = null;
            Zip = "";
        }


        private void recieve(AddMod obj)
        {
            windowType = obj;
            if (obj.isMod)
            {
                Title = "Modify Customer";
                var customerQuery =
                    from customer in MMABooksEntity.MMABooks.Customers
                    where customer.CustomerID == obj.CustomerID
                    select customer;

                customer = customerQuery.Single();
                DisplayCustomer(this.customer);

            }
            else
            {
                Title = "Add Customer";
            }
        }

        private void CancelCommand()
        {
            CloseWindow();
        }

        private void AcceptCommand()
        {
            if (windowType.isMod)
            {
                this.PutCustomerData(customer);
                try
                {
                    // Update the database.
                    MMABooksEntity.MMABooks.SaveChanges();
                }

                // Add concurrency error handling.
                // Place the catch block before the one for a generic exception.
                catch (DbUpdateConcurrencyException ex)
                {
                    ex.Entries.Single().Reload();
                }


                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().ToString());
                    customer = null;
                }


            }
            else
            {
                customer = new Customer();
                this.PutCustomerData(customer);

                // Add the new vendor to the collection of vendors.
                MMABooksEntity.MMABooks.Customers.Add(customer);

                try
                {
                    // Update the database.
                    MMABooksEntity.MMABooks.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    ex.Entries.Single().Reload();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().ToString());
                    customer = null;
                }
            }
            CloseWindow();
            return;
        }

        private void CloseWindow()
        {
            ClearControls();
            Messenger.Default.Send<Customer>(customer);
        }
    }
}
