# QBSDKWrapper
Intuit offers their QBFC which can be used as convenience classes to access Quickbooks destop via their SDK.  Unfortunately, I 
found these not as convenient as I would like to so I wrote a wrapper for their wrapper.  It's like an onion.  

## Sample Usage
```
public class QBSDKWrapper : QBSDKWrapperBase
{

   public QBSDKWrapper() : base("MyAppGUID", "MyAppName")
   {

   }

   public async Task GetVendors()
   {
       IMsgSetRequest requestSet = sessionMgr.getMsgSetRequest();
       requestSet.Attributes.OnError = ENRqOnError.roeStop;
       IVendorQuery vendorQuery = requestSet.AppendVendorQueryRq();
       vendorQuery.ORVendorListQuery.VendorListFilter.ActiveStatus.SetValue(ENActiveStatus.asActiveOnly);
       vendorQuery.IncludeRetElementList.Add("Name");
       vendorQuery.IncludeRetElementList.Add("ListID");

       bool result = await Task.Run(() =>
       {
           bool retval = true;
           try
           {
               retval = sessionMgr.doRequests(ref requestSet);
           }
           catch (Exception except)
           {
               log.Error("GetVendorListAsync:doRequests", except);
           }
           return retval;
       });
       if (!result)
       {
           IVendorRetList vendorretlist = sessionMgr.getResponse(0) as IVendorRetList;
           if (vendorretlist.Count != 0)
           {
               for (int ndx = 0; ndx <= (vendorretlist.Count - 1); ndx++)
               {
                   IVendorRet vendorRet = vendorretlist.GetAt(ndx);
                   if (vendorRet.ListID == null)
                       continue;
                   Console.WriteLine($"{vendorRet.Name.GetValue()},{vendorRet.ListID.GetValue()}");
               }
           }
       }
   }   
}

//Using your newly found powers
            using (QBSDKWrapper qbconnector = new QBSDKWrapper())
            {
                Status status = await qbconnector.ConnectAsync(opts.CompanyFile);
                Console.WriteLine(status.GetFormattedMessage());
                if (status.Code != ErrorCode.ConnectQBOK) 
                    return -1;
                await qbconnector.GetVendors();
            }