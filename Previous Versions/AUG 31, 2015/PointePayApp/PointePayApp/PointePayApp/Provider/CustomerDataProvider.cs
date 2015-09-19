using PointePayApp.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PointePayApp.Model;
using SQLite;
using Windows.Storage;
using Windows.ApplicationModel;

namespace PointePayApp.Provider
{
    public class CustomerDataProvider : IDisposable
    {
        private bool disposed = false;
        private string _dbPath = string.Empty;

        #region DataConnection

        public CustomerDataProvider()
        {
            //Copy Existing database
            CopyDatabase();

            // Get a reference to the SQLite database
            _dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "PointePayOffline.db");
        }

        ~CustomerDataProvider()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {

                }
                disposed = true;
            }
        }

        private void CopyDatabase()
        {
            IsolatedStorageFile ISF = IsolatedStorageFile.GetUserStoreForApplication();
            String DBFile = "PointePayOffline.db";
            if (!ISF.FileExists(DBFile)) CopyFromContentToStorage(ISF, "Assets/PointePayOffline.db", DBFile);
        }

        private void CopyFromContentToStorage(IsolatedStorageFile ISF, String SourceFile, String DestinationFile)
        {
            Stream Stream = Application.GetResourceStream(new Uri(SourceFile, UriKind.Relative)).Stream;
            IsolatedStorageFileStream ISFS = new IsolatedStorageFileStream(DestinationFile, System.IO.FileMode.Create, System.IO.FileAccess.Write, ISF);
            CopyStream(Stream, ISFS);
            ISFS.Flush();
            ISFS.Close();
            Stream.Close();
            ISFS.Dispose();
        }

        private void CopyStream(Stream Input, IsolatedStorageFileStream Output)
        {
            Byte[] Buffer = new Byte[5120];
            Int32 ReadCount = Input.Read(Buffer, 0, Buffer.Length);
            while (ReadCount > 0)
            {
                Output.Write(Buffer, 0, ReadCount);
                ReadCount = Input.Read(Buffer, 0, Buffer.Length);
            }
        }

        #endregion

        public bool AddCustomerOffline(CustomerOfflineViewModel newCustomerOffline, string _synced)
        {
            bool result = false;
            try
            {
                using (var db = new SQLite.SQLiteConnection(_dbPath))
                {
                    CustomerOffline objCustomerOffline = new CustomerOffline();

                    objCustomerOffline.customerId = Convert.ToString(newCustomerOffline.customerId);
                    objCustomerOffline.employeeId = Convert.ToString(newCustomerOffline.employeeId);
                    objCustomerOffline.organizationId = Convert.ToString(newCustomerOffline.organizationId);
                    objCustomerOffline.firstName = newCustomerOffline.firstName;
                    objCustomerOffline.lastName = newCustomerOffline.lastName;
                    objCustomerOffline.email = newCustomerOffline.email;
                    objCustomerOffline.phone = newCustomerOffline.phone;
                    objCustomerOffline.state = Convert.ToString(newCustomerOffline.state);
                    objCustomerOffline.city = Convert.ToString(newCustomerOffline.city);
                    objCustomerOffline.area = Convert.ToString(newCustomerOffline.area);
                    objCustomerOffline.addressLine1 = newCustomerOffline.addressLine1;
                    objCustomerOffline.street = newCustomerOffline.address_Line2;
                    objCustomerOffline.notes = "";
                    objCustomerOffline.stateName = newCustomerOffline.stateName;
                    objCustomerOffline.cityName = newCustomerOffline.cityName;
                    objCustomerOffline.areaName = newCustomerOffline.areaName;
                    objCustomerOffline.imageName = newCustomerOffline.imageName;

                    objCustomerOffline.synced = _synced;  // i.e. Need to synced when online and Update the synced status = "True"

                    db.RunInTransaction(() =>
                    {
                        db.Insert(objCustomerOffline);
                    });
                }

                result = true;
            }//try
            catch (Exception ex)
            {

            }//catch
            return result;
        }

        public bool UpdateCustomerOffline(CustomerRequest CustomerOffline, string _synced)
        {
            bool result = false;
            try
            {
                using (var db = new SQLite.SQLiteConnection(_dbPath))
                {
                    var objCustomerOffline = db.Query<PointePayApp.Model.CustomerOffline>("select * from CustomerOffline where customerId=" + CustomerOffline.customerId).FirstOrDefault();
                    if (objCustomerOffline != null)
                    {
                        ////objEmpOffline.employeeId = Convert.ToString(newEmployeeOffline.employeeId);
                        ////objEmpOffline.organizationId = Convert.ToString(newEmployeeOffline.organizationId);
                        objCustomerOffline.firstName = CustomerOffline.firstName;
                        objCustomerOffline.lastName = CustomerOffline.lastName;
                        objCustomerOffline.email = CustomerOffline.email;
                        objCustomerOffline.phone = CustomerOffline.phone;
                        objCustomerOffline.email = CustomerOffline.email;
                        ////objEmpOffline.state = Convert.ToString(newEmployeeOffline.state);
                        ////objEmpOffline.city = Convert.ToString(newEmployeeOffline.city);
                        ////objEmpOffline.area = Convert.ToString(newEmployeeOffline.area);
                        objCustomerOffline.addressLine1 = CustomerOffline.addressLine1;
                        objCustomerOffline.synced = _synced;  // i.e. Need to synced when online and Update the synced status = "True"

                        db.RunInTransaction(() =>
                        {
                            db.Update(objCustomerOffline);
                        });
                    }
                }

                result = true;
            }//try
            catch (Exception ex)
            {

            }//catch
            return result;
        }

        public bool DeleteAllCustomerOffline()
        {
            bool result = false;
            try
            {
                using (var db = new SQLite.SQLiteConnection(_dbPath))
                {
                    var CustomerOffline = db.Query<PointePayApp.Model.CustomerOffline>("select * from CustomerOffline").ToList();
                    if (CustomerOffline != null)
                    {
                        foreach (var itm in CustomerOffline)
                        {
                            //Dlete row
                            db.RunInTransaction(() =>
                            {
                                db.Delete(itm);
                            });
                        }
                    }
                }//using
                result = true;
            }//try
            catch (Exception ex)
            {

            }//catch
            return result;
        }

        public bool UpdatesyncedStatusCustomerOffline(int customerId)
        {
            bool result = false;
            try
            {
                using (var db = new SQLite.SQLiteConnection(_dbPath))
                {
                    var objCustomerOffline = db.Query<PointePayApp.Model.EmployeeOffline>("select * from CustomerOffline where customerId=" + customerId).FirstOrDefault();
                    if (objCustomerOffline != null)
                    {
                        //update word row
                        objCustomerOffline.synced = "True";
                        db.RunInTransaction(() =>
                        {
                            db.Update(objCustomerOffline);
                        });
                    }
                }//using
                result = true;
            }//try
            catch (Exception ex)
            {

            }//catch
            return result;
        }

        public List<CustomerOfflineViewModel> GetAllCustomerOfflineList()
        {
            List<CustomerOfflineViewModel> objList = new List<CustomerOfflineViewModel>();
            try
            {
                // Initialize the database if necessary
                using (var db = new SQLite.SQLiteConnection(_dbPath))
                {
                    objList = db.Query<PointePayApp.Model.CustomerOffline>("select * from CustomerOffline").Select(x => new CustomerOfflineViewModel
                    {
                        customerId = x.customerId,
                        employeeId = Convert.ToInt32(x.employeeId),
                        organizationId = Convert.ToInt32(x.organizationId),
                        firstName = x.firstName,
                        lastName = x.lastName,
                        email = x.email,
                        phone = x.phone,
                        state = x.state,
                        city = x.city,
                        area = x.area,
                        addressLine1 = x.addressLine1,
                        address_Line2 = x.street,
                        stateName = x.stateName,
                        cityName = x.cityName,
                        areaName = x.areaName,
                        imageName = x.imageName,
                        synced = x.synced

                    }).ToList();
                }//using

            }//try
            catch (Exception ex)
            {

            }//try
            return objList;
        }//GetAllCustomerOfflineList

        public List<CustomerOfflineViewModel> GetsyncedCustomerOfflineList(string synced)
        {
            List<CustomerOfflineViewModel> objList = new List<CustomerOfflineViewModel>();
            try
            {
                // Initialize the database if necessary
                using (var db = new SQLite.SQLiteConnection(_dbPath))
                {
                    objList = db.Query<PointePayApp.Model.CustomerOffline>("select * from CustomerOffline  Where synced='" + synced + "'").Select(x => new CustomerOfflineViewModel
                    {
                        customerId = x.customerId,
                        employeeId = Convert.ToInt32(x.employeeId),
                        organizationId = Convert.ToInt32(x.organizationId),
                        firstName = x.firstName,
                        lastName = x.lastName,
                        email = x.email,
                        phone = x.phone,
                        state = x.state,
                        city = x.city,
                        area = x.area,
                        addressLine1 = x.addressLine1,
                        address_Line2 = x.street,
                        stateName = x.stateName,
                        cityName = x.cityName,
                        areaName = x.areaName,
                        imageName = x.imageName,
                        synced = x.synced

                    }).ToList();
                }//using

            }//try
            catch (Exception ex)
            {

            }//try
            return objList;
        }//GetAllCustomerOfflineList
    }
}
