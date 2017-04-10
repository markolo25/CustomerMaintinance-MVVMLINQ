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

        /// <summary>
        /// Creates a AddModViewModel
        /// </summary>
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

        /// <summary>
        /// Fills in all of the text boxes
        /// with data of the selectedCustomer
        /// </summary>
        /// <param name="selectedCustomer"></param>
        private void DisplayCustomer(Customer selectedCustomer)
        {
            try
            {
                Name = selectedCustomer.Name;
                Address = selectedCustomer.Address;
                City = selectedCustomer.City;
                SelectedState = selectedCustomer.State1.StateCode;
                Zip = selectedCustomer.ZipCode;
            }
            catch(Exception e)
            {
                ClearControls();
                MessageBox.Show("That user has been deleted");
                CloseWindow();
            }
       
        }


        /// <summary>
        /// Fills in a customer object with currently filled in values.
        /// </summary>
        /// <param name="customer"></param>
        private void PutCustomerData(Customer customer)
        {
            customer.Name = Name;
            customer.Address = Address;
            customer.City = City;
            customer.State = SelectedState;
            customer.ZipCode = Zip;
        }

        /// <summary>
        /// Clears up the textboxes in the child window
        /// </summary>
        private void ClearControls()
        {
            Name = "";
            Address = "";
            City = "";
            SelectedState = null;
            Zip = "";
        }


        /// <summary>
        /// Message Handler for the class
        /// </summary>
        /// <param name="obj"></param>
        private void recieve(AddMod obj)
        {
            windowType = obj;
            if (obj.isMod)
            {
                    Title = "Modify Customer";
                Messenger.Default.Register<Customer>(this, (cust) =>
                 {
                     customer = cust;
                 });
                if(customer == null)
                {
                    CloseWindow();
                }
                    DisplayCustomer(this.customer);

            }
            else
            {
                Title = "Add Customer";
            }
        }

        /// <summary>
        /// Handles the Cancel Button Functions
        /// </summary>
        private void CancelCommand()
        {
            CloseWindow();
        }

        /// <summary>
        /// Handles Accept Button Functions
        /// </summary>
        private void AcceptCommand()
        {
            if (windowType.isMod)
            {
                if(customer != null)
                {
                    this.PutCustomerData(customer);
                }
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
                    if (MMABooksEntity.MMABooks.Entry(customer).State == System.Data.EntityState.Detached)
                    {
                        MessageBox.Show("The Data Has Been Deleted by another");
                        ClearControls();
                    }
                    if (MMABooksEntity.MMABooks.Entry(customer).State == System.Data.EntityState.Unchanged)
                    {
                        MessageBox.Show("The Data has been modified by another");
                        DisplayCustomer(customer);
                    }
                    else
                    {
                        MessageBox.Show("Unknown Concurrency Error");
                    }
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

        /// <summary>
        /// Closes the window by sending a message to the parent class
        /// </summary>
        private void CloseWindow()
        {
            ClearControls();
            Messenger.Default.Send<Customer>(customer);
        }
    }
}
