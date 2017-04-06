using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using System;
using CustomerMaintinence_MVVM.Model;
using System.Windows;
using System.Linq;
using System.Data.Entity.Infrastructure;
using GalaSoft.MvvmLight.Messaging;
using CustomerMaintinence_MVVM.View;

namespace CustomerMaintinence_MVVM.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {

        Customer selectedCustomer;
        private int _custID;
        AddModWIndow _addmod;
        private string _name, _address, _city, _state, _zip;
        public ICommand AddButton { get; private set; }
        public ICommand ModifyButton { get; private set; }
        public ICommand DeleteButton { get; private set; }
        public ICommand ExitButton { get; private set; }
        public ICommand GetCustomerButton { get; private set; }
        public int CustomerID
        {
            get
            {
                return _custID;
            }
            set
            {
                _custID = value;
                this.RaisePropertyChanged();
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
        public string State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                this.RaisePropertyChanged(() => State);
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
        public Boolean selected = false;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            GetCustomerButton = new RelayCommand(GetCustomerCommand);
            AddButton = new RelayCommand(AddCommand);
            ModifyButton = new RelayCommand(ModCommand);
            DeleteButton = new RelayCommand(DeleteCommand);
            ExitButton = new RelayCommand(ExitCommand);


        }

        /// <summary>
        /// Fills in the properties bound the the view
        /// with their respective properties in the Customer Class
        /// </summary>
        private void DisplayCustomer()
        {
            Name = selectedCustomer.Name;
            Address = selectedCustomer.Address;
            City = selectedCustomer.City;
            State = selectedCustomer.State1.StateName;
            Zip = selectedCustomer.ZipCode;
            selected = true;
        }

        /// <summary>
        /// Cleans up the properties the text boxes are bound to.
        /// </summary>
        private void ClearControls()
        {
            CustomerID = 0;
            Name = "";
            Address = "";
            City = "";
            State = "";
            Zip = "";
            selected = false;
        }

        /// <summary>
        /// Queries the database for a customer using an ID as a
        /// Primary Key
        /// </summary>
        private void GetCustomerCommand()
        {
            try
            {
                // Code a query to retrieve the selected customer
                // and store the Customer object in the class variable.
                var customerQuery =
                    from customer in MMABooksEntity.MMABooks.Customers
                    where customer.CustomerID == CustomerID
                    select customer;

                if (customerQuery.Count() > 0)
                {
                    selectedCustomer = customerQuery.Single();
                }

                if (selectedCustomer == null)
                {
                    MessageBox.Show("No customer found with this ID. " +
                        "Please try again.", "Customer Not Found");
                }
                else
                {
                    //  If the customer is found, add code to the GetCustomer method that checks if the State object
                    // has been loaded and that loads if it hasn't.
                    if (!MMABooksEntity.MMABooks.Entry(selectedCustomer).Reference("State1").IsLoaded)
                    {
                        MMABooksEntity.MMABooks.Entry(selectedCustomer).Reference("State1").Load();
                    }

                    DisplayCustomer();

                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                ex.Entries.Single().Reload();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().ToString());
            }



        }

        /// <summary>
        /// Closes the program
        /// </summary>
        private void ExitCommand()
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Deletes the currently selected Customer.
        /// </summary>
        private void DeleteCommand()
        {
            if (selected)
            {
                MessageBoxResult result = MessageBox.Show("Delete " + selectedCustomer.Name + "?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Mark the row for deletion.
                        // Update the database.
                        MMABooksEntity.MMABooks.Customers.Remove(selectedCustomer);
                        MMABooksEntity.MMABooks.SaveChanges();
                        this.ClearControls();

                    }
                    // Add concurrency error handling.
                    // Place the catch block before the one for a generic exception.
                    catch (DbUpdateConcurrencyException ex)
                    {
                        ex.Entries.Single().Reload();
                        if(MMABooksEntity.MMABooks.Entry(selectedCustomer).State == System.Data.EntityState.Detached)
                        {
                            MessageBox.Show("The Data Has Been Deleted by another");
                            ClearControls();
                        }

                    }

                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, ex.GetType().ToString());
                    }
                }
            }
            else
            {
                MessageBox.Show("Select a customer first");
            }
        }

        /// <summary>
        /// Calls a Child WIndow in order to modify the currently selected Customer
        /// </summary>
        private void ModCommand()
        {
            if (selected)
            {
                _addmod = new AddModWIndow();
                registerCloseWindow();
                _addmod.Show();
                Messenger.Default.Send<AddMod>(new AddMod(true, this.CustomerID));
            }
            else
            {
                MessageBox.Show("Select a customer first");
            }
        }


        /// <summary>
        /// calls a child window to create a new Customer
        /// </summary>
        private void AddCommand()
        {
            _addmod = new AddModWIndow();
            registerCloseWindow();
            Messenger.Default.Send<AddMod>(new AddMod(false, this.CustomerID));
            _addmod.Show();
        }

        /// <summary>
        /// Handler class for the child window, that closes the child window and displays data
        /// from the results of the actions performed in the child window
        /// </summary>
        private void registerCloseWindow()
        {
            Messenger.Default.Register<Customer>(this, (nm) =>
            {
                _addmod.Close();
                if (nm != null)
                {
                    CustomerID = nm.CustomerID;
                    selectedCustomer = nm;
                    DisplayCustomer();
                }
                

            });
            
        }
    }
}