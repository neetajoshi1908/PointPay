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
    public class CategoryDataProvider : IDisposable
    {
        private bool disposed = false;
        private string _dbPath = string.Empty;

        #region DataConnection

        public CategoryDataProvider()
        {
            //Copy Existing database
            CopyDatabase();

            // Get a reference to the SQLite database
            _dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "PointePayOffline.db");
        }

        ~CategoryDataProvider()
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

        public bool AddCategoryOffline(CategoryOfflineViewModel newCategoryOffline, string _synced)
        {
            bool result = false;
            try
            {
                using (var db = new SQLite.SQLiteConnection(_dbPath))
                {
                    CategoryOffline objCategoryOffline = new CategoryOffline();

                    objCategoryOffline.categoryId = Convert.ToString(newCategoryOffline.categoryId);
                    objCategoryOffline.organizationId = Convert.ToString(newCategoryOffline.organizationId);
                    objCategoryOffline.categoryCode = Convert.ToString(newCategoryOffline.categoryCode);
                    objCategoryOffline.categoryDescription = newCategoryOffline.categoryDescription;
                    objCategoryOffline.parentCategoryId = newCategoryOffline.parentCategoryId;
                    objCategoryOffline.imageName = newCategoryOffline.imageName;
                    objCategoryOffline.active = newCategoryOffline.active;

                    objCategoryOffline.synced = _synced;  // i.e. Need to synced when online and Update the synced status = "True"

                    db.RunInTransaction(() =>
                    {
                        db.Insert(objCategoryOffline);
                    });
                }

                result = true;
            }//try
            catch (Exception ex)
            {

            }//catch
            return result;
        }

        public bool UpdateCategoryOffline(CategoryRequest CustomerOffline, string _synced)
        {
            bool result = false;
            try
            {
                using (var db = new SQLite.SQLiteConnection(_dbPath))
                {
                    var objCategoryOffline = db.Query<PointePayApp.Model.CategoryOffline>("select * from CategoryOffline where categoryId=" + CustomerOffline.categoryId).FirstOrDefault();
                    if (objCategoryOffline != null)
                    {
                        //objCategoryOffline.categoryId = Convert.ToString(CustomerOffline.categoryId);
                        //objCategoryOffline.organizationId = Convert.ToString(CustomerOffline.organizationId);
                        objCategoryOffline.categoryCode = Convert.ToString(CustomerOffline.categoryCode);
                        objCategoryOffline.categoryDescription = CustomerOffline.categoryDescription;
                        objCategoryOffline.parentCategoryId = Convert.ToString(CustomerOffline.parentCategoryId);
                        //objCategoryOffline.active = CustomerOffline.active;

                        objCategoryOffline.synced = _synced;  // i.e. Need to synced when online and Update the synced status = "True"

                        db.RunInTransaction(() =>
                        {
                            db.Update(objCategoryOffline);
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

        public bool DeleteAllCategoryOffline()
        {
            bool result = false;
            try
            {
                using (var db = new SQLite.SQLiteConnection(_dbPath))
                {
                    var CategoryOffline = db.Query<PointePayApp.Model.CategoryOffline>("select * from CategoryOffline").ToList();
                    if (CategoryOffline != null)
                    {
                        foreach (var itm in CategoryOffline)
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

        public List<CategoryOfflineViewModel> GetAllCategoryOfflineList()
        {
            List<CategoryOfflineViewModel> objList = new List<CategoryOfflineViewModel>();
            try
            {
                // Initialize the database if necessary
                using (var db = new SQLite.SQLiteConnection(_dbPath))
                {
                    objList = db.Query<PointePayApp.Model.CategoryOffline>("select * from CategoryOffline").Select(x => new CategoryOfflineViewModel
                    {
                        categoryId = x.categoryId,
                        organizationId = x.organizationId,
                        categoryCode = x.categoryCode,
                        categoryDescription = x.categoryDescription,
                        parentCategoryId = x.parentCategoryId,
                        active = x.active,
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

        public List<CategoryOfflineViewModel> GetsyncedCategoryOfflineList(string synced)
        {
            List<CategoryOfflineViewModel> objList = new List<CategoryOfflineViewModel>();
            try
            {
                // Initialize the database if necessary
                using (var db = new SQLite.SQLiteConnection(_dbPath))
                {
                    objList = db.Query<PointePayApp.Model.CategoryOffline>("select * from CategoryOffline  Where synced='" + synced + "'").Select(x => new CategoryOfflineViewModel
                    {
                        categoryId = x.categoryId,
                        organizationId = x.organizationId,
                        categoryCode = x.categoryCode,
                        categoryDescription = x.categoryDescription,
                        parentCategoryId = x.parentCategoryId,
                        active = x.active,
                        imageName = x.imageName,
                        synced = x.synced

                    }).ToList();
                }//using

            }//try
            catch (Exception ex)
            {

            }//try
            return objList;
        }//synced
    }
}
