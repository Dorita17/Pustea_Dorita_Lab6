using AutoLotModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pustea_Dorita_Lab6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    enum ActionState
    {
        New, Edit, Delete, Nothing
    }
    public partial class MainWindow : Window
    {
        //using AutoLotModel
        ActionState action = ActionState.Nothing;
        AutoLotEntitesModel ctx = new AutoLotEntitesModel();
        CollectionViewSource customerViewSource;
        CollectionViewSource inventoryViewSource;
        CollectionViewSource customerOrdersViewSource;

        //Binding objects - Customers
        Binding txtFirstNameBinding = new Binding();
        Binding txtLastNameBinding = new Binding();

        //Binding objects - Inventory
        Binding txtColorBinding = new Binding();
        Binding txtMakeBinding = new Binding();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            // Set the binding - Customers
            txtFirstNameBinding.Path = new PropertyPath("FirstName");
            txtLastNameBinding.Path = new PropertyPath("LastName");

            firstNameTextBox.SetBinding(TextBox.TextProperty, txtFirstNameBinding);
            lastNameTextBox.SetBinding(TextBox.TextProperty, txtLastNameBinding);

            //Set the binding - Inventory
            txtColorBinding.Path = new PropertyPath("Color");
            txtMakeBinding.Path = new PropertyPath("Make");

            colorTextBox.SetBinding(TextBox.TextProperty, txtColorBinding);
            makeTextBox.SetBinding(TextBox.TextProperty, txtMakeBinding);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            customerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            inventoryViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("inventoryViewSource")));
            customerOrdersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerOrdersViewSource")));
            customerViewSource.Source = ctx.Customers.Local;
            inventoryViewSource.Source = ctx.Inventories.Local;
            // customerOrdersViewSource.Source = ctx.Orders.Local;
            ctx.Customers.Load();
            ctx.Inventories.Load();
            ctx.Orders.Load();

            cmbCustomers.ItemsSource = ctx.Customers.Local;
            // cmbCustomers.DisplayMemberPath = "FirstName";
            cmbCustomers.SelectedValuePath = "CustId";

            cmbInventory.ItemsSource = ctx.Inventories.Local;
            // cmbInventory.DisplayMemberPath = "Make";
            cmbInventory.SelectedValuePath = "CarId";

            BindDataGrid();
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;

            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnPrevious.IsEnabled = false;
            btnNext.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;

            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);

            firstNameTextBox.Text = "";
            lastNameTextBox.Text = "";

            Keyboard.Focus(firstNameTextBox);
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;

            string tempFirstName = firstNameTextBox.Text.ToString();
            string tempLastName = lastNameTextBox.Text.ToString();

            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnPrevious.IsEnabled = false;
            btnNext.IsEnabled = false;
            btnCancel.IsEnabled = true;
            btnSave.IsEnabled = true;

            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);

            SetValidationBinding();

            firstNameTextBox.Text = tempFirstName;
            lastNameTextBox.Text = tempLastName;

            Keyboard.Focus(firstNameTextBox);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;

            string tempFirstName = firstNameTextBox.Text.ToString();
            string tempLastName = lastNameTextBox.Text.ToString();

            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnPrevious.IsEnabled = false;
            btnNext.IsEnabled = false;
            btnCancel.IsEnabled = true;
            btnSave.IsEnabled = true;

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);

            firstNameTextBox.Text = tempFirstName;
            lastNameTextBox.Text = tempLastName;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            Customer customer = null;
            if (action == ActionState.New)
            {
                try
                {
                    //instantiem Customer entity
                    customer = new Customer()
                    {
                        FirstName = firstNameTextBox.Text.Trim(),
                        LastName = lastNameTextBox.Text.Trim()
                    };

                    //adaugam entitatea nou creata in context
                    ctx.Customers.Add(customer);
                    customerViewSource.View.Refresh();

                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                btnNew.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnNext.IsEnabled = true;
                btnPrevious.IsEnabled = true;
                btnCancel.IsEnabled = false;
                btnSave.IsEnabled = false;

                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;

                firstNameTextBox.SetBinding(TextBox.TextProperty, txtFirstNameBinding);
                lastNameTextBox.SetBinding(TextBox.TextProperty, txtLastNameBinding);
            }

            else if (action == ActionState.Edit)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    customer.FirstName = firstNameTextBox.Text.Trim();
                    customer.LastName = lastNameTextBox.Text.Trim();

                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                customerViewSource.View.Refresh();

                //pozitionarea pe item-ul curent
                customerViewSource.View.MoveCurrentTo(customer);

                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnPrevious.IsEnabled = true;
                btnNew.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;

                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;

                firstNameTextBox.SetBinding(TextBox.TextProperty, txtFirstNameBinding);
                lastNameTextBox.SetBinding(TextBox.TextProperty, txtLastNameBinding);
            }

            else if (action == ActionState.Delete)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    ctx.Customers.Remove(customer);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                customerViewSource.View.Refresh();

                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnPrevious.IsEnabled = true;
                btnNext.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;

                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;

                firstNameTextBox.SetBinding(TextBox.TextProperty, txtFirstNameBinding);
                lastNameTextBox.SetBinding(TextBox.TextProperty, txtLastNameBinding);
            }

            SetValidationBinding();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;

            btnNew.IsEnabled = true;
            btnEdit.IsEnabled = true;
            btnDelete.IsEnabled = true;
            btnPrevious.IsEnabled = true;
            btnNext.IsEnabled = true;
            btnCancel.IsEnabled = false;
            btnSave.IsEnabled = false;

            custIdTextBox.IsEnabled = false;
            firstNameTextBox.IsEnabled = false;
            lastNameTextBox.IsEnabled = false;

            firstNameTextBox.SetBinding(TextBox.TextProperty, txtFirstNameBinding);
            lastNameTextBox.SetBinding(TextBox.TextProperty, txtLastNameBinding);
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToNext();
        }

        private void btnNewI_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;

            btnNewI.IsEnabled = false;
            btnEditI.IsEnabled = false;
            btnDeleteI.IsEnabled = false;
            btnPreviousI.IsEnabled = false;
            btnNextI.IsEnabled = false;
            btnSaveI.IsEnabled = true;
            btnCancelI.IsEnabled = true;

            colorTextBox.IsEnabled = true;
            makeTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);

            firstNameTextBox.Text = "";
            lastNameTextBox.Text = "";

            Keyboard.Focus(colorTextBox);
        }

        private void btnEditI_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;

            string tempColor = colorTextBox.Text.ToString();
            string tempMake = makeTextBox.Text.ToString();

            btnNewI.IsEnabled = false;
            btnEditI.IsEnabled = false;
            btnDeleteI.IsEnabled = false;
            btnPreviousI.IsEnabled = false;
            btnNextI.IsEnabled = false;
            btnCancelI.IsEnabled = true;
            btnSaveI.IsEnabled = true;

            colorTextBox.IsEnabled = true;
            makeTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);

            colorTextBox.Text = tempColor;
            makeTextBox.Text = tempMake;

            Keyboard.Focus(colorTextBox);
        }

        private void btnDeleteI_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;

            string tempColor = colorTextBox.Text.ToString();
            string tempMake = makeTextBox.Text.ToString();

            btnNewI.IsEnabled = false;
            btnEditI.IsEnabled = false;
            btnDeleteI.IsEnabled = false;
            btnPreviousI.IsEnabled = false;
            btnNextI.IsEnabled = false;
            btnCancelI.IsEnabled = true;
            btnSaveI.IsEnabled = true;

            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);

            colorTextBox.Text = tempColor;
            makeTextBox.Text = tempMake;
        }

        private void btnSaveI_Click(object sender, RoutedEventArgs e)
        {
            Inventory inventory = null;
            if (action == ActionState.New)
            {
                try
                {
                    //instantiem Customer entity
                    inventory = new Inventory()
                    {
                        Color = colorTextBox.Text.Trim(),
                        Make = makeTextBox.Text.Trim()
                    };

                    //adaugam entitatea nou creata in context
                    ctx.Inventories.Add(inventory);
                    inventoryViewSource.View.Refresh();

                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                btnNewI.IsEnabled = true;
                btnDeleteI.IsEnabled = true;
                btnEditI.IsEnabled = true;
                btnNextI.IsEnabled = true;
                btnPreviousI.IsEnabled = true;
                btnCancelI.IsEnabled = false;
                btnSaveI.IsEnabled = false;

                carIdTextBox.IsEnabled = false;
                colorTextBox.IsEnabled = false;
                makeTextBox.IsEnabled = false;

                colorTextBox.SetBinding(TextBox.TextProperty, txtColorBinding);
                makeTextBox.SetBinding(TextBox.TextProperty, txtMakeBinding);
            }

            else if (action == ActionState.Edit)
            {
                try
                {
                    inventory = (Inventory)inventoryDataGrid.SelectedItem;
                    inventory.Color = colorTextBox.Text.Trim();
                    inventory.Make = makeTextBox.Text.Trim();

                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                inventoryViewSource.View.Refresh();

                //pozitionarea pe item-ul curent
                customerViewSource.View.MoveCurrentTo(inventory);

                btnNewI.IsEnabled = true;
                btnEditI.IsEnabled = true;
                btnDeleteI.IsEnabled = true;
                btnPreviousI.IsEnabled = true;
                btnNextI.IsEnabled = true;
                btnSaveI.IsEnabled = false;
                btnCancelI.IsEnabled = false;

                carIdTextBox.IsEnabled = false;
                colorTextBox.IsEnabled = false;
                makeTextBox.IsEnabled = false;

                colorTextBox.SetBinding(TextBox.TextProperty, txtColorBinding);
                makeTextBox.SetBinding(TextBox.TextProperty, txtMakeBinding);
            }

            else if (action == ActionState.Delete)
            {
                try
                {
                    inventory = (Inventory)inventoryDataGrid.SelectedItem;
                    ctx.Inventories.Remove(inventory);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                inventoryViewSource.View.Refresh();

                btnNewI.IsEnabled = true;
                btnEditI.IsEnabled = true;
                btnDeleteI.IsEnabled = true;
                btnPreviousI.IsEnabled = true;
                btnNextI.IsEnabled = true;
                btnSaveI.IsEnabled = false;
                btnCancelI.IsEnabled = false;

                carIdTextBox.IsEnabled = false;
                colorTextBox.IsEnabled = false;
                makeTextBox.IsEnabled = false;

                colorTextBox.SetBinding(TextBox.TextProperty, txtColorBinding);
                makeTextBox.SetBinding(TextBox.TextProperty, txtMakeBinding);
            }
        }

        private void btnCancelI_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;

            btnNewI.IsEnabled = true;
            btnEditI.IsEnabled = true;
            btnDeleteI.IsEnabled = true;
            btnPreviousI.IsEnabled = true;
            btnNextI.IsEnabled = true;
            btnCancelI.IsEnabled = false;
            btnSaveI.IsEnabled = false;

            carIdTextBox.IsEnabled = false;
            colorTextBox.IsEnabled = false;
            makeTextBox.IsEnabled = false;

            colorTextBox.SetBinding(TextBox.TextProperty, txtColorBinding);
            makeTextBox.SetBinding(TextBox.TextProperty, txtMakeBinding);
        }

        private void btnPreviousI_Click(object sender, RoutedEventArgs e)
        {
            inventoryViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNextI_Click(object sender, RoutedEventArgs e)
        {
            inventoryViewSource.View.MoveCurrentToNext();
        }

        private void BindDataGrid()
        {
            var queryOrder = from ord in ctx.Orders
                             join cust in ctx.Customers on ord.CustId equals
                             cust.CustId
                             join inv in ctx.Inventories on ord.CarId
                 equals inv.CarId
                             select new
                             {
                                 ord.OrderId,
                                 ord.CarId,
                                 ord.CustId,
                                 cust.FirstName,
                                 cust.LastName,
                                 inv.Make,
                                 inv.Color
                             };
            customerOrdersViewSource.Source = queryOrder.ToList();
        }

        private void btnNewO_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnEditO_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDeleteO_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSaveO_Click(object sender, RoutedEventArgs e)
        {
            Order order = null;
            if (action == ActionState.New)
            {
                try
                {
                    Customer customer = (Customer)cmbCustomers.SelectedItem;
                    Inventory inventory = (Inventory)cmbInventory.SelectedItem;

                    //instantiem Order entity
                    order = new Order()
                    {
                        CustId = customer.CustId,
                        CarId = inventory.CarId
                    };

                    //adaugam entitatea nou creata in context
                    ctx.Orders.Add(order);
                    customerOrdersViewSource.View.Refresh();

                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                btnNewO.IsEnabled = true;
                btnDeleteO.IsEnabled = true;
                btnEditO.IsEnabled = true;
                btnNextO.IsEnabled = true;
                btnPreviousO.IsEnabled = true;
                btnCancelO.IsEnabled = false;
                btnSaveO.IsEnabled = false;
            }

            else if (action == ActionState.Edit)
            {
                dynamic selectedOrder = ordersDataGrid.SelectedItem;
                try
                {
                    int curr_id = selectedOrder.OrderId;

                    var editedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);

                    if (editedOrder != null)
                    {
                        editedOrder.OrderId = Int32.Parse(cmbCustomers.SelectedValue.ToString());
                        editedOrder.CarId = Convert.ToInt32(cmbInventory.SelectedValue.ToString());

                        //salvam modificarile
                        ctx.SaveChanges();
                    }

                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                BindDataGrid();
                //pozitionarea pe item-ul curent
                customerViewSource.View.MoveCurrentTo(selectedOrder);
            }

            else if (action == ActionState.Delete)
            {
                try
                {
                    dynamic selectedOrder = ordersDataGrid.SelectedItem;

                    int curr_id = selectedOrder.OrderId;
                    var deletedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);

                    if (deletedOrder != null)
                    {
                        ctx.Orders.Remove(deletedOrder);
                        ctx.SaveChanges();
                        MessageBox.Show("Order deleted succesfully!", "Message");
                        BindDataGrid();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnCancelO_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnPreviousO_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnNextO_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SetValidationBinding()
        {
            Binding firstNameValidationBinding = new Binding();
            firstNameValidationBinding.Source = customerViewSource;
            firstNameValidationBinding.Path = new PropertyPath("FirstName");
            firstNameValidationBinding.NotifyOnValidationError = true;
            firstNameValidationBinding.Mode = BindingMode.TwoWay;
            firstNameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //string required
            firstNameValidationBinding.ValidationRules.Add(new StringNotEmpty());
            firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameValidationBinding);
            Binding lastNameValidationBinding = new Binding();
            lastNameValidationBinding.Source = customerViewSource;
            lastNameValidationBinding.Path = new PropertyPath("LastName");
            lastNameValidationBinding.NotifyOnValidationError = true;
            lastNameValidationBinding.Mode = BindingMode.TwoWay;
            lastNameValidationBinding.UpdateSourceTrigger =
           UpdateSourceTrigger.PropertyChanged;
            //string min length validator
            lastNameValidationBinding.ValidationRules.Add(new StringMinLengthValidator());
            lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameValidationBinding); //setare binding nou
        }
    }
}
