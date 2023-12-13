using SessionFramework;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace QBSDKWrapper
{
    public class QBSDKWrapperBase : IDisposable
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected readonly SessionManager sessionMgr;
        public string AttachDir { get; private set; }
        public string CompanyFile { get; private set; }

        protected QBSDKWrapperBase(string AppId, string AppName)
        {
            sessionMgr = SessionManager.getInstance(AppId, AppName);
        }

        public Status Connect(string companyFile)
        {
            CompanyFile = companyFile;
            string companydir = Path.GetFileNameWithoutExtension(companyFile);
            string companyfilepath = Path.GetDirectoryName(companyFile);
            AttachDir = companyfilepath + @"\attach\" + companydir + @"\Txn\";
            ErrorCode code;
            // One Connect to rule them all
            string message;
            try
            {
                sessionMgr.beginSession(companyFile);
                message = "Connected to Quickbooks Successfully.";
                code = ErrorCode.ConnectQBOK;
            }
            catch (Exception except)
            {
                log.Info("Connect", except);
                if (except is COMException exception)
                {
                    code = ErrorCode.NoConnection;
                    switch (exception.ErrorCode)
                    {
                        case unchecked((int)0x8004040A):
                            message = exception.Message;
                            break;
                        case unchecked((int)0x80040410):
                            message = "Quickbooks is currently open in Single User Mode.  Either close the company file, or re-open it in multi-user mode.";
                            break;
                        case unchecked((int)0x80040414):
                            message = "There is a window open in Quickbooks preventing QBConnector from accessing it.  Please close the window or the company in Quickbooks.";
                            break;
                        case unchecked((int)0x8004041B):
                            message = exception.Message;
                            break;
                        case unchecked((int)0x80040422):
                            message = exception.Message;
                            break;
                        default:
                            message = exception.Message;
                            break;
                    }
                }
                else
                {
                    message = except.Message;
                    code = ErrorCode.NoConnection;
                }
            }
            return new Status(message, code, 0);
        }

        public async Task<Status> ConnectAsync(string companyFile)
        {
            return await Task.Run(() => Connect(companyFile));
        }

        public void Disconnect()
        {
            sessionMgr.endSession();
        }


        #region IDisposable Members
        // Flag: Has Dispose already been called?
        private bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any managed objects here.
                //
            }
            // Free any unmanaged resources here.
            Disconnect();
            disposed = true;
        }
        #endregion
    }
}
