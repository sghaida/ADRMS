﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Net.Mime;

using CCC.RMSLib;
using System.Diagnostics;

namespace RMSWS
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class ADRMS : IRMSWS
    {
        private EncryptionAndDecryption cryptor = new EncryptionAndDecryption();


        [DllImport(@"urlmon.dll", CharSet = CharSet.Auto)]
        private extern static System.UInt32 FindMimeFromData(
            System.UInt32 pBC,
            [MarshalAs(UnmanagedType.LPStr)] System.String pwzUrl,
            [MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer,
            System.UInt32 cbSize,
            [MarshalAs(UnmanagedType.LPStr)] System.String pwzMimeProposed,
            System.UInt32 dwMimeFlags,
            out System.UInt32 ppwzMimeOut,
            System.UInt32 dwReserverd
        );


        public string Protect(string ownerEmailAddress, string filePath, string templateName, string listOfRights)
        {

            TraceSource ts = new TraceSource("myTraceSource");

            // Writing out some events
            ts.TraceEvent(TraceEventType.Warning, 0, "warning message");
            ts.TraceEvent(TraceEventType.Error, 0, "error message");
            ts.TraceEvent(TraceEventType.Information, 0, "information message");
            ts.TraceEvent(TraceEventType.Critical, 0, "critical message");


            Collection<UserRights> userRights = new Collection<UserRights>();
            Collection<TemplateInfo> templatesInfo = cryptor.GetTemplatesInfo();

            if (!cryptor.IsEncrypted(filePath))
            {
                //If there exists a string of users rights
                if (!string.IsNullOrEmpty(listOfRights))
                {
                    userRights = HelperFunctions.ConvertRightsStringToCollection(listOfRights);

                    if (userRights.Count > 0)
                    {
                        //FileInfo fileInfo = new FileInfo(filePath);
                        cryptor.EncryptFile(ownerEmailAddress, userRights, filePath);

                        ts.TraceEvent(TraceEventType.Warning, 0, "Protect: File has been processed.");
                        return "Protect: File has been processed.";
                    }
                    else
                    {
                        ts.TraceEvent(TraceEventType.Warning, 0, "Protect: Please pass a valid rights string.");
                        return "Protect: Please pass a valid rights string.";
                    }
                }
                else if (!string.IsNullOrEmpty(templateName))
                {
                    var template = templatesInfo.FirstOrDefault(item => item.Name.ToLower() == templateName);

                    if (template != null)
                    {
                        cryptor.EncryptFile(filePath, template.TemplateId);
                        ts.TraceEvent(TraceEventType.Warning, 0, "Protect: File has been processed");
                        return "Protect: File has been processed.";
                    }
                    else
                    {
                        ts.TraceEvent(TraceEventType.Warning, 0, "Protect: Template does not exist, please choose an available template name.");
                        return "Protect: Template does not exist, please choose an available template name.";
                    }
                }
                else
                {
                    ts.TraceEvent(TraceEventType.Warning, 0, "Protect: Please pass either a rights string or a template name.");
                    return "Protect: Please pass either a rights string or a template name.";
                }
            }
            else
            {

                ts.TraceEvent(TraceEventType.Warning, 0, "Protect: File is already protected.");
                return "Protect: File is already protected.";
            }
        }

        public string Unprotect(string filePath)
        {

            TraceSource ts = new TraceSource("myTraceSource");

            // Writing out some events
            ts.TraceEvent(TraceEventType.Warning, 0, "warning message");
            ts.TraceEvent(TraceEventType.Error, 0, "error message");
            ts.TraceEvent(TraceEventType.Information, 0, "information message");
            ts.TraceEvent(TraceEventType.Critical, 0, "critical message");

            if (cryptor.IsEncrypted(filePath))
            {
                cryptor.DecryptFile(filePath);
                ts.TraceEvent(TraceEventType.Warning, 0, "Unprotect: File has been processed.");
                return "Unprotect: File has been processed.";
            }
            else
            {
                ts.TraceEvent(TraceEventType.Warning, 0, "Unprotect: File is not protected.");
                return "Unprotect: File is not protected.";
            }
        }

    }

}