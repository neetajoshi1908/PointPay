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
    public class EmployeeDataProvider : IDisposable
    {
        private bool disposed = false;
        private string _dbPath = string.Empty;

        #region DataConnection

        public EmployeeDataProvider()
        {
            //Copy Existing database
            CopyDatabase();

            // Get a reference to the SQLite database
            _dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "PointePayOffline.db");
        }

        ~EmployeeDataProvider()
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

        // Insert the newEmployeeOffline. 
        public bool AddEmployeeOffline(EmployeeRequest newEmployeeOffline,string _synced)
        {
            bool result = false;
            try
            {
                using (var db = new SQLite.SQLiteConnection(_dbPath))
                {
                    EmployeeOffline objEmployeeOffline = new EmployeeOffline();

                    objEmployeeOffline.staffEmployeeId = Convert.ToString(newEmployeeOffline.staffEmployeeId);
                    objEmployeeOffline.employeeId = Convert.ToString(newEmployeeOffline.employeeId);
                    objEmployeeOffline.organizationId = Convert.ToString(newEmployeeOffline.organizationId);
                    objEmployeeOffline.firstName = newEmployeeOffline.firstName;
                    objEmployeeOffline.lastName = newEmployeeOffline.lastName;
                    objEmployeeOffline.email = newEmployeeOffline.email;
                    objEmployeeOffline.businessPhoneCode = newEmployeeOffline.businessPhoneCode;
                    objEmployeeOffline.businessPhone = newEmployeeOffline.businessPhone;
                    objEmployeeOffline.designation = newEmployeeOffline.designation;
                    objEmployeeOffline.state = Convert.ToString(newEmployeeOffline.state);
                    objEmployeeOffline.city = Convert.ToString(newEmployeeOffline.city);
                    objEmployeeOffline.area = Convert.ToString(newEmployeeOffline.area);
                    objEmployeeOffline.stateName = newEmployeeOffline.stateName;
                    objEmployeeOffline.areaName = newEmployeeOffline.areaName;
                    objEmployeeOffline.cityName = newEmployeeOffline.cityName;
                    objEmployeeOffline.addressLine1 = newEmployeeOffline.addressLine1;
                    objEmployeeOffline.salary = newEmployeeOffline.salary;
                    objEmployeeOffline.userName = newEmployeeOffline.userName;
                    objEmployeeOffline.role = newEmployeeOffline.empRoleArray;
                    objEmployeeOffline.active = newEmployeeOffline.active;
                    objEmployeeOffline.synced = _synced;  // i.e. Need to synced when online and Update the synced status = "True"

                    db.RunInTransaction(() =>
                    {
                        db.Insert(objEmployeeOffline);
                    });
                }

                result = true;
            }//try
            catch (Exception ex)
            {

            }//catch
            return result;
        }

        public bool UpdateEmployeeOffline(EmployeeRequest newEmployeeOffline, string _synced)
        {
            bool result = false;
            try
            {
                using (var db = new SQLite.SQLiteConnection(_dbPath))
                {
                    var objEmpOffline = db.Query<PointePayApp.Model.EmployeeOffline>("select * from EmployeeOffline where staffEmployeeId=" + newEmployeeOffline.staffEmployeeId).FirstOrDefault();
                    if (objEmpOffline != null)
                    {
                        ////objEmpOffline.employeeId = Convert.ToString(newEmployeeOffline.employeeId);
                        ////objEmpOffline.organizationId = Convert.ToString(newEmployeeOffline.organizationId);
                        objEmpOffline.firstName = newEmployeeOffline.firstName;
                        objEmpOffline.lastName = newEmployeeOffline.lastName;
                        objEmpOffline.email = newEmployeeOffline.email;
                        objEmpOffline.businessPhoneCode = newEmployeeOffline.businessPhoneCode;
                        objEmpOffline.businessPhone = newEmployeeOffline.businessPhone;
                        objEmpOffline.designation = "";
                        ////objEmpOffline.state = Convert.ToString(newEmployeeOffline.state);
                        ////objEmpOffline.city = Convert.ToString(newEmployeeOffline.city);
                        ////objEmpOffline.area = Convert.ToString(newEmployeeOffline.area);
                        objEmpOffline.addressLine1 = newEmployeeOffline.addressLine1;
                        objEmpOffline.salary = newEmployeeOffline.salary;
                        objEmpOffline.userName = newEmployeeOffline.userName;
                        ////objEmpOffline.role = newEmployeeOffline.empRoleArray;
                        ////objEmpOffline.active = newEmployeeOffline.active;
                        objEmpOffline.synced = _synced;  // i.e. Need to synced when online and Update the synced status = "True"

                        db.RunInTransaction(() =>
                        {
                            db.Update(objEmpOffline);
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

        public List<EmployeeOfflineViewModel> GetsyncedEmployeeOfflineList(string synced)
        {
            List<EmployeeOfflineViewModel> objList = new List<EmployeeOfflineViewModel>();
            try
            {
                // Initialize the database if necessary
                using (var db = new SQLite.SQLiteConnection(_dbPath))
                {
                    objList = db.Query<PointePayApp.Model.EmployeeOffline>("select * from EmployeeOffline Where synced='" + synced + "'").Select(x => new EmployeeOfflineViewModel
                    {
                        staffEmployeeId = x.staffEmployeeId,
                        employeeId = x.employeeId,
                        organizationId = x.organizationId,
                        firstName = x.firstName,
                        lastName = x.lastName,
                        email = x.email,
                        businessPhoneCode = x.businessPhoneCode,
                        businessPhone = x.businessPhone,
                        designation = x.designation,
                        state = x.state,
                        city = x.city,
                        area = x.area,
                        addressLine1 = x.addressLine1,
                        salary = x.salary,
                        userName = x.userName,
                        role = x.role,
                        synced = x.synced,
                        cityName=x.cityName,
                        stateName=x.stateName,
                        areaName=x.areaName

                    }).ToList();
                }//using

            }//try
            catch (Exception ex)
            {

            }//try
            return objList;
        }//GetEmployeeOfflineList

        public List<EmployeeOfflineViewModel> GetAllEmployeeOfflineList()
        {
            List<EmployeeOfflineViewModel> objList = new List<EmployeeOfflineViewModel>();
            try
            {
                // Initialize the database if necessary
                using (var db = new SQLite.SQLiteConnection(_dbPath))
                {
                    objList = db.Query<PointePayApp.Model.EmployeeOffline>("select * from EmployeeOffline").Select(x => new EmployeeOfflineViewModel
                    {
                        staffEmployeeId = x.staffEmployeeId,
                        employeeId = x.employeeId,
                        organizationId = x.organizationId,
                        firstName = x.firstName,
                        lastName = x.lastName,
                        email = x.email,
                        businessPhoneCode = x.businessPhoneCode,
                        businessPhone = x.businessPhone,
                        designation = x.designation,
                        state = x.state,
                        city = x.city,
                        area = x.area,
                        addressLine1 = x.addressLine1,
                        salary = x.salary,
                        userName = x.userName,
                        role = x.role,
                        synced = x.synced,
                        active = x.active,
                        cityName = x.cityName,
                        stateName = x.stateName,
                        areaName = x.areaName

                    }).ToList();
                }//using

            }//try
            catch (Exception ex)
            {

            }//try
            return objList;
        }//GetEmployeeOfflineList

        public bool UpdatesyncedStatusEmployeeOffline(int staffEmployeeId)
        {
            bool result = false;
            try
            {
                using (var db = new SQLite.SQLiteConnection(_dbPath))
                {
                    var objEmployeeOffline = db.Query<PointePayApp.Model.EmployeeOffline>("select * from EmployeeOffline where staffEmployeeId=" + staffEmployeeId).FirstOrDefault();
                    if (objEmployeeOffline != null)
                    {
                        //update word row
                        objEmployeeOffline.synced = "True";
                        db.RunInTransaction(() =>
                        {
                            db.Update(objEmployeeOffline);
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

        public bool DeleteAllEmployeeOffline()
        {
            bool result = false;
            try
            {
                using (var db = new SQLite.SQLiteConnection(_dbPath))
                {
                    var objEmployeeOffline = db.Query<PointePayApp.Model.EmployeeOffline>("select * from EmployeeOffline").ToList();
                    if (objEmployeeOffline != null)
                    {
                        foreach(var itm in objEmployeeOffline)
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

    }
}
